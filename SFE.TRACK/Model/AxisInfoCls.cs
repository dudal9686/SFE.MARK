using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Media;
using CoreCSRunSim;
using MachineDefine;
using SFE.TRACK.ViewModel;
using MachineDefine;
using System.Threading;
using System.Windows.Threading;

namespace SFE.TRACK.Model
{
    public class AxisInfoCls : ViewModelBase
    {
        string axisID = string.Empty;
        int axisNo = 0;
        double actualPosition = 10.0f;
        double commandPosition = 0.0f;
        double manualFirstTeachingPosition = 0;
        double manualSecondTeachingPosition = 0;
        string parent = string.Empty;
        int blockNo = 2;
        int moduleNo = 0;
        string moduleName = string.Empty;
        bool isHomeChecked = false; //UI에서 홈 선택시 바인딩
        
        bool servo = false;
        bool plusLimit = false;
        bool minusLimit = false;
        bool plusHomeLimit = false;
        bool minusHomeLimit = false;
        bool alarm = false;
        bool inMotion = false;
        bool inPosition = false;
        bool isHome = false;
        bool isStop = true;
        public bool IsRepeatMode = false;
        enHomeState homeSituation = enHomeState.HOME_NONE;

        int acc = 1000;
        int dec = 1000;
        int vel = 150;

        bool isOwn = false;
        string command = string.Empty;
        DispatcherTimer timer = new DispatcherTimer();

        public UnitMotor Motor { get; set; }

        SolidColorBrush servoState = new SolidColorBrush();
        SolidColorBrush alarmState = new SolidColorBrush();
        SolidColorBrush plusLimitState = new SolidColorBrush();
        SolidColorBrush minusLimitState = new SolidColorBrush();
        SolidColorBrush inPositionState = new SolidColorBrush();
        SolidColorBrush inMotionState = new SolidColorBrush();
        SolidColorBrush plusHomeLimitState = new SolidColorBrush();
        SolidColorBrush minusHomeLimitState = new SolidColorBrush();
        SolidColorBrush homeState = new SolidColorBrush();
        SolidColorBrush isHomeState = new SolidColorBrush();

        public RelayCommand HomeRelayCommand { get; set; }
        public RelayCommand ServoRelayCommand { get; set; }
        public RelayCommand ServoOffRelayCommand { get; set; }
        public RelayCommand EncoderClearRelayCommand { get; set; }
        public RelayCommand StopRelayCommand { get; set; }
        public RelayCommand AlarmResetRelayCommand { get; set; }
        string company = string.Empty;
        public AxisInfoCls()
        {
            servoState = Brushes.GreenYellow;
            alarmState = Brushes.White;
            plusLimitState = Brushes.White;
            minusLimitState = Brushes.Red;
            plusHomeLimitState = Brushes.White;
            minusHomeLimitState = Brushes.White;
            inPositionState = Brushes.GreenYellow;
            inMotionState = Brushes.White;
            homeState = Brushes.DarkGray;
            isHomeState = Brushes.Red;

            HomeRelayCommand = new RelayCommand(HomeCommand);
            ServoRelayCommand = new RelayCommand(ServoCommand);
            ServoOffRelayCommand = new RelayCommand(ServoOffCommand);
            EncoderClearRelayCommand = new RelayCommand(EncoderClearCommand);
            StopRelayCommand = new RelayCommand(StopCommand);
            AlarmResetRelayCommand = new RelayCommand(AlarmResetCommand);

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            PlusLimit = Motor.IsPositiveLimit;
            MinusLimit = Motor.IsNegativeLimit;
            Servo = Motor.IsServoOn;
            InMotion = Motor.IsMoving;
            //IsStop = !Motor.IsMoving;
            Alarm = Motor.IsAlarm;
            //IsHome = Motor.IsHome;
            ActualPosition = Motor.GetEncoderPos();
            CommandPosition = Motor.CommandPosition;
        }

        ~AxisInfoCls()
        {

        }

        public string Company
        {
            get { return company; }
            set
            {
                company = value;
                if(company.ToUpper() == "AZINECAT") timer.Start();
            }
        }

        public bool IsOwn
        {
            get { return isOwn; }
            set { isOwn = value; RaisePropertyChanged("IsOwn"); }
        }

        public string AxisID
        {
            get { return axisID; }
            set { axisID = value; RaisePropertyChanged("AxisID"); }
        }

        public int AxisNo
        {
            get { return axisNo; }
            set { axisNo = value; RaisePropertyChanged("AxisNo"); }
        }

        public bool Servo
        {
            get { return servo; }
            set
            {
                servo = value;
                if (servo) ServoState = Brushes.GreenYellow;
                else ServoState = Brushes.Red;
                RaisePropertyChanged("Servo");
            }
        }

        public bool IsStop
        {
            get { return isStop; }
            set { isStop = value; RaisePropertyChanged("IsStop"); }
        }

        public bool IsHome
        {
            get { return isHome; }
            set { isHome = value;
                if (isHome) IsHomeState = Brushes.GreenYellow;
                else IsHomeState = Brushes.Red;
                RaisePropertyChanged("IsHome"); }
        }

        public enHomeState HomeSituation
        {
            get { return homeSituation; }
            set { homeSituation = value; 
                if(homeSituation == enHomeState.HOME_NONE) { IsHome = false; HomeState = Brushes.DarkGray; IsHomeState = Brushes.Red; }
                else if (homeSituation == enHomeState.HOME_OK) { IsHome = true; HomeState = Brushes.GreenYellow; IsHomeState = Brushes.GreenYellow; }
                else if (homeSituation == enHomeState.HOMMING) { IsHome = false; HomeState = Brushes.Yellow; IsHomeState = Brushes.Red; }
                else if (homeSituation == enHomeState.HOME_ERROR) { IsHome = false; HomeState = Brushes.Red; }
                RaisePropertyChanged("HomeSituation"); }
        }

        //홈이 진행 됐다 안 됐다.
        public SolidColorBrush IsHomeState
        {
            get { return isHomeState; }
            set { isHomeState = value; RaisePropertyChanged("IsHomeState"); }
        }

        //메인화면에 홈 진행 상태를 보여주기 위한 색
        public SolidColorBrush HomeState
        {
            get { return homeState; }
            set { homeState = value; RaisePropertyChanged("HomeState"); }
        }

        public bool IsHomeChecked //UI 에서 홈 선택시 바인딩
        {
            get { return isHomeChecked; }
            set { isHomeChecked = value; RaisePropertyChanged("IsHomeChecked"); }
        }

        public SolidColorBrush ServoState
        {
            get { return servoState; }
            set { servoState = value; RaisePropertyChanged("ServoState"); }
        }

        public SolidColorBrush AlarmState
        {
            get { return alarmState; }
            set { alarmState = value; RaisePropertyChanged("AlarmState"); }
        }

        public bool Alarm
        {
            get { return alarm; }
            set
            {
                alarm = value;
                if (alarm) AlarmState = Brushes.Red;
                else AlarmState = Brushes.White;
                RaisePropertyChanged("Alarm");
            }
        }

        public SolidColorBrush PlusLimitState
        {
            get { return plusLimitState; }
            set { plusLimitState = value; RaisePropertyChanged("PlusLimitState"); }
        }

        public bool PlusLimit
        {
            get { return plusLimit; }
            set
            {
                plusLimit = value;
                if (PlusLimit) PlusLimitState = Brushes.Red;
                else PlusLimitState = Brushes.White;
                RaisePropertyChanged("PlusLimit");
            }
        }

        public SolidColorBrush PlusHomeLimitState
        {
            get { return plusHomeLimitState; }
            set { plusHomeLimitState = value; RaisePropertyChanged("PlusLimitState"); }
        }

        public bool PlusHomeLimit
        {
            get { return plusHomeLimit; }
            set
            {
                plusHomeLimit = value;
                if (PlusHomeLimit) PlusHomeLimitState = Brushes.Red;
                else PlusHomeLimitState = Brushes.White;
                RaisePropertyChanged("PlusHomeLimit");
            }
        }

        public SolidColorBrush MinusHomeLimitState
        {
            get { return minusHomeLimitState; }
            set { minusHomeLimitState = value; RaisePropertyChanged("MinusHomeLimitState"); }
        }

        public bool MinusHomeLimit
        {
            get { return minusHomeLimit; }
            set
            {
                minusHomeLimit = value;
                if (MinusHomeLimit) MinusHomeLimitState = Brushes.Red;
                else MinusHomeLimitState = Brushes.White;
                RaisePropertyChanged("MinusHomeLimit");
            }
        }


        public SolidColorBrush MinusLimitState
        {
            get { return minusLimitState; }
            set { minusLimitState = value; RaisePropertyChanged("MinusLimitState"); }
        }

        public bool MinusLimit
        {
            get { return minusLimit; }
            set
            {
                minusLimit = value;
                if (minusLimit) MinusLimitState = Brushes.Red;
                else MinusLimitState = Brushes.White;
                RaisePropertyChanged("MinusLimit");
            }
        }

        public SolidColorBrush InPositionState
        {
            get { return inPositionState; }
            set { inPositionState = value; RaisePropertyChanged("InPositionState"); }
        }

        public bool InPosition
        {
            get { return inPosition; }
            set
            {
                inPosition = value;
                if (inPosition) InPositionState = Brushes.GreenYellow;
                else InPositionState = Brushes.White;
                RaisePropertyChanged("InPosition");
            }
        }


        public SolidColorBrush InMotionState
        {
            get { return inMotionState; }
            set { inMotionState = value; RaisePropertyChanged("InMotionState"); }
        }

        public bool InMotion
        {
            get { return inMotion; }
            set
            {
                inMotion = value;
                if (InMotion) InMotionState = Brushes.GreenYellow;
                else InMotionState = Brushes.White;
                RaisePropertyChanged("InMotion");
            }
        }

        public double ActualPosition
        {
            get { return actualPosition; }
            set { actualPosition = value; RaisePropertyChanged("ActualPosition"); }
        }

        public double CommandPosition
        {
            get { return commandPosition; }
            set { commandPosition = value; RaisePropertyChanged("CommandPosition"); }
        }

        public double ManualFirstTeachingPosition
        {
            get { return manualFirstTeachingPosition; }
            set { manualFirstTeachingPosition = value; RaisePropertyChanged("ManualFirstTeachingPosition"); }
        }

        public double ManualSecondTeachingPosition
        {
            get { return manualSecondTeachingPosition; }
            set { manualSecondTeachingPosition = value; RaisePropertyChanged("ManualSecondTeachingPosition"); }
        }

        public int ACC
        {
            get { return acc; }
            set { acc = value; RaisePropertyChanged("ACC"); }
        }

        public int DEC
        {
            get { return dec; }
            set { dec = value; RaisePropertyChanged("DEC"); }
        }

        public int VEL
        {
            get { return vel; }
            set { vel = value; RaisePropertyChanged("VEL"); }
        }

        public string Parent
        {
            get { return parent; }
            set { parent = value; RaisePropertyChanged("Parent"); }
        }

        public int BlockNo
        {
            get { return blockNo; }
            set { blockNo = value; }
        }

        public int ModuleNo
        {
            get { return moduleNo; }
            set { moduleNo = value; }
        }

        public string ModuleName
        {
            get { return Global.GetModule(BlockNo, ModuleNo).MachineTitle; }
        }
        private void HomeCommand()
        {
            if (!GetRepeatMode()) return;

            if (!Global.MessageOpen(enMessageType.OKCANCEL, "Do You Want the <Home Search>?")) return;

            command = string.Format("Motor:{0}", AxisID);
            HomeSituation = enHomeState.HOME_NONE;
            if (Company == "SFE_CAN")
                Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___OriginMove, command);
            else Motor.DoHomming();
            HomeSituation = enHomeState.HOMMING;
        }

        private void ServoCommand()
        {
            if (!GetRepeatMode()) return;

            command = string.Format("Motor:{0},{1}", AxisID, 1);
            if (Company == "SFE_CAN")
                Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.StatusChange___ServoOn, command);
            else Motor.DoServoOn(true);
        }
        private void ServoOffCommand()
        {
            if (!GetRepeatMode()) return;

            Servo = false;
            command = string.Format("Motor:{0},{1}", AxisID, 0);
            if (Company == "SFE_CAN") Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.StatusChange___ServoOn, command);
            else Motor.DoServoOn(false);
        }
        private void EncoderClearCommand()
        {
            if (!GetRepeatMode()) return;

            if (!Global.MessageOpen(enMessageType.OKCANCEL, "Do you want the [Encoder Clear]?")) return;
            command = string.Format("Motor:{0}", AxisID);
            if (Company == "SFE_CAN") Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.StatusChange___EncoderClear, command);
            else Motor.SetZeroPosition();
        }
        private void StopCommand()
        {
            command = string.Format("Motor:{0}", AxisID);
            if (Company == "SFE_CAN") Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___Stop, command);
            else Motor.StopMove();
            if (IsRepeatMode) IsRepeatMode = false;
        }
        private void AlarmResetCommand()
        {
            command = string.Format("Motor:{0}", AxisID);
            if (Company == "SFE_CAN") Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.StatusChange___ServoAlarmClear, command);
            else Motor.ClerAlarm();
            Console.WriteLine("AlarmResetCommand");
            System.Threading.Thread.Sleep(300);
        }

        private bool GetRepeatMode()
        {
            if(IsRepeatMode)
            {
                Global.MessageOpen(enMessageType.OKCANCEL, "Repeat Mode.");
                return false;
            }

            return true;
        }
    }
}
