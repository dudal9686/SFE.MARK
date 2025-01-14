﻿using System;
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
        public AxisInfoCls Axis { get; set; }
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
            this.Axis = o.Axis;
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
                command = string.Format("Motor:{0},{1},{2},{3},{4},{5}", Axis.AxisID, PitchLen, speedPack.speed, speedPack.acc, speedPack.dec, 10000);
                MotorInfo motorInfo = (MotorInfo)Axis.Motor.MyNameInfo;
                if (motorInfo.TrdParty == "SFE_CAN") Global.MachineWorker.SendCommand(55, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___RelativeMove, command);
                else Axis.Motor.DoSCurveMove((double)PitchLen, speedPack, UnitMotor.EnumMovePosType.INCREMENTAL);
            }
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
            MotorInfo motorInfo = (MotorInfo)Axis.Motor.MyNameInfo;

            if (IsVelocity[0])
            {
                speedPack.acc = 100;
                speedPack.dec = 100;
                speedPack.speed = -10;
                command = string.Format("Motor:");
                if (motorInfo.TrdParty == "SFE_CAN") Global.MachineWorker.SendCommand(55, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___JogMove, command);
                else Axis.Motor.DoVelocityMove(speedPack);
            }
            else if (IsVelocity[1])
            {
                speedPack.acc = 100;
                speedPack.dec = 100;
                speedPack.speed = -50;
                command = string.Format("");
                if (motorInfo.TrdParty == "SFE_CAN") Global.MachineWorker.SendCommand(55, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___JogMove, command);
                Axis.Motor.DoVelocityMove(speedPack);
            }
            else if (IsVelocity[2])
            {
                speedPack.acc = Axis.ACC;
                speedPack.dec = Axis.DEC;
                speedPack.speed = Axis.VEL;
                command = string.Format("Motor:{0},-{1},{2},{3},{4},{5}", Axis.AxisID, PitchLen, speedPack.speed, speedPack.acc, speedPack.dec, 10000);
                
                if (motorInfo.TrdParty == "SFE_CAN") Global.MachineWorker.SendCommand(55, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___RelativeMove, command);
                else Axis.Motor.DoSCurveMove((-1) * (double)PitchLen, speedPack, UnitMotor.EnumMovePosType.INCREMENTAL);
            }
            Console.WriteLine("CCCCC");
        }

        private void MouseUpMinusCommand()
        {
            if (IsVelocity[0] || IsVelocity[1])
            {
                Axis.Motor.StopMove();
            }
            Console.WriteLine("DDDDD");
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
