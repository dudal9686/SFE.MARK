using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace SFE.TRACK
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool Shutdown_;
        public MainWindow()
        {
            InitializeComponent();
        }

        public async void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e.Cancel) return;

            // we want manage the closing itself!
            e.Cancel = !this.Shutdown_;
            // yes we want now really close the window
            if (this.Shutdown_) return;


            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Quit",
                NegativeButtonText = "Cancel",
                AnimateShow = true,
                AnimateHide = true
            };

            var result = await this.ShowMessageAsync(
                "Quit application?",
                "Sure you want to quit application?",
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (result == MessageDialogResult.Affirmative)
                this.Shutdown_ = true;

            if (this.Shutdown_)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
    }
}
