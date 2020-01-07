using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using static TransLucency.NativeMethods.CriticalHandle;
using static TransLucency.Properties.Settings;

namespace TransLucency
{
    public class Taskbar
    {
        internal static void SetOpacity(bool closing = false)
        {
            var DesiredTransparency = Default.TaskbarTransparency;

            byte ActualOpacity = 255;

            var TaskBarHWND = FindWindow("Shell_traywnd", "");

            if (!closing)
            {
                while (ActualOpacity > DesiredTransparency)
                {
                    try
                    {
                        ActualOpacity--;

                        SetWindowLong(TaskBarHWND, GWL_EXSTYLE,
                               GetWindowLong(TaskBarHWND, GWL_EXSTYLE) | WS_EX_LAYERED).GetTypeCode();
                        SetLayeredWindowAttributes(TaskBarHWND, LWA_COLORKEY, ActualOpacity, LWA_ALPHA);

                        Thread.Sleep(4);
                    }
                    catch (Win32Exception)
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
            }
            else
            {
                try
                {
                    SetWindowLong(TaskBarHWND, GWL_EXSTYLE,
                               GetWindowLong(TaskBarHWND, GWL_EXSTYLE) | WS_EX_LAYERED).GetTypeCode();
                    SetLayeredWindowAttributes(TaskBarHWND, LWA_COLORKEY, ActualOpacity, LWA_ALPHA);
                }
                catch (Win32Exception)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
        }
    }
}
