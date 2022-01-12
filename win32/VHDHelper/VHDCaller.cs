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
        [DllImport("libWriter.dll")]
        public static extern int write(string VHDName, int lba, string DataName);
    }
}
