using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace SFE.TRACK
{
    public class ClockCls : DependencyObject
    {
        public static DependencyProperty STDateTimeProperty_ = DependencyProperty.Register("DateTime", typeof(DateTime), typeof(ClockCls));
        public static DependencyProperty STRobotConnection_ = DependencyProperty.Register("RobotConnectColor", typeof(System.Windows.Media.SolidColorBrush), typeof(ClockCls));
        public static DependencyProperty STChamberConnection_ = DependencyProperty.Register("ChamberConnectColor", typeof(System.Windows.Media.SolidColorBrush), typeof(ClockCls));
        DispatcherTimer timer = new DispatcherTimer();
        public DateTime DateTime
        {
            set { SetValue(STDateTimeProperty_, value); }
            get { return (DateTime)GetValue(STDateTimeProperty_); }
        }

        public System.Windows.Media.SolidColorBrush RobotConnectColor
        {
            set { SetValue(STRobotConnection_, value); }
            get { return (System.Windows.Media.SolidColorBrush)GetValue(STRobotConnection_); }
        }

        public System.Windows.Media.SolidColorBrush ChamberConnectColor
        {
            set { SetValue(STChamberConnection_, value); }
            get { return (System.Windows.Media.SolidColorBrush)GetValue(STChamberConnection_); }
        }

        public ClockCls()
        {
            
            timer.Tick += TimerOnTick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        ~ClockCls()
        {
            timer.Stop();
            timer = null;
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            DateTime = DateTime.Now;

            if (Global.MachineWorker == null) return;

            Dictionary<int, string> list = Global.MachineWorker.GetCmdHelper().GetClientList();

            if (list.ContainsKey(Global.CHAMBER_ID)) { ChamberConnectColor = Brushes.Green; Global.IsChamberConnection = true; }
            else { ChamberConnectColor = Brushes.Red; Global.IsChamberConnection = false; }

            if (list.ContainsKey(Global.MCS_ID)) { RobotConnectColor = Brushes.Green; Global.IsMCSConnection = true; }
            else { RobotConnectColor = Brushes.Red; Global.IsMCSConnection = false; }
        }
    }
}
