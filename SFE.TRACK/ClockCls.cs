using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using MachineDefine;
using SFE.TRACK.Model;

namespace SFE.TRACK
{
    public class ClockCls : DependencyObject
    {
        public static DependencyProperty STDateTimeProperty_ = DependencyProperty.Register("DateTime", typeof(DateTime), typeof(ClockCls));
        DispatcherTimer timer = new DispatcherTimer();
        string ioName = string.Empty;
        bool isStatus = false;
        public DateTime DateTime
        {
            set { SetValue(STDateTimeProperty_, value); }
            get { return (DateTime)GetValue(STDateTimeProperty_); }
        }        

        public ClockCls()
        {
            
            timer.Tick += TimerOnTick;
            timer.Interval = TimeSpan.FromMilliseconds(500);
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

            foreach(IODataCls ioData in Global.STDIList)
            {
                if (ioData.Company.ToUpper() == "AJINECAT")
                    ioData.State = ioData.IO.IsActive();
            }

            foreach (IODataCls ioData in Global.STDOList)
            {
                if (ioData.Company.ToUpper() == "AJINECAT")
                    ioData.State = ioData.IO.IsActive();
            }

            foreach(AxisInfoCls axis in Global.STAxis)
            {
                if (axis.Company.ToUpper() == "AJINECAT")
                {
                    axis.PlusLimit = axis.Motor.IsPositiveLimit;
                    axis.MinusLimit = axis.Motor.IsNegativeLimit;
                    axis.Servo = axis.Motor.IsServoOn;
                    axis.InMotion = axis.Motor.IsMoving;
                    axis.InPosition = axis.Motor.IsInPosition;
                    axis.IsStop = !axis.Motor.IsMoving;
                    axis.Alarm = axis.Motor.IsAlarm;
                    axis.IsHome = axis.Motor.IsHome;
                    axis.ActualPosition = axis.Motor.GetEncoderPos() / 1000;
                    axis.CommandPosition = axis.Motor.CommandPosition / 1000;
                }
            }
        }
    }
}
