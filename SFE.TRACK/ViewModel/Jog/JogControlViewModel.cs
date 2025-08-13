using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using CoreCSRunSim;
using CoreCSMac;
using SFE.TRACK.Model;
using System.Collections.ObjectModel;
using MachineDefine;

namespace SFE.TRACK.ViewModel.Jog
{
    public class JogControlViewModel : ViewModelBase
    {
        public RelayCommand MouseDownPlusRelayCommand { get; set; }
        public RelayCommand MouseUpPlusRelayCommand { get; set; }
        public RelayCommand MouseDownMinusRelayCommand { get; set; }
        public RelayCommand MouseUpMinusRelayCommand { get; set; }
        public RelayCommand<Window> OKRelayCommand { get; set; }
        public RelayCommand<Window> CancelRelayCommand { get; set; }
        public RelayCommand PitchClickRelayCommand { get; set; }
        bool[] isVelocity = new bool[3] { true, false, false };
        float pitchLen = 0.1f;
        TSpeedPack speedPack = new TSpeedPack();
        AxisInfoCls Axis_ { get; set; }
        string command = string.Empty;

        public JogControlViewModel()
        {
            MouseDownPlusRelayCommand = new RelayCommand(MouseDownPlusCommand);
            MouseUpPlusRelayCommand = new RelayCommand(MouseUpPlusCommand);
            MouseDownMinusRelayCommand = new RelayCommand(MouseDownMinusCommand);
            MouseUpMinusRelayCommand = new RelayCommand(MouseUpMinusCommand);
            OKRelayCommand = new RelayCommand<Window>(OKCommand);
            CancelRelayCommand = new RelayCommand<Window>(CancelCommand);
            PitchClickRelayCommand = new RelayCommand(PitchClickCommand);
            Messenger.Default.Register<TeachingDataMessageCls>(this, OnReceiveMessageAction);
        }
        public AxisInfoCls Axis
        {
            get { return Axis_; }
            set { Axis_ = value; RaisePropertyChanged("Axis"); }
        }
        public bool[] IsVelocity
        {
            get { return isVelocity; }
            set { isVelocity = value;
                RaisePropertyChanged("IsVelocity"); }
        }

        public float PitchLen
        {
            get { return pitchLen; }
            set { pitchLen = value; RaisePropertyChanged("PitchLen"); }
        }

        private void OnReceiveMessageAction(TeachingDataMessageCls o)
        {
            //여기서 상태를 받아 온다.
            this.Axis = o.Axis;
            Console.WriteLine(string.Format("AxisID = {0}, Actual Pos = {1}", Axis.AxisID, Axis.ActualPosition));
            if (Axis.AxisID.ToUpper().IndexOf("_PIN") != -1)
            {
                Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual__ChamberState, string.Format("Chamber:{0}:{1}", Axis.BlockNo, Axis.ModuleNo));
            }
            else
            {
                if (Axis.Company == "SFE_CAN")
                    Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.StatusChange__MotorAxisRequest, string.Format("MOTOR:{0}", Axis.AxisID));
            }
        }

        private void MouseDownPlusCommand()
        {
            if (IsVelocity[0])
            {
                if(Axis.AxisID.ToUpper().IndexOf("_PIN") != -1)
                {
                    command = string.Format("Chamber:{0}:{1}:{2}", Axis.BlockNo, Axis.ModuleNo, (int)enDirection.CW);
                    Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual__JogMove, command);
                    return;
                }

                speedPack.acc = 100;
                speedPack.dec = 100;
                speedPack.speed = 10;
                command = string.Format("Motor:{0},{1},{2}", Axis.AxisID, (int)enDirection.CW, (int)enJogMode.LOW);
                if (Axis.Company == "SFE_CAN") Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move__JogMove, command);
                else
                {
                    speedPack.acc = speedPack.acc * Global.STPulseToUnit;
                    speedPack.dec = speedPack.dec * Global.STPulseToUnit;
                    speedPack.speed = speedPack.speed * Global.STPulseToUnit;
                    Axis.Motor.DoVelocityMove(speedPack);
                }
            }
            else if (IsVelocity[1])
            {
                if (Axis.AxisID.ToUpper().IndexOf("_PIN") != -1)
                {
                    command = string.Format("Chamber:{0}:{1}:{2}", Axis.BlockNo, Axis.ModuleNo, (int)enDirection.CW);
                    Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual__JogMove, command);
                    return;
                }

                speedPack.acc = 100;
                speedPack.dec = 100;
                speedPack.speed = 20;
                command = string.Format("Motor:{0},{1},{2}", Axis.AxisID, (int)enDirection.CW, (int)enJogMode.HIGH);
                if (Axis.Company == "SFE_CAN") Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move__JogMove, command);
                else
                {
                    speedPack.acc = speedPack.acc * Global.STPulseToUnit;
                    speedPack.dec = speedPack.dec * Global.STPulseToUnit;
                    speedPack.speed = speedPack.speed * Global.STPulseToUnit;
                    Axis.Motor.DoVelocityMove(speedPack);
                }
            }
            else if (IsVelocity[2])
            {
                if (Axis.AxisID.ToUpper().IndexOf("_PIN") != -1)
                {
                    command = string.Format("Chamber:{0}:{1}:{2}", Axis.BlockNo, Axis.ModuleNo, PitchLen);
                    Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual__PitchMove, command);
                    return;
                }

                speedPack.acc = speedPack.acc.Equals(0) ? 100 : Axis.ACC;
                speedPack.dec = speedPack.dec.Equals(0) ? 100 : Axis.DEC;
                speedPack.speed = speedPack.speed.Equals(0) ? 20 : Axis.VEL;
                //command = string.Format("Motor:{0},{1},{2},{3},{4},{5}", Axis.AxisID, PitchLen, speedPack.speed, speedPack.acc, speedPack.dec, 10000);
                command = string.Format("Motor:{0},{1},{2}", Axis.AxisID, (int)enDirection.CW, PitchLen);
                if (Axis.Company == "SFE_CAN") Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move__PitchMove, command);
                else
                {
                    speedPack.acc = speedPack.acc * Global.STPulseToUnit;
                    speedPack.dec = speedPack.dec * Global.STPulseToUnit;
                    speedPack.speed = speedPack.speed * Global.STPulseToUnit;
                    Axis.Motor.DoSCurveMove((double)PitchLen * Global.STPulseToUnit, speedPack, UnitMotor.EnumMovePosType.INCREMENTAL);
                }
            }
        }

        private void MouseUpPlusCommand()
        {
            if (IsVelocity[0] || IsVelocity[1])
            {
                if (Axis.AxisID.ToUpper().IndexOf("_PIN") != -1)
                {
                    command = string.Format("Chamber:{0}:{1}", Axis.BlockNo, Axis.ModuleNo);
                    Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual__MoveStop, command);
                    return;
                }

                command = string.Format("Motor:{0}", Axis.AxisID);
                if (Axis.Company == "SFE_CAN") Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move__Stop, command);
                else Axis.Motor.StopMove();
            }
        }

        private void MouseDownMinusCommand()
        {
            if (IsVelocity[0])
            {
                if (Axis.AxisID.ToUpper().IndexOf("_PIN") != -1)
                {
                    command = string.Format("Chamber:{0}:{1}:{2}", Axis.BlockNo, Axis.ModuleNo, (int)enDirection.CCW);
                    Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual__JogMove, command);
                    return;
                }

                speedPack.acc = 100;
                speedPack.dec = 100;
                speedPack.speed = -10;
                command = string.Format("Motor:{0},{1},{2}", Axis.AxisID, (int)enDirection.CCW, (int)enJogMode.LOW);
                if (Axis.Company == "SFE_CAN") Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move__JogMove, command);
                else
                {
                    speedPack.acc = speedPack.acc * Global.STPulseToUnit;
                    speedPack.dec = speedPack.dec * Global.STPulseToUnit;
                    speedPack.speed = speedPack.speed * Global.STPulseToUnit;
                    Axis.Motor.DoVelocityMove(speedPack);
                }
            }
            else if (IsVelocity[1])
            {
                if (Axis.AxisID.ToUpper().IndexOf("_PIN") != -1)
                {
                    command = string.Format("Chamber:{0}:{1}:{2}", Axis.BlockNo, Axis.ModuleNo, (int)enDirection.CCW);
                    Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual__JogMove, command);
                    return;
                }

                speedPack.acc = 100;
                speedPack.dec = 100;
                speedPack.speed = -20;
                command = string.Format("Motor:{0},{1},{2}", Axis.AxisID, (int)enDirection.CCW, (int)enJogMode.HIGH);
                if (Axis.Company == "SFE_CAN") Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move__JogMove, command);
                else
                {
                    speedPack.acc = speedPack.acc * Global.STPulseToUnit;
                    speedPack.dec = speedPack.dec * Global.STPulseToUnit;
                    speedPack.speed = speedPack.speed * Global.STPulseToUnit;
                    Axis.Motor.DoVelocityMove(speedPack);
                }
            }
            else if (IsVelocity[2])
            {
                if (Axis.AxisID.ToUpper().IndexOf("_PIN") != -1)
                {
                    command = string.Format("Chamber:{0}:{1}:{2}", Axis.BlockNo, Axis.ModuleNo, (-1) * PitchLen);
                    Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual__PitchMove, command);
                    return;
                }


                speedPack.acc = speedPack.acc.Equals(0) ? 100 : Axis.ACC;
                speedPack.dec = speedPack.dec.Equals(0) ? 100 : Axis.DEC;
                speedPack.speed = speedPack.speed.Equals(0) ? -20 : Axis.VEL;
                //command = string.Format("Motor:{0},-{1},{2},{3},{4},{5}", Axis.AxisID, PitchLen, speedPack.speed, speedPack.acc, speedPack.dec, 10000);
                command = string.Format("Motor:{0},{1},{2}", Axis.AxisID, (int)enDirection.CCW, PitchLen);
                if (Axis.Company == "SFE_CAN") Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move__PitchMove, command);
                else
                {
                    speedPack.acc = speedPack.acc * Global.STPulseToUnit;
                    speedPack.dec = speedPack.dec * Global.STPulseToUnit;
                    speedPack.speed = speedPack.speed * Global.STPulseToUnit;
                    Axis.Motor.DoSCurveMove((-1) * (double)PitchLen * Global.STPulseToUnit, speedPack, UnitMotor.EnumMovePosType.INCREMENTAL);
                }
            }
        }

        private void MouseUpMinusCommand()
        {
            if (IsVelocity[0] || IsVelocity[1])
            {
                if (Axis.AxisID.ToUpper().IndexOf("_PIN") != -1)
                {
                    command = string.Format("Chamber:{0}:{1}", Axis.BlockNo, Axis.ModuleNo);
                    Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual__MoveStop, command);
                    return;
                }

                command = string.Format("Motor:{0}", Axis.AxisID);
                if (Axis.Company == "SFE_CAN") Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move__Stop, command);
                else Axis.Motor.StopMove();
            }
        }

        private void OKCommand(Window window)
        {
            //현 위치 값.
            Global.STTeachingMessage.Position = Axis.ActualPosition;
            window.DialogResult = true;
        }

        private void CancelCommand(Window window)
        {
            window.DialogResult = false;
        }

        private void PitchClickCommand()
        {
            PitchLen = Global.KeyPad(PitchLen);
        }
    }
}
