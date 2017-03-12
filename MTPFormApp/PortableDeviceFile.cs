namespace PortableDevices
{
    public class PortableDeviceFile : PortableDeviceObject
    {
        public PortableDeviceFile(string id, string name, ulong size) : base(id, name, size)
        {
        }

        public string Path { get; set; }
    }
}