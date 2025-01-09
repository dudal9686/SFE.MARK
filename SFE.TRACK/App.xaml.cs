using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Globalization;

namespace SFE.TRACK
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        Mutex mutex;
        public App()
        {
            string mutexName = "SFE.TRACK";
            bool isProcess;
            mutex = new Mutex(true, mutexName, out isProcess);

            if (!isProcess)
            {
                System.Windows.MessageBox.Show("program is already running.(SFE.MARK)");
                Application.Current.Shutdown();
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Process[] process = Process.GetProcessesByName("IPCNetServer");
            if (process.Length == 0)
            {
                try
                {
                    System.Diagnostics.Process.Start(@"C:\MachineSet\IPCNetServer\IPCNetServer.exe");
                }
                catch { }
            }
            //en-US, zh-CN
            CultureInfo cultureInfo = new CultureInfo(SFE.TRACK.Properties.Settings.Default.LANG_CODE);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;            
        }
    }
}
