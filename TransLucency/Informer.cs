using System.Threading;
using System.Windows.Forms;

namespace TransLucency
{
    public partial class Informer : Form
    {
        public Informer()
        {
            Startup();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.CriticalHandle.WM_TASKBARCREATED)
            {
                FileExplorer.shellWindows.WindowRegistered -= FileExplorer.ShellWindows_WindowRegistered;

                Startup();
            }
            base.WndProc(ref m);
        }

        private void Startup()
        {
            Thread _initializeTaskbar = new Thread(() => Taskbar.SetOpacity())
            {
                IsBackground = true
            };
            _initializeTaskbar.Start();

            Thread _initializeExplorer = new Thread(() => FileExplorer.SetOpacity())
            {
                IsBackground = true
            };
            _initializeExplorer.Start();
        }
    }
}
