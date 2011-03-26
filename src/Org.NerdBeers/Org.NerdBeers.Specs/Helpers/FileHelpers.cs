using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Org.NerdBeers.Specs.Helpers
{
    static class FileHelpers
    {
        public static string GetFullPath(string fn)
        {
            return Path.Combine(Path.GetDirectoryName(typeof(FileHelpers).Assembly.Location), fn);
        }
    }
}
