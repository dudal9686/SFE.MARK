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
using MachineDefine;

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
        string command = string.Empty;
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
            if (Axis == null) return;
            if (Axis.IsRepeatMode) return;
            if (IsVelocity[0])
            {
                speedPack.acc = 100;
                speedPack.dec = 100;
                speedPack.speed = 10;
                command = string.Format("Motor:{0},{1},{2}", Axis.AxisID, (int)enDirection.CW, (int)enJogMode.LOW);
                if (Axis.Company == "SFE_CAN") Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___JogMove, command);
                else Axis.Motor.DoVelocityMove(speedPack);
            }
            else if (IsVelocity[1])
            {
                speedPack.acc = 100;
                speedPack.dec = 100;
                speedPack.speed = 20;
                command = string.Format("Motor:{0},{1},{2}", Axis.AxisID, (int)enDirection.CW, (int)enJogMode.HIGH);
                if (Axis.Company == "SFE_CAN") Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___JogMove, command);
                else Axis.Motor.DoVelocityMove(speedPack);
            }
            else if (IsVelocity[2])
            {
                speedPack.acc = Axis.ACC.Equals(0) ? 100 : Axis.ACC;
                speedPack.dec = Axis.DEC.Equals(0) ? 100 : Axis.DEC;
                speedPack.speed = Axis.VEL.Equals(0) ? 10 : Axis.VEL;
                //command = string.Format("Motor:{0},{1},{2},{3},{4},{5}", Axis.AxisID, PitchLen, speedPack.speed, speedPack.acc, speedPack.dec, 10000);
                command = string.Format("Motor:{0},{1},{2}", Axis.AxisID, (int)enDirection.CW, Pitch);
                if (Axis.Company == "SFE_CAN") Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___PitchMove, command);
                else Axis.Motor.DoSCurveMove((double)Pitch, speedPack, UnitMotor.EnumMovePosType.INCREMENTAL);
            }
        }

        private void MouseUpPlusCommand()
        {
            if (Axis == null) return;
            if (Axis.IsRepeatMode) return;
            if (IsVelocity[0] || IsVelocity[1])
            {
                command = string.Format("Motor:{0}", Axis.AxisID);
                if (Axis.Company == "SFE_CAN") Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___Stop, command);
                else Axis.Motor.StopMove();
            }
        }

        private void MouseDownMinusCommand()
        {
            if (Axis == null) return;
            if (Axis.IsRepeatMode) return;
            if (IsVelocity[0])
            {
                speedPack.acc = 100;
                speedPack.dec = 100;
                speedPack.speed = -10;
                command = string.Format("Motor:{0},{1},{2}", Axis.AxisID, (int)enDirection.CCW, (int)enJogMode.LOW);
                if (Axis.Company == "SFE_CAN") Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___JogMove, command);
                else Axis.Motor.DoVelocityMove(speedPack);
            }
            else if (IsVelocity[1])
            {
                speedPack.acc = 100;
                speedPack.dec = 100;
                speedPack.speed = -20;
                command = string.Format("Motor:{0},{1},{2}", Axis.AxisID, (int)enDirection.CCW, (int)enJogMode.HIGH);
                if (Axis.Company == "SFE_CAN") Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___JogMove, command);
                Axis.Motor.DoVelocityMove(speedPack);
            }
            else if (IsVelocity[2])
            {
                speedPack.acc = Axis.ACC.Equals(0) ? 100 : Axis.ACC;
                speedPack.dec = Axis.DEC.Equals(0) ? 100 : Axis.DEC;
                speedPack.speed = Axis.VEL.Equals(0) ? -20 : Axis.VEL;
                //command = string.Format("Motor:{0},-{1},{2},{3},{4},{5}", Axis.AxisID, PitchLen, speedPack.speed, speedPack.acc, speedPack.dec, 10000);
                command = string.Format("Motor:{0},{1},{2}", Axis.AxisID, (int)enDirection.CCW, Pitch);
                if (Axis.Company == "SFE_CAN") Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___PitchMove, command);
                else Axis.Motor.DoSCurveMove((-1) * (double)Pitch, speedPack, UnitMotor.EnumMovePosType.INCREMENTAL);
            }
        }

        private void MouseUpMinusCommand()
        {
            if (Axis == null) return;
            if (Axis.IsRepeatMode) return;
            if (IsVelocity[0] || IsVelocity[1])
            {
                command = string.Format("Motor:{0}", Axis.AxisID);
                if (Axis.Company == "SFE_CAN") Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___Stop, command);
                else Axis.Motor.StopMove();
            }
        }

        private void FirstMoveCommand()
        {
            if (Axis == null) return;
            if (Axis.IsRepeatMode) return;
            if (!Axis.Servo)
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
            if(Axis.Company == "SFE_CAN")
            {
                command = string.Format("Motor:{0},{1},{2},{3},{4},{5}", Axis.AxisID, Axis.ManualFirstTeachingPosition, Axis.VEL, Axis.ACC, Axis.DEC, 10000);
                Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___DirectMove, command);
            }
            else Axis.Motor.DoSCurveMove(Axis.ManualFirstTeachingPosition, speedPack, UnitMotor.EnumMovePosType.ABSOLUTE);
        }
        private void SecondMoveCommand()
        {
            if (Axis == null) return;
            if (Axis.IsRepeatMode) return;
            if (!Axis.Servo)
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
            if (Axis.Company == "SFE_CAN")
            {
                command = string.Format("Motor:{0},{1},{2},{3},{4},{5}", Axis.AxisID, Axis.ManualSecondTeachingPosition, Axis.VEL, Axis.ACC, Axis.DEC, 10000);
                Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___DirectMove, command);
            }
            else Axis.Motor.DoSCurveMove(Axis.ManualSecondTeachingPosition, speedPack, UnitMotor.EnumMovePosType.ABSOLUTE);
        }
        private void FirstTeachingCommand()
        {
            if (Axis == null) return;
            if (Axis.IsRepeatMode) return;
            if (Axis.Company == "SFE_CAN")
            {
                command = string.Format("Motor:{0}", Axis.AxisID);
                Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___EncoderPos, command, true);
                Thread.Sleep(700);
            }
            Axis.ManualFirstTeachingPosition = Axis.ActualPosition;
        }
        private void SecondTeachingCommand()
        {
            if (Axis == null) return;
            if (Axis.IsRepeatMode) return;
            if (Axis.Company == "SFE_CAN")
            {
                command = string.Format("Motor:{0}", Axis.AxisID);
                Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___EncoderPos, command, true);
                Thread.Sleep(700);
            }
            Axis.ManualSecondTeachingPosition = Axis.ActualPosition;
        }
        private void PitchClickCommand()
        {
            Pitch = Global.KeyPad(Pitch);   
        }
        private void RepeatCommand()
        {
            if (Axis == null) return;
            if (Axis.IsRepeatMode) return;
            if (!Axis.Servo)
            {
                Global.MessageOpen(enMessageType.OK, "Please servo on.");
                return;
            }
            if (Axis.Alarm)
            {
                Global.MessageOpen(enMessageType.OK, "Axis Servo Alarm.");
                return;
            }
            if (Axis.IsRepeatMode) return;
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
                    if (!Axis.Servo) break;
                    if (Axis.Alarm) break;

                    speedPack.acc = Axis.ACC;
                    speedPack.dec = Axis.DEC;
                    speedPack.speed = Axis.VEL;
                    speedPack.timeout = 10000;

                    if (index == 0)
                    {
                        if (Axis.Company == "SFE_CAN")
                        {
                            command = string.Format("Motor:{0},{1},{2},{3},{4},{5}", Axis.AxisID, Axis.ManualFirstTeachingPosition, Axis.VEL, Axis.ACC, Axis.DEC, 10000);
                            Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___DirectMove, command);
                        }
                        else Axis.Motor.DoSCurveMove(Axis.ManualFirstTeachingPosition, speedPack, UnitMotor.EnumMovePosType.ABSOLUTE);

                        while (Axis.IsRepeatMode)
                        {
                            if (Axis.InPosition && !Axis.Motor.IsMoving && !Axis.Motor.IsAlarm) break;
                            Thread.Sleep(1);
                        }
                        Thread.Sleep(100);
                    }
                    else if (index == 1)
                    {
                        if (Axis.Company == "SFE_CAN")
                        {
                            command = string.Format("Motor:{0},{1},{2},{3},{4},{5}", Axis.AxisID, Axis.ManualSecondTeachingPosition, Axis.VEL, Axis.ACC, Axis.DEC, 10000);
                            Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___DirectMove, command);
                        }
                        else Axis.Motor.DoSCurveMove(Axis.ManualSecondTeachingPosition, speedPack, UnitMotor.EnumMovePosType.ABSOLUTE);
                        while (Axis.IsRepeatMode)
                        {
                            if (Axis.InPosition && !Axis.Motor.IsMoving && !Axis.Motor.IsAlarm) break;
                            Thread.Sleep(1);
                        }
                        Thread.Sleep(100);
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
            if(Axis != null)
                if (Axis.IsRepeatMode == true) return;

            AxisList = Global.STAxis.FindAll(x => x.AxisNo == (int)Global.STMotorIODataMessage.AxisType).ToList();

            if (AxisList.Count != 0)
            {
                AxisInfo = string.Format("Axis Status ({0})", AxisList[0].AxisID);
                Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.StatusChange___MotorAxisRequest, string.Format("MOTOR:{0}", AxisList[0].AxisID));
                Axis = AxisList[0];
                Console.WriteLine(Axis.ServoState);
            }
            else AxisInfo = "Axis Status";
        }
        private void VelocityGridDoubleClickCommand(object o)
        {
            if (Axis == null) return;
            if (Axis.IsRepeatMode) return;
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
