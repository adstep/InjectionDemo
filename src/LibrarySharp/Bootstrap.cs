using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using RGiesecke.DllExport;

namespace LibrarySharp
{
    public static class Bootstrap
    {
        [DllExport("Run", CallingConvention.StdCall)]
        public static void Run([MarshalAs(UnmanagedType.LPWStr)] string args)
        {
            var process = Process.GetCurrentProcess();
            MessageBox.Show($"Current Process: {process.Id} Args: {args}", "LibrarySharp");
        }
    }
}
