namespace PortableDevices
{
    public abstract class PortableDeviceObject
    {
        protected PortableDeviceObject(string id, string name, ulong size)
        {
            this.Id = id;
            this.Name = name;
            this.Size = size;
        }

        public string Id { get; private set; }

        public string Name { get; private set; }

        public ulong Size { get; private set; }
    }
}