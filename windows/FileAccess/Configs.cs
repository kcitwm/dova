using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dova.Files
{
    public class Configs : Config
    {
        public static string FileStorageRoot = @"D:\FileStorage\";

        static Configs()
        {
            FileStorageRoot = Get("DefaultFileDriver", FileStorageRoot);
        }

    }
}
