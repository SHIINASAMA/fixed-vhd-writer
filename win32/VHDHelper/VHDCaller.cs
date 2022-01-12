using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VHDHelper
{
    public class VHDCaller
    {
        [DllImport("libWriter.dll", EntryPoint = "write")]
        public static extern int Write(string VHDName, int lba, string DataName);
    }
}
