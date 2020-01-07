using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace TransLucency
{
    [Serializable()]
    internal static class NativeMethods
    {
        [SecurityCritical]
        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        internal abstract class CriticalHandle : CriticalFinalizerObject
        {
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool IsWindowVisible(long hWnd);

            [SecurityCritical]
            internal const uint WM_QUIT = 0x12;
            [DllImport("user32.dll", SetLastError = true, EntryPoint = "PostThreadMessage", CharSet = CharSet.Unicode)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool PostThreadMessage(int idThread, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", SetLastError = true)]
            internal static extern int SetWindowLong(long hWnd, int nIndex, int dwNewLong);

            [DllImport("user32.dll", SetLastError = true)]
            internal static extern int GetWindowLong(long hWnd, int GWL_EXSTYLE);

            public const int GWL_EXSTYLE = -20;
            public const int WS_EX_LAYERED = 0x80000;
            public const int LWA_ALPHA = 0x2;
            public const int LWA_COLORKEY = 0x1;

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool SetLayeredWindowAttributes(long hwnd, long crKey, byte bAlpha, long dwFlags);

            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            internal static extern long FindWindow(string lpClassName, string lpWindowName);

            [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            static extern uint RegisterWindowMessage(string msgString);

            static public readonly uint WM_TASKBARCREATED = RegisterWindowMessage("TaskbarCreated");
        }
    }
}
