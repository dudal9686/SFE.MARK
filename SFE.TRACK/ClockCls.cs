using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SFE.TRACK
{
    public class ClockCls : DependencyObject
    {
        public static DependencyProperty STDateTimeProperty_ = DependencyProperty.Register("DateTime", typeof(DateTime), typeof(ClockCls));
        DispatcherTimer timer = new DispatcherTimer();
        public DateTime DateTime
        {
            set { SetValue(STDateTimeProperty_, value); }
            get { return (DateTime)GetValue(STDateTimeProperty_); }
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
        }
    }
}
