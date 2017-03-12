using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortableDeviceApiLib;

namespace PortableDevices
{
    class Program
    {
        static void Main(string[] args)
        {
            var devices = new PortableDeviceCollection();
            devices.Refresh();
            var device = devices.First();

            device.Connect();

            PortableDeviceFolder folder = device.GetContents();
            Console.WriteLine(folder.Name);
            foreach (PortableDeviceObject obj in folder.Files)
            {
                Console.WriteLine(obj.Name);
            }

            device.Disconnect();

            Console.WriteLine();
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }
    }
}
