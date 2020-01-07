using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using static TransLucency.NativeMethods.CriticalHandle;
using static TransLucency.Properties.Settings;

[assembly: CLSCompliant(true)]
namespace TransLucency
{
    static class Program
    {
        static Informer inform;

        public static Mutex Mutex { get; } = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");

        [STAThread]
        static void Main()
        {
            if (Mutex.WaitOne(TimeSpan.Zero, true))
            {
                try
                {
                    StartApp();
                }
                finally
                {
                    Mutex.ReleaseMutex();
                }
            }
            else
            {
                ExitApp();
            }
        }

        private static void StartApp()
        {
            if (Default.UpgradeRequired)
            {
                Default.Upgrade();
                Default.UpgradeRequired = false;
                Default.Save();

                if (Default.FirstRun)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    Default.ExplorerTransparency = 235;
                    Default.TaskbarTransparency = 235;
                    Default.FirstRun = false;
                    Default.Save();

                    var results = MessageBox.Show("TransLucency saves settings in LocalAppData. Would you like to open the default location and change the values?", "TransLucency", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (results == DialogResult.Yes)
                    {
                        try
                        {
                            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\xCONFLiCTiONx");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "TransLucency", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }

            IdleStart();
        }

        private static void IdleStart()
        {
            while (true)
            {
                long hWnd = FindWindow("Shell_TrayWnd", null);

                if (hWnd != 0)
                {
                    if (IsWindowVisible(hWnd))
                    {
                        break;
                    }
                }

                Thread.Sleep(2000);
            }

            inform = new Informer();
            inform.Disposed += delegate
            {
                inform.Dispose();
                inform = null;
            };
            inform.Visible = false;
            inform.WindowState = FormWindowState.Minimized;
            inform.Show();
            inform.Hide();

            Application.Run();
        }

        private static void ExitApp()
        {
            FileExplorer.SetOpacity(true);

            Taskbar.SetOpacity(true);

            Process current = Process.GetCurrentProcess();

            foreach (Process process in Process.GetProcessesByName(current.ProcessName))
            {
                try
                {
                    if (process.Id != current.Id)
                    {
                        foreach (ProcessThread processThread in process.Threads)
                        {
                            NativeMethods.CriticalHandle.PostThreadMessage(processThread.Id, NativeMethods.CriticalHandle.WM_QUIT, IntPtr.Zero, IntPtr.Zero);
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            Environment.Exit(0);
        }
    }
}
