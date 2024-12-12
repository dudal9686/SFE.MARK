using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SFE.TRACK
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        
        public App()
        {
            Process[] process = Process.GetProcessesByName("IPCNetServer");
            if(process.Length == 0)
            {

            }
        }
    }
}
