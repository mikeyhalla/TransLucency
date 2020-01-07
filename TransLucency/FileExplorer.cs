using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using static TransLucency.NativeMethods.CriticalHandle;
using static TransLucency.Properties.Settings;

namespace TransLucency
{
    public class FileExplorer
    {
        internal static SHDocVw.ShellWindows shellWindows;

        private static readonly List<long> ExplorerHandles = new List<long>();

        internal static void SetOpacity(bool closing = false)
        {
            shellWindows = new SHDocVw.ShellWindows();

            if (!closing)
            {
                shellWindows.WindowRegistered += ShellWindows_WindowRegistered;
            }

            foreach (SHDocVw.InternetExplorer explorer in shellWindows)
            {
                string filename = Path.GetFileNameWithoutExtension(explorer.FullName).ToLower();

                if (filename.Equals("explorer"))
                {
                    if (!closing)
                    {
                        Thread thread = new Thread(() => SetOpacity(explorer.HWND, closing))
                        {
                            IsBackground = true
                        };
                        thread.Start();
                    }
                    else
                    {
                        SetOpacity(explorer.HWND, closing);
                    }
                }
            }
        }

        internal static void ShellWindows_WindowRegistered(int lCookie)
        {
            List<long> UpdatedExplorerHandles = new List<long>();

            foreach (SHDocVw.InternetExplorer explorer in shellWindows)
            {
                string filename = Path.GetFileNameWithoutExtension(explorer.FullName).ToLower();

                if (filename.Equals("explorer"))
                {
                    UpdatedExplorerHandles.Add(explorer.HWND);
                }
            }

            List<long> FilteredExplorerHandles = UpdatedExplorerHandles.Except(ExplorerHandles).ToList();

            foreach (var term in FilteredExplorerHandles)
            {
                Thread thread = new Thread(() => SetOpacity(term))
                {
                    IsBackground = true
                };
                thread.Start();
            }

            ExplorerHandles.Clear();

            foreach (var term in UpdatedExplorerHandles)
            {
                ExplorerHandles.Add(term);
            }
        }

        internal static void SetOpacity(long handle, bool closing = false)
        {
            byte ActualOpacity = 255;

            if (!closing)
            {
                Thread.Sleep(160);

                byte DesiredTransparency = Default.ExplorerTransparency;

                while (ActualOpacity > DesiredTransparency)
                {
                    try
                    {
                        ActualOpacity--;

                        SetWindowLong(handle, GWL_EXSTYLE,
                                      GetWindowLong(handle, GWL_EXSTYLE) | WS_EX_LAYERED).GetTypeCode();
                        SetLayeredWindowAttributes(handle, LWA_COLORKEY, ActualOpacity, LWA_ALPHA);

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
                    SetWindowLong(handle, GWL_EXSTYLE,
                                      GetWindowLong(handle, GWL_EXSTYLE) | WS_EX_LAYERED).GetTypeCode();
                    SetLayeredWindowAttributes(handle, LWA_COLORKEY, ActualOpacity, LWA_ALPHA);

                    shellWindows.WindowRegistered -= ShellWindows_WindowRegistered;
                }
                catch (Win32Exception)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
        }
    }
}
