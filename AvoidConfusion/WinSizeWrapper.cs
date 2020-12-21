using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace AvoidConfusion
{
    class WinSizeWrapper
    {


        ///<summary>
        ///Sends a message to a window.
        ///</summary>
        ///<param name="hWnd">The window to handle the message.</param>
        ///<param name="msg">The message to send.</param>
        ///<param name="lParam">The low parameter.</param>
        ///<param name="hParam">The high parameter.</param>
        [DllImport("User32.dll")]
        protected static extern unsafe void* SendMessage(int* hWnd, uint msg, uint hParam, uint lParam);
        public static unsafe void MaximizeWindow(int* windowHandle) => SendMessage(windowHandle, 0x0112, 0xF030, 0);

        public static unsafe void MaximizeThisWindow() => 
            MaximizeWindow((int*)Process.GetCurrentProcess().Handle.ToPointer());
    }
}
