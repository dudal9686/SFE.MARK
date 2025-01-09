using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SFE.TRACK.Model;
using CoreCSRunSim;
using System.Threading;

namespace SFE.TRACK.ViewModel.Motor
{
    public class MotorControlViewModel : ViewModelBase
    {
        List<AxisInfoCls> list { get; set; }
        string AxisInfo_ = "Axis Status";
        AxisInfoCls Axis_ { get; set; }
        bool[] isVelocity = new bool[3] { true, false, false };
        public RelayCommand<object> VelocityGridDoubleClickRelayCommand { get; set; }        
        public RelayCommand MouseDownPlusRelayCommand { get; set; }
        public RelayCommand MouseUpPlusRelayCommand { get; set; }
        public RelayCommand MouseDownMinusRelayCommand { get; set; }
        public RelayCommand MouseUpMinusRelayCommand { get; set; }
        public RelayCommand FirstMoveRelayCommand { get; set; }
        public RelayCommand SecondMoveRelayCommand { get; set; }
        public RelayCommand FirstTeachingRelayCommand { get; set; }
        public RelayCommand SecondTeachingRelayCommand { get; set; }
        public RelayCommand RepeatRelayCommand { get; set; }
        public RelayCommand PitchClickRelayCommand { get; set; }
        TSpeedPack speedPack = new TSpeedPack();
        float pitch = 0.1f;        

        public MotorControlViewModel()
        {
            VelocityGridDoubleClickRelayCommand = new RelayCommand<object>(VelocityGridDoubleClickCommand);
            
            MouseDownPlusRelayCommand = new RelayCommand(MouseDownPlusCommand);
            MouseUpPlusRelayCommand = new RelayCommand(MouseUpPlusCommand);
            MouseDownMinusRelayCommand = new RelayCommand(MouseDownMinusCommand);
            MouseUpMinusRelayCommand = new RelayCommand(MouseUpMinusCommand);
            FirstMoveRelayCommand = new RelayCommand(FirstMoveCommand);
            SecondMoveRelayCommand = new RelayCommand(SecondMoveCommand);
            FirstTeachingRelayCommand = new RelayCommand(FirstTeachingCommand);
            SecondTeachingRelayCommand = new RelayCommand(SecondTeachingCommand);
            PitchClickRelayCommand = new RelayCommand(PitchClickCommand);
            RepeatRelayCommand = new RelayCommand(RepeatCommand);
        }        

        public bool[] IsVelocity
        {
            get { return isVelocity; }
            set { isVelocity = value; RaisePropertyChanged("IsVelocity"); }
        }

        public AxisInfoCls Axis
        {
            get { return Axis_; }
            set { Axis_ = value; RaisePropertyChanged("Axis"); }
        }

        public float Pitch
        {
            get { return pitch; }
            set { pitch = value; RaisePropertyChanged("Pitch"); }
        }

        private void MouseDownPlusCommand()
        {
            if (IsVelocity[0])
            {
                speedPack.acc = 100;
                speedPack.dec = 100;
                speedPack.speed = 10;
                Axis.Motor.DoVelocityMove(speedPack);
            }
            else if (IsVelocity[1])
            {
                speedPack.acc = 100;
                speedPack.dec = 100;
                speedPack.speed = 50;
                Axis.Motor.DoVelocityMove(speedPack);
            }
            else if (IsVelocity[2])
            {
                speedPack.acc = Axis.ACC;
                speedPack.dec = Axis.DEC;
                speedPack.speed = Axis.VEL;
                Axis.Motor.DoSCurveMove((double)Pitch, speedPack, UnitMotor.EnumMovePosType.INCREMENTAL);
            }
            Console.WriteLine("AAAAA");
        }

        private void MouseUpPlusCommand()
        {
            if (IsVelocity[0] || IsVelocity[1])
            {
                Axis.Motor.StopMove();
            }
        }

        private void MouseDownMinusCommand()
        {
            if (IsVelocity[0])
            {
                speedPack.acc = 100;
                speedPack.dec = 100;
                speedPack.speed = -10;
                Axis.Motor.DoVelocityMove(speedPack);
            }
            else if (IsVelocity[1])
            {
                speedPack.acc = 100;
                speedPack.dec = 100;
                speedPack.speed = -50;
                Axis.Motor.DoVelocityMove(speedPack);
            }
            else if (IsVelocity[2])
            {
                speedPack.acc = Axis.ACC;
                speedPack.dec = Axis.DEC;
                speedPack.speed = Axis.VEL;
                Axis.Motor.DoSCurveMove((-1) * (double)Pitch, speedPack, UnitMotor.EnumMovePosType.INCREMENTAL);
            }
        }

        private void MouseUpMinusCommand()
        {
            if (IsVelocity[0] || IsVelocity[1])
            {
                Axis.Motor.StopMove();
            }
            Console.WriteLine("DDDDD");
        }

        private void FirstMoveCommand()
        {
            if (!Axis.Motor.IsServoOn)
            {
                Global.MessageOpen(enMessageType.OK, "Please servo on.");
                return;
            }

            if(Axis.Alarm)
            {
                Global.MessageOpen(enMessageType.OK, "Axis Servo Alarm.");
                return;
            }

            speedPack.acc = Axis.ACC;
            speedPack.dec = Axis.DEC;
            speedPack.speed = Axis.VEL;
            speedPack.timeout = 10000;
            Axis.Motor.DoSCurveMove(Axis.ManualFirstTeachingPosition, speedPack, UnitMotor.EnumMovePosType.ABSOLUTE);
        }
        private void SecondMoveCommand()
        {
            if (!Axis.Motor.IsServoOn)
            {
                Global.MessageOpen(enMessageType.OK, "Please servo on.");
                return;
            }
            if (Axis.Alarm)
            {
                Global.MessageOpen(enMessageType.OK, "Axis Servo Alarm.");
                return;
            }
            speedPack.acc = Axis.ACC;
            speedPack.dec = Axis.DEC;
            speedPack.speed = Axis.VEL;
            speedPack.timeout = 10000;
            Axis.Motor.DoSCurveMove(Axis.ManualSecondTeachingPosition, speedPack, UnitMotor.EnumMovePosType.ABSOLUTE);
        }
        private void FirstTeachingCommand()
        {
            Axis.ManualFirstTeachingPosition = Axis.ActualPosition;
        }
        private void SecondTeachingCommand()
        {
            Axis.ManualSecondTeachingPosition = Axis.ActualPosition;
        }
        private void PitchClickCommand()
        {
            Pitch = Global.KeyPad(Pitch);   
        }
        private void RepeatCommand()
        {
            if (Axis == null) return;
            if (!Axis.Motor.IsServoOn) return;
            if (Axis.Motor.IsAlarm) return;
            Axis.IsRepeatMode = true;
            Task.Run(() => DoWork());
        }
        private async Task DoWork()
        {
            int index = 0;
            await Task.Run(() =>
            {
                while(Axis.IsRepeatMode)
                {
                    if (Axis == null) break;
                    if (!Axis.Motor.IsServoOn) break;
                    if (Axis.Motor.IsAlarm) break;

                    speedPack.acc = Axis.ACC;
                    speedPack.dec = Axis.DEC;
                    speedPack.speed = Axis.VEL;
                    speedPack.timeout = 10000;

                    if (index == 0)
                    {
                        Axis.Motor.DoSCurveMove(Axis.ManualFirstTeachingPosition, speedPack, UnitMotor.EnumMovePosType.ABSOLUTE);
                        while (Axis.IsRepeatMode)
                        {
                            if (Axis.InPosition && !Axis.Motor.IsMoving && !Axis.Motor.IsAlarm) break;
                            Thread.Sleep(1);
                        }
                    }
                    else if (index == 1)
                    {
                        Axis.Motor.DoSCurveMove(Axis.ManualSecondTeachingPosition, speedPack, UnitMotor.EnumMovePosType.ABSOLUTE);
                        while (Axis.IsRepeatMode)
                        {
                            if (Axis.InPosition && !Axis.Motor.IsMoving && !Axis.Motor.IsAlarm) break;
                            Thread.Sleep(1);
                        }
                    }
                    Thread.Sleep(1);
                    index++;
                    if (index == 2) index = 0;
                }
            });
            Axis.IsRepeatMode = false;
        }
        public List<AxisInfoCls> AxisList
        {
            get { return list; }
            set { list = value; RaisePropertyChanged("AxisList"); }
        }

        public string AxisInfo
        {
            get { return AxisInfo_; }
            set { AxisInfo_ = value; RaisePropertyChanged("AxisInfo"); }
        }

        public void SetDisplay()
        {
            AxisList = Global.STAxis.FindAll(x => x.AxisNo == (int)Global.STMotorIODataMessage.AxisType).ToList();

            if (AxisList.Count != 0)
            {
                AxisInfo = string.Format("Axis Status ({0})", AxisList[0].AxisID);
                Axis = AxisList[0];
                Console.WriteLine(Axis.ServoState);
            }
            else AxisInfo = "Axis Status";
        }
        private void VelocityGridDoubleClickCommand(object o)
        {
            System.Windows.Controls.DataGrid grid = o as System.Windows.Controls.DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            switch(index)
            {
                case 0:
                    Axis.ACC = Convert.ToInt32(Global.KeyPad(Axis.ACC));
                    break;
                case 1:
                    Axis.DEC = Convert.ToInt32(Global.KeyPad(Axis.DEC));
                    break;
                case 2:
                    Axis.VEL = Convert.ToInt32(Global.KeyPad(Axis.VEL));
                    break;
            }
        }
    }
}
