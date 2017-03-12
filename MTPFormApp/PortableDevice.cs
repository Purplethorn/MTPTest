using System;
using System.IO;
using PortableDeviceApiLib;
using PortableDeviceTypesLib;
using _tagpropertykey = PortableDeviceApiLib._tagpropertykey;
using IPortableDeviceKeyCollection = PortableDeviceApiLib.IPortableDeviceKeyCollection;
using IPortableDeviceValues = PortableDeviceApiLib.IPortableDeviceValues;
using System.Runtime.InteropServices;

namespace PortableDevices
{
    public class PortableDevice
    {
        private bool _isConnected;
        private readonly PortableDeviceClass _device;
        private IPortableDeviceContent _content;
        private IPortableDeviceProperties _properties;

        internal PortableDeviceClass PortableDeviceClass
        {
            get
            {
                return this._device;
            }
        }

        public string DeviceId { get; set; }

        public PortableDevice(string deviceId)
        {
            this._device = new PortableDeviceClass();
            this.DeviceId = deviceId;
        }

        public void Connect()
        {
            if (this._isConnected) { return; }

            IPortableDeviceValues clientInfo = (IPortableDeviceValues)new PortableDeviceValuesClass();
            this._device.Open(this.DeviceId, clientInfo);
            this._isConnected = true;
        }

        public void Prepare()
        {
            // Get the content of the device
            this._device.Content(out this._content);

            // Get the properties of the object
            this._content.Properties(out this._properties);
        }

        public void Disconnect()
        {
            if (!this._isConnected) { return; }
            this._device.Close();
            this._isConnected = false;
        }

        public void TransferContentToDevice(string fileName, PortableDeviceFolder parentFolder)
        {
            string parentObjectId = parentFolder.Id;

            IPortableDeviceContent content;
            this._device.Content(out content);

            IPortableDeviceValues values =
                GetRequiredPropertiesForContentType(fileName, parentObjectId);

            PortableDeviceApiLib.IStream tempStream;
            uint optimalTransferSizeBytes = 0;
            content.CreateObjectWithPropertiesAndData(
                values,
                out tempStream,
                ref optimalTransferSizeBytes,
                null);

            System.Runtime.InteropServices.ComTypes.IStream targetStream =
                (System.Runtime.InteropServices.ComTypes.IStream)tempStream;
            try
            {
                using (var sourceStream =
                    new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[optimalTransferSizeBytes];
                    int bytesRead;
                    do
                    {
                        bytesRead = sourceStream.Read(
                            buffer, 0, (int)optimalTransferSizeBytes);
                        IntPtr pcbWritten = IntPtr.Zero;
                        targetStream.Write(
                            buffer, bytesRead, pcbWritten);
                    } while (bytesRead > 0);
                }
                targetStream.Commit(0);
            }
            finally
            {
                Marshal.ReleaseComObject(tempStream);
            }
        }

        private IPortableDeviceValues GetRequiredPropertiesForContentType(
            string fileName,
            string parentObjectId)
        {
            IPortableDeviceValues values =
                new PortableDeviceTypesLib.PortableDeviceValues() as IPortableDeviceValues;

            var WPD_OBJECT_PARENT_ID = new _tagpropertykey();
            WPD_OBJECT_PARENT_ID.fmtid =
                new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
                         0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_PARENT_ID.pid = 3;
            values.SetStringValue(ref WPD_OBJECT_PARENT_ID, parentObjectId);

            FileInfo fileInfo = new FileInfo(fileName);
            var WPD_OBJECT_SIZE = new _tagpropertykey();
            WPD_OBJECT_SIZE.fmtid =
                new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
                         0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_SIZE.pid = 11;
            values.SetUnsignedLargeIntegerValue(WPD_OBJECT_SIZE, (ulong)fileInfo.Length);

            var WPD_OBJECT_ORIGINAL_FILE_NAME = new _tagpropertykey();
            WPD_OBJECT_ORIGINAL_FILE_NAME.fmtid =
                new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
                         0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_ORIGINAL_FILE_NAME.pid = 12;
            values.SetStringValue(WPD_OBJECT_ORIGINAL_FILE_NAME, Path.GetFileName(fileName));

            var WPD_OBJECT_NAME = new _tagpropertykey();
            WPD_OBJECT_NAME.fmtid =
                new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
                         0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_NAME.pid = 4;
            values.SetStringValue(WPD_OBJECT_NAME, Path.GetFileName(fileName));

            return values;
        }


        public void DownloadFile(PortableDeviceFile file, string saveToPath)
        {
            IPortableDeviceContent content;
            this._device.Content(out content);

            IPortableDeviceResources resources;
            content.Transfer(out resources);

            PortableDeviceApiLib.IStream wpdStream;
            uint optimalTransferSize = 0;

            var property = new _tagpropertykey();
            property.fmtid = new Guid(0xE81E79BE, 0x34F0, 0x41BF, 0xB5, 0x3F, 0xF1, 0xA0, 0x6A, 0xE8, 0x78, 0x42);
            property.pid = 0;

            resources.GetStream(file.Id, ref property, 0, ref optimalTransferSize, out wpdStream);

            System.Runtime.InteropServices.ComTypes.IStream sourceStream = (System.Runtime.InteropServices.ComTypes.IStream)wpdStream;

            //var filename = Path.GetFileName(file.Name);
            FileStream targetStream = new FileStream(saveToPath, FileMode.Create, FileAccess.Write);

            unsafe
            {
                var buffer = new byte[1024];
                int bytesRead;
                do
                {
                    sourceStream.Read(buffer, 1024, new IntPtr(&bytesRead));
                    targetStream.Write(buffer, 0, 1024);
                } while (bytesRead > 0);
                targetStream.Close();
            }
        }

        public PortableDeviceFolder GetRootContent(bool isRecurse)
        {
            var root = new PortableDeviceFolder("DEVICE", "DEVICE", 0);

            return this.GetContents(root, isRecurse);
        }

        public PortableDeviceFolder GetContents(PortableDeviceFolder folder, bool isRecurse)
        {

            EnumerateContents(ref this._content, ref this._properties, folder);

            if (isRecurse)
            {
                foreach (PortableDeviceObject obj in folder.Files)
                {
                    if (obj is PortableDeviceFolder)
                    {
                        this.GetContents((PortableDeviceFolder)obj, isRecurse);
                    }
                }
            }

            return folder;
        }

        private static void EnumerateContents(
            ref IPortableDeviceContent content,
            ref IPortableDeviceProperties properties,
            PortableDeviceFolder parent)
        {

            // Enumerate the items contained by the current object
            IEnumPortableDeviceObjectIDs objectIds;
            content.EnumObjects(0, parent.Id, null, out objectIds);

            uint fetched = 0;
            do
            {
                string objectId;

                objectIds.Next(1, out objectId, ref fetched);
                if (fetched > 0)
                {
                    var currentObject = WrapObject(properties, objectId);
                    parent.Files.Add(currentObject);
                }
            } while (fetched > 0);
        }

        private static PortableDeviceObject WrapObject(IPortableDeviceProperties properties,
            string objectId)
        {
            IPortableDeviceKeyCollection keys;
            properties.GetSupportedProperties(objectId, out keys);

            IPortableDeviceValues values;
            properties.GetValues(objectId, keys, out values);

            // Get the name of the object
            string name;
            var property = new _tagpropertykey();
            property.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
                                      0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            property.pid = 4;
            values.GetStringValue(property, out name);

            // Get the type of the object
            Guid contentType;
            property = new _tagpropertykey();
            property.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
                                      0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            property.pid = 7;
            values.GetGuidValue(property, out contentType);
            
            // Check if the object is folder or file
            var folderType = new Guid(0x27E2E392, 0xA111, 0x48E0, 0xAB, 0x0C,
                                      0xE1, 0x77, 0x05, 0xA0, 0x5F, 0x85);
            var functionalType = new Guid(0x99ED0160, 0x17FF, 0x4C44, 0x9D, 0x98,
                                          0x1D, 0x7A, 0x6F, 0x94, 0x19, 0x21);
            if (contentType == folderType || contentType == functionalType)
            {
                return new PortableDeviceFolder(objectId, name, 0);
            }
            else
            {
                // Get the name of the object
                property = new _tagpropertykey();
                property.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
                                          0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
                property.pid = 12;
                values.GetStringValue(property, out name);

                // Get the size of the object
                ulong size;
                property = new _tagpropertykey();
                property.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
                                          0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
                property.pid = 11;
                values.GetUnsignedLargeIntegerValue(property, out size);

                return new PortableDeviceFile(objectId, name, size);
            }
        }

    }
}
