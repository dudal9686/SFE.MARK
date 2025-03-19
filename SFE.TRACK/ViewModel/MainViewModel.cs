using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System;
using SFE.TRACK.Model;
using System.Collections.Generic;
using CoreCSBase;
using CoreCSBase.IPC;
using CoreCSMac;
using CoreCSRunSim;
using MachineDefine;
using System.Windows.Threading;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using DefaultBase;

namespace SFE.TRACK.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public RelayCommand ShutDownRelayCommand { get; set; }
        public RelayCommand LoginRelayCommand { get; set; }
        public RelayCommand LanguageRelayCommand { get; set; }
        ObservableCollection<bool> isEnabledMenu { get; set; }
        ObservableCollection<bool> isSelectedMenu { get; set; }
        List<LMBase> LmList = null;
        List<AssyBase> AssyList = null;
        List<UnitIO> DIList = null;
        List<UnitIO> DOList = null;
        List<UnitAIO> AIOList = null;
        List<UnitMotor> MotorList = null;
        List<UnitCustom> CustomUnitList = null;

        System.Windows.Media.SolidColorBrush TitleColor_ = (SolidColorBrush)new BrushConverter().ConvertFrom("#00004f");
        MachineReaderWorker _worker;
        DispatcherTimer timer = new DispatcherTimer();
        public bool IsMotorState = false;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            SFE.TRACK.Language.Localization.Initialize();
            ShutDownRelayCommand = new RelayCommand(ShutDownCommand);
            LoginRelayCommand = new RelayCommand(LoginCommand);
            LanguageRelayCommand = new RelayCommand(LanguageCommand);
            isEnabledMenu = new ObservableCollection<bool>();
            isSelectedMenu = new ObservableCollection<bool>();
            for (int i = 0; i < 10; i++) { IsEnabledMenu.Add(true); IsSelectedMenu.Add(true); }
            _worker = new MachineReaderWorker();
            _worker.EvtCommandLaunched += _worker_EvtCommandLaunched;
            _worker.EvtCommandStatusChanged += _worker_EvtCommandStatusChanged;
            _worker.EvtCommandResultComes += _worker_EvtCommandResultComes;
            _worker.EvtInfoReloaded += _worker_EvtInfoReloaded;
            _worker.EvtPrgCfgReloaded += _worker_EvtPrgCfgReloaded;
           
            Global.MachineWorker = _worker;
            if (_worker.StartWorker(Global.MMI_ID, "HMI", @"C:\MachineSet\SFETrack.mm", true) == false)
            {
                MessageBox.Show("Starting worker failed");
                return;
            }

            _worker.Controller.EvtAlarmOccured += Controller_EvtAlarmOccured;

            InitData();
            InitJobProcess();
            InitIO();

            Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.StatusChange___MotorDoRequest, string.Format("Motor:FALSE"));
            Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.StatusChange___IODoRequest, string.Format("IO:FALSE"));

            PrgCfgItem prgItem = _worker.Reader.GetConfigItem(EnumConfigGroup.Environment, EnumConfig_Environment.RecipeTransperInfo);
            prgItem.GetValue(out string recipename);
            Properties.Settings.Default.RECIPE_TRANSFER = recipename;
            Properties.Settings.Default.Save();

            prgItem = _worker.Reader.GetConfigItem(EnumConfigGroup.Environment, EnumConfig_Environment.DummyLinkRecipeInfo);
            prgItem.GetValue(out string dummyLinkRecipeName);
            Properties.Settings.Default.DUMMY_COND = dummyLinkRecipeName;
            Properties.Settings.Default.Save();

            timer.Tick += Timer_Tick; ;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();

            //FoupCls foup_ = Global.STModuleList.Find(x => x.ModuleType == enModuleType.FOUP && x.ModuleNo == 1) as FoupCls;
            //foup_.IsDetect = true;
            //foup_.IsScan = true;

            //foup_ = Global.STModuleList.Find(x => x.ModuleType == enModuleType.FOUP && x.ModuleNo == 2) as FoupCls;
            //foup_.IsDetect = true;
            //foup_.IsScan = true;

            //SpinChamberCls spinUnit = Global.STModuleList.Find(x => x.MachineName == "COT") as SpinChamberCls;
            //spinUnit.ModuleState = enModuleState.PREPROCESS;
            //spinUnit.Wafer = foup_.FoupWaferList[0].Clone();
            //spinUnit.Wafer.WaferState = enWaferState.WAFER_PROCESS;
            //foup_.FoupWaferList[0].WaferState = enWaferState.WAFER_EMPTY;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(Global.STMachineStatus == enMachineStatus.RUN)
            {
                IsEnabledMenu[(int)enMainMenu.MAINT] = false;
                IsEnabledMenu[(int)enMainMenu.MOTOR] = false;
                IsEnabledMenu[(int)enMainMenu.PARAMETER] = false;
                IsEnabledMenu[(int)enMainMenu.MOTIONMOVING] = false;
                IsEnabledMenu[(int)enMainMenu.MAINT] = false;
            }
            else if(Global.STMachineStatus == enMachineStatus.STOP)
            {
                IsEnabledMenu[(int)enMainMenu.MAINT] = true;
                IsEnabledMenu[(int)enMainMenu.MOTOR] = true;
                IsEnabledMenu[(int)enMainMenu.PARAMETER] = true;
                IsEnabledMenu[(int)enMainMenu.MOTIONMOVING] = true;
                IsEnabledMenu[(int)enMainMenu.MAINT] = true;
            }
        }

        #region MainMenu
        public ObservableCollection<bool> IsEnabledMenu
        {
            get { return isEnabledMenu; }
            set { isEnabledMenu = value; RaisePropertyChanged("IsEnabledMenu"); }
        }
        public ObservableCollection<bool> IsSelectedMenu
        {
            get { return isSelectedMenu; }
            set { isSelectedMenu = value; RaisePropertyChanged("IsSelectedMenu"); }
        }
        public System.Windows.Media.SolidColorBrush TitleColor
        {
            get { return TitleColor_; }
            set { TitleColor_ = value; RaisePropertyChanged("TitleColor"); }
        }
        #endregion

        private void InitJobProcess()
        {
            Global.STJobInfo.Clear();

            for (int i = 0; i < 10; i++) Global.STJobInfo.LotInfoList.Add(new LotInfoCls());
        }

        private bool InitData()
        {
            if (!Global.STDataAccess.ReadModuleData() /*|| !AxisCls.OpenDevice()*/)
            {
                MessageBox.Show("Unable to retrieve equipment information.");
                return false;
            }

            LmList = _worker.Controller.GetLM();
            AssyList = _worker.Controller.GetAssy();
            DIList = _worker.Controller.GetInput();
            DOList = _worker.Controller.GetOutput();
            AIOList = _worker.Controller.GetAIO();
            MotorList = _worker.Controller.GetMotor();
            CustomUnitList = _worker.Controller.GetCustom();

            SetRecipeFileList();
            SetAxisData();
            SetTeachingData();
            Global.STDataAccess.ReadMonitoringData();
            Global.STDataAccess.ReadUserInfo();
            Global.STDataAccess.ReadMaintSupportData();
            ReadBuzzerData();
            return true;
        }

        private void ReadBuzzerData()
        {
            Global.STDataAccess.ReadLampData();
            PrgCfgItem prgItem = _worker.Reader.GetConfigItem(EnumConfigGroup.Environment, EnumConfig_Environment.TowerLamp);
            GetTowerLamp(prgItem);
        }

        private void GetTowerLamp(PrgCfgItem prgItem)
        {
            if (Global.STLampList.Count == 0) return;

            //List<string> list = new List<string>();
            //prgItem.GetValue(list);

            int buzzerTimeOut = prgItem.GetInt(0);
            string packet = string.Empty;
            for(int i = 0; i < Global.STLampList.Count; i++)
            {
                packet = prgItem.GetString(i + 1);
                LampCls lamp = Global.STLampList[i];
                for(int j = 0; j < packet.Length; j++)
                {
                    switch(j)
                    {
                        case 0:
                            if (packet[0] == 'O') lamp.RedString = enLamp.ON.ToString();
                            else if (packet[0] == 'X') lamp.RedString = enLamp.OFF.ToString();
                            else if (packet[0] == 'T') lamp.RedString = enLamp.TOGGLE.ToString();
                            break;
                        case 1:
                            if (packet[1] == 'O') lamp.YellowString = enLamp.ON.ToString();
                            else if (packet[1] == 'X') lamp.YellowString = enLamp.OFF.ToString();
                            else if (packet[1] == 'T') lamp.YellowString = enLamp.TOGGLE.ToString();
                            break;
                        case 2:
                            if (packet[2] == 'O') lamp.GreenString = enLamp.ON.ToString();
                            else if (packet[2] == 'X') lamp.GreenString = enLamp.OFF.ToString();
                            else if (packet[2] == 'T') lamp.GreenString = enLamp.TOGGLE.ToString();
                            break;
                        case 3:
                            if (packet[3] == 'O') lamp.BuzzerString = enBuzzer.ON.ToString();
                            else if (packet[3] == 'X') lamp.BuzzerString = enBuzzer.OFF.ToString();
                            break;
                    }

                    lamp.BuzzerTimeOut = buzzerTimeOut;
                }
            }

        }
        private void SetAxisData()
        {
            foreach (UnitMotor motor in MotorList)
            {
                AxisInfoCls axis = new AxisInfoCls();
                axis.AxisID = motor.MyNameInfo.Name;
                axis.Motor = motor;
                MotorInfo motorInfo = (MotorInfo)motor.MyNameInfo;
                axis.Company = motorInfo.TrdParty;
                axis.AxisNo = Convert.ToInt32((enAxisType)Enum.Parse(typeof(enAxisType), motor.MyNameInfo.Name));

                string parentType = motor.GetParentIDentity().Substring(motor.GetParentIDentity().IndexOf(':') + 1, motor.GetParentIDentity().Length - motor.GetParentIDentity().IndexOf(':') - 1);
                axis.Parent = parentType;
                if (parentType == "CRA" || parentType == "PRA")
                {
                    axis.IsOwn = true;
                    axis.ModuleNo = 0;
                    if (parentType == "CRA") axis.BlockNo = 1;
                }
                else
                {
                    string[] arr = axis.Motor.MyNameInfo.Information.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    axis.ModuleNo = Convert.ToInt32(arr[arr.Length - 1]);
                }
                Global.STAxis.Add(axis);
            }
        }

        private void SetTeachingData()
        {
            foreach (AxisInfoCls axis in Global.STAxis)
            {
                string parentType = axis.Parent;

                MotorInfo motorInfo = (MotorInfo)axis.Motor.MyNameInfo;
                List<string> motorTeachingList = motorInfo.Teaching; //어레이가 있는지 아는 리스트 '(' 로 구분
                //List<Teaching> motorTechingArrayContainList = motor.GetTeachingPositionList();

                for (int i = 0; i < motorTeachingList.Count; i++)
                {
                    if (motorTeachingList[i].IndexOf("(") != -1) // PRA
                    {
                        SetTeachingArray(axis, motorTeachingList[i]);
                    }
                    else
                    {
                        TeachingDataCls teachingData = new TeachingDataCls();
                        teachingData.IsArray = false;
                        teachingData.TeachingName = motorTeachingList[i];
                        if (parentType == "PRA")
                        {
                            teachingData.MainTitle = "PRA";
                            teachingData.ModuleNo = 0;
                            teachingData.IsOwn = true;
                            //techingInfo
                        }
                        else if (parentType == "CRA")
                        {
                            teachingData.MainTitle = "CRA";
                            teachingData.BlockNo = 1;
                            if (motorTeachingList[i].IndexOf("CST1") != -1)
                                teachingData.ModuleNo = 1;
                            else if (motorTeachingList[i].IndexOf("CST2") != -1)
                                teachingData.ModuleNo = 2;
                            else if (motorTeachingList[i].IndexOf("CST3") != -1)
                                teachingData.ModuleNo = 3;
                            else if (motorTeachingList[i].IndexOf("CST4") != -1)
                                teachingData.ModuleNo = 4;
                            else teachingData.ModuleNo = 0;

                            teachingData.IsOwn = true;

                        }
                        else //ADH, Chamber, Coater, Developer
                        {
                            string[] arr = axis.Motor.MyNameInfo.Information.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            teachingData.ModuleNo = Convert.ToInt32(arr[arr.Length - 1]);
                            teachingData.MainTitle = Global.GetModule(2, teachingData.ModuleNo).MachineName;
                            teachingData.IsOwn = false;
                        }
                        teachingData.Motor = axis.Motor;
                        Global.STTeachingData.Add(teachingData);
                    }
                }
            }
        }

        private void SetTeachingArray(AxisInfoCls axis, string name)
        {
            List<ModuleBaseCls> list = Global.STModuleList.FindAll(x => x.ModuleType == enModuleType.CHAMBER || x.ModuleType == enModuleType.SPINCHAMBER);

            foreach (ModuleBaseCls module in list)
            {
                TeachingDataCls data = new TeachingDataCls();
                data.MainTitle = "CHAMBER";
                data.ModuleNo = module.ModuleNo;
                data.IsArray = true;
                data.TeachingName = name.Substring(0, name.IndexOf("("));
                data.Motor = axis.Motor;
                data.IsOwn = true;
                Global.STTeachingData.Add(data);
            }
        }

        private void InitIO()
        {
            GetDIData();
            GetDOData();
            GetAIData();
        }

        private void GetDIData()
        {
            string[] arr;
            int moduleNo = 0;
            ModuleBaseCls module;
            string parentType = string.Empty;
            IODataCls data;
            foreach (UnitIO io in DIList)
            {
                data = new IODataCls();
                arr = io.MyNameInfo.Information.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                data.IO = io;
                data.BlockNo = 2;
                data.IOType = enIOType.DI;
                IOInfo ioInfo = (IOInfo)io.MyNameInfo;
                data.Company = ioInfo.TrdParty;
                data.Alias = io.MyNameInfo.Alias;
                data.Enable = false;
                parentType = io.GetParentIDentity().Substring(io.GetParentIDentity().IndexOf(':') + 1, io.GetParentIDentity().Length - io.GetParentIDentity().IndexOf(':') - 1);

                if (parentType == "CRA" || parentType == "PRA")
                {
                    if (io.MyNameInfo.Alias == "CRA") data.BlockNo = 1;
                    data.ModuleNo = 1;
                    data.BoardNo = 0;// Convert.ToInt32(arr[arr.Length - 2]);
                    data.ChannelNo = Convert.ToInt32(arr[arr.Length - 2]);
                    data.IONum = data.ChannelNo * 32 + Convert.ToInt32(arr[arr.Length - 1]);
                }
                else
                {
                    moduleNo = Convert.ToInt32(arr[arr.Length - 1]);
                    module = Global.GetModule(data.BlockNo, moduleNo);
                    data.ModuleNo = moduleNo;
                    data.BoardType = arr[0];

                    if (data.BoardType == "SIO") data.IONum = ((Convert.ToInt32(arr[2]) - 128) * 64) + Convert.ToInt32(arr[3]);
                    else data.IONum = ((Convert.ToInt32(arr[2]) - 272) * 64) + Convert.ToInt32(arr[3]);

                    if (data.BoardType == "MINI") data.IOIndex = Convert.ToInt32(arr[3]);

                    if (parentType == "ChemicalBox") data.Alias = string.Format("{0}", parentType);
                    else data.Alias = string.Format("{0}[{1}-{2}]", module.MachineName, data.BlockNo, moduleNo);
                }

                Global.STDIList.Add(data);
            }
        }

        private void GetDOData()
        {
            string[] arr;
            int moduleNo = 0;
            ModuleBaseCls module;
            string parentType = string.Empty;
            IODataCls data;
            foreach (UnitIO io in DOList)
            {
                data = new IODataCls();
                arr = io.MyNameInfo.Information.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                data.IO = io;
                data.BlockNo = 2;
                data.IOType = enIOType.DO;
                IOInfo ioInfo = (IOInfo)io.MyNameInfo;
                data.Company = ioInfo.TrdParty;
                data.Alias = io.MyNameInfo.Alias;
                data.Enable = true;
                parentType = io.GetParentIDentity().Substring(io.GetParentIDentity().IndexOf(':') + 1, io.GetParentIDentity().Length - io.GetParentIDentity().IndexOf(':') - 1);

                if (parentType == "CRA" || parentType == "PRA")
                {
                    if (io.MyNameInfo.Alias == "CRA") data.BlockNo = 1;
                    data.ModuleNo = 1;
                    data.BoardNo = 0;//Convert.ToInt32(arr[arr.Length - 2]);
                    data.ChannelNo = Convert.ToInt32(arr[arr.Length - 2]);
                    data.IONum = data.ChannelNo * 32 + Convert.ToInt32(arr[arr.Length - 1]);
                }
                else
                {
                    moduleNo = Convert.ToInt32(arr[arr.Length - 1]);
                    module = Global.GetModule(data.BlockNo, moduleNo);
                    data.ModuleNo = moduleNo;
                    data.BoardType = arr[0];

                    if (data.BoardType == "SIO") data.IONum = ((Convert.ToInt32(arr[2]) - 128) * 64) + Convert.ToInt32(arr[3]);
                    else data.IONum = ((Convert.ToInt32(arr[2]) - 272) * 64) + Convert.ToInt32(arr[3]);

                    if (data.BoardType == "MINI") data.IOIndex = Convert.ToInt32(arr[3]);

                    if (parentType == "ChemicalBox") data.Alias = string.Format("{0}", parentType);
                    else data.Alias = string.Format("{0}[{1}-{2}]", module.MachineName, data.BlockNo, moduleNo);
                }

                Global.STDOList.Add(data);
            }
        }

        private void GetAIData()
        {
            string[] arr;
            int moduleNo = 0;
            ModuleBaseCls module;
            AIODataCls data;
            foreach (UnitAIO io in AIOList)
            {
                data = new AIODataCls();
                arr = io.MyNameInfo.Information.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                data.IO = io;
                data.BlockNo = 2;
                data.IOType = enIOType.AI;
                data.Alias = io.MyNameInfo.Alias;
                data.Enable = false;

                if (io.MyNameInfo.Alias == "CRA" || io.MyNameInfo.Alias == "PRA")
                {
                    if (io.MyNameInfo.Alias == "CRA") data.BlockNo = 1;
                    data.ModuleNo = 1;
                    data.IONum = Convert.ToInt32(arr[arr.Length - 1]);
                }
                else if (io.MyNameInfo.Alias == "PRA")
                {
                    data.ModuleNo = 1;
                    data.IONum = Convert.ToInt32(arr[arr.Length - 1]);
                }
                else
                {
                    moduleNo = Convert.ToInt32(arr[arr.Length - 1]);
                    module = Global.GetModule(data.BlockNo, moduleNo);
                    data.IONum = Convert.ToInt32(arr[arr.Length - 2]);
                    data.Alias = string.Format("{0}[{1}-{2}]", module.MachineName, data.BlockNo, moduleNo);
                }

                Global.STAIOList.Add(data);
            }
        }

        public void SetAlarm()
        {
            for (int i = 0; i < IsSelectedMenu.Count; i++)
            {
                if (i == (int)enMainMenu.ALARM) IsSelectedMenu[i] = true;
                else IsSelectedMenu[i] = false;
            }
            TitleColor = System.Windows.Media.Brushes.Red;
        }
        public void ClearAlarm(string code = "")
        {
            if (Global.STAlarmList.Count == 0)
            {
                TitleColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#00004f");
                if (Global.STWarningList.Count != 0) TitleColor = System.Windows.Media.Brushes.Khaki;
            }
        }
        public void SetWarning()
        {
            if (Global.STAlarmList.Count == 0) TitleColor = System.Windows.Media.Brushes.Khaki;
        }
        public void ClearWarning()
        {
            if (Global.STWarningList.Count == 0)
            {
                TitleColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#00004f");
                if (Global.STAlarmList.Count != 0) TitleColor = System.Windows.Media.Brushes.Red;
            }
        }
        private void SetRecipeFileList()
        {
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\WaferFlow\", ref Global.WaferFlowRecipeFileList);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Dummy\Cond\ADH\", ref Global.ADHDummyCondRecipeFileList);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Dummy\Seq\ADH\", ref Global.ADHDummySeqRecipeFileList);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Process\ADH\", ref Global.ADHProcessRecipeFileList);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Clean\Cond\", ref Global.CleanCondRecipeFileList);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Clean\COT\", ref Global.CoaterCleanRecipeFileList);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Dummy\Cond\COT\", ref Global.CoaterDummyCondRecipeFileList);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Dummy\Seq\COT\", ref Global.CoaterDummySeqRecipeFileList);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Process\COT\", ref Global.CoaterProcessRecipeFileList);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Process\CPL\", ref Global.CPLProcessRecipeFileList);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Clean\DEV\", ref Global.DevCleanRecipeFileList);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Dummy\Cond\DEV\", ref Global.DevDummyCondRecipeFileList);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Dummy\Seq\DEV\", ref Global.DevDummySeqRecipeFileList);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Process\DEV\", ref Global.DevProcessRecipeFileList);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Dummy\Link\", ref Global.DummyCondLinkRecipeFileList);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Process\HHP\", ref Global.HHPProcessRecipeFileList);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Process\LHP\", ref Global.LHPProcessRecipeFileList);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Pump\", ref Global.PumpRecipeFileList);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\System\", ref Global.SystemRecipeFileList);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Process\TCP\", ref Global.TCPProcessRecipeFileList);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\JobInfo\", ref Global.JobInfoFileList, ".sfe");
        }

        private void LoginCommand()
        {
            #region Test Source
            //_worker.SendCommand(20,IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Door___DoAction, "On");
            //IsEnabledMenu[(int)enMainMenu.RECIPE] = false;
            #endregion

            if (Global.STLoginInfo.ID != string.Empty)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "Are you sure you want to log out?"))
                {
                    Global.STLoginInfo.ID = string.Empty;
                    Global.STLoginInfo.PassWord = string.Empty;
                    Global.STLoginInfo.AuthLevel = enAuthLevel.GUEST;
                }
                return;
            }

            View.Account.UserAccount login = new View.Account.UserAccount();
            login.Owner = Application.Current.MainWindow;
            login.ShowDialog();
        }

        private void LanguageCommand()
        {
            SFE.TRACK.Language.LanguageView lang = new SFE.TRACK.Language.LanguageView();
            lang.Owner = Application.Current.MainWindow;
            lang.ShowDialog();
        }

        private void ShutDownCommand()
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).Close();
        }

        private void Controller_EvtAlarmOccured(object sender, LibEvtArgs e)
        {
            EvtCommandLaunch launch = (EvtCommandLaunch)sender;  
            if (e.m_Params.Length == 0) // alarm cleared
            {
                //_worker.Controller.   ();
            }
            else // alarm occured
            {
                int index = 0;
                string code = (string)e.GetParam(index++);
                string maker = (string)e.GetParam(index++);
                string owner = (string)e.GetParam(index++);
                string fullMsg = (string)e.GetParam(index++);
                AlarmItem item = _worker.Reader.FindAlarmItem(code);

                AlarmLogCls alarm = new AlarmLogCls();
                alarm.Code = code;
                alarm.Owner = owner;
                alarm.Message = fullMsg;
                alarm.Help = item.Action;
                alarm.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                alarm.SendID = launch.FromID;
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    Global.STAlarmList.Add(alarm);
                    Global.STLog.AddAlarmLog(string.Format("{0}({1}){2}", alarm.Code, alarm.SendID, alarm.Message));
                });

                SetAlarm();
            }
        }

        private void _worker_EvtCommandLaunched(object sender, EvtCommandLaunch e)
        {
            if (e.Result != CommandResult.Idle) return;
            CommandItem item = (CommandItem)sender;
            IPCNetClient.DataType dataType = e.DataType;
            string[] arr = null;
            string[] arrParam = null;
            string packet = string.Empty;

            string message = e.LaunchedMessage.GetString();

            EnumCommand command = item.GetGroup<EnumCommand>();

            Global.STLog.AddSocketLog(e.FromID, "RECV", dataType.ToString(), command, item.GetEnumName()/* item.MainCmdName + "___" + item.SubCmdName*/, message);

            if (command == EnumCommand.Action)
            {
                EnumCommand_Action eValue = item.GetCommand<EnumCommand_Action>();
                if (eValue == EnumCommand_Action.Door___DoAction)
                {

                }
                else if(eValue == EnumCommand_Action.TermManual___Do)
                {
                    Global.ManualMessageClose();
                }
                else if (eValue == EnumCommand_Action.StatusChange___ServoOn)
                {

                }
                else if(eValue == EnumCommand_Action.ChamberManual___DoManual)
                {
                    if (Global.STMachineStatus == enMachineStatus.HOME) return;

                    arr = message.Split(':');

                    if(arr[1].ToUpper().Trim() == "TRUE")
                    {
                        Global.ManualMessageOpen();
                    }
                    else
                    {
                        Global.ManualMessageClose();
                    }
                }
            }
            else if (command == EnumCommand.Alarm)
            {
                EnumCommand_Alarm eValue = item.GetCommand<EnumCommand_Alarm>();
            }
            else if (command == EnumCommand.Warning)
            {
                EnumCommand_Warning eValue = item.GetCommand<EnumCommand_Warning>();
                if (eValue == EnumCommand_Warning.Send___Set)
                {
                    AlarmLogCls alarm = new AlarmLogCls();
                    alarm.Message = message;
                    alarm.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    alarm.SendID = Global.CHAMBER_ID;
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        Global.STWarningList.Add(alarm);
                    });
                    SetWarning();
                }
            }
            else if (command == EnumCommand.Config)
            {
                EnumCommand_Config eValue = item.GetCommand<EnumCommand_Config>();
            }
            else if (command == EnumCommand.Setting)
            {
                EnumCommand_Setting eValue = item.GetCommand<EnumCommand_Setting>();
            }
            else if (command == EnumCommand.Status)
            {
                EnumCommand_Status eValue = item.GetCommand<EnumCommand_Status>();

                if (eValue == EnumCommand_Status.RunStatus___Result)
                {
                    arr = message.Split(':');

                    if(arr[0].ToUpper() == "MACHINE" && arr[2].ToUpper() == "AUTO" && arr[3].ToUpper() == "STOP")
                    {
                        Global.STMachineStatus = enMachineStatus.STOP;
                    }

                }
                else if (eValue == EnumCommand_Status.MCS___LotStatus)
                {

                }
                else if (eValue == EnumCommand_Status.UnitStatus___MotorState)
                {
                    arr = message.Split(':');
                    arrParam = arr[1].Split(',');
                    foreach (AxisInfoCls axis in Global.STAxis)
                    {
                        if (axis.AxisID == arrParam[0])
                        {
                            axis.Servo = arrParam[1].Equals("1") ? true : false;
                            axis.Alarm = arrParam[2].Equals("1") ? true : false;
                            axis.IsStop = arrParam[3].Equals("1") ? true : false;
                            axis.InMotion = arrParam[4].Equals("1") ? true : false;
                            axis.InPosition = arrParam[5].Equals("1") ? true : false;
                            axis.PlusLimit = arrParam[6].Equals("1") ? true : false;
                            axis.PlusHomeLimit = arrParam[7].Equals("1") ? true : false;
                            axis.MinusLimit = arrParam[8].Equals("1") ? true : false;
                            axis.MinusHomeLimit = arrParam[9].Equals("1") ? true : false;
                            //axis.IsHome = arrParam[10].Equals("1") ? true : false;
                            axis.ActualPosition = Convert.ToDouble(arrParam[11]);
                            axis.CommandPosition = Convert.ToDouble(arrParam[12]);
                            break;
                        }
                    }
                }
                else if (eValue == EnumCommand_Status.UnitStatus___DIState)
                {
                    List<IODataCls> arInPutIO = Global.STDIList.FindAll(x => x.Company == "SFE_CAN" && x.BoardType == "SIO");
                    arInPutIO.Sort((IOListA, IOListB) => IOListA.IONum.CompareTo(IOListB.IONum));

                    int value = 0;
                    int bit = 0;
                    arr = message.Split(':');

                    for (int i = 0; i < arInPutIO.Count; i++)
                    {
                        IODataCls ioData = arInPutIO[i];
                        if (ioData.BoardType == "MINI") continue;
                        value = Convert.ToByte(arr[1][ioData.IONum / 4].ToString(), 16);
                        bit = ioData.IONum % 4;
                        ioData.State = ((value << (bit)) & 0x8) == 0x8;
                    }
                }
                else if (eValue == EnumCommand_Status.UnitStatus___DOState)
                {
                    List<IODataCls> arOutPutIO = Global.STDOList.FindAll(x => x.Company == "SFE_CAN" && x.BoardType == "SIO");
                    arOutPutIO.Sort((IOListA, IOListB) => IOListA.IONum.CompareTo(IOListB.IONum));

                    int value = 0;
                    int bit = 0;
                    arr = message.Split(':');

                    for (int i = 0; i < arOutPutIO.Count; i++)
                    {
                        IODataCls ioData = arOutPutIO[i];
                        if (ioData.BoardType == "MINI") continue;
                        value = Convert.ToByte(arr[1][ioData.IONum / 4].ToString(), 16);
                        bit = ioData.IONum % 4;
                        ioData.State = ((value << (bit)) & 0x8) == 0x8;
                    }
                }
                else if (eValue == EnumCommand_Status.UnitStatus___MiniBoardIOState)
                {
                    arr = message.Split(':');

                    //IODataCls ioData = null;
                    int value = 0;
                    for (int i = 0; i < arr[0].Length; i++)//DO
                    {
                        value = Convert.ToByte(arr[0][i].ToString(), 16);
                        //for (int j = 0; j < 3; j++)
                        {
                            IODataCls data = Global.STDOList.Find(x => x.BoardType == "MINI" && x.IOIndex == 0 && x.ModuleNo == 5 + i);
                            if (data == null) continue;

                            data.State = ((value >> (0)) & 0x1) == 0x1;
                        }
                    }

                    for (int i = 0; i < arr[1].Length; i++)//DI
                    {
                        value = Convert.ToByte(arr[1][i].ToString(), 16);
                        //IODataCls data = Global.STDIList.Find(x => x.BoardType == "MINI" && x.IOIndex == 0 && x.ModuleNo == 5 + i);
                        //if (data == null) continue;
                        for (int j = 0; j < 3; j++)
                        {
                            IODataCls data = Global.STDIList.Find(x => x.BoardType == "MINI" && x.IOIndex == j && x.ModuleNo == 5 + i);
                            if (data == null) continue;

                            data.State = ((value >> (j)) & 0x1) == 0x1;
                        }
                        //ioData.State = (value & 0x1) == 0x1;
                    }
                }
                else if (eValue == EnumCommand_Status.MCS___InitStep)
                {
                    arr = message.Split(':');
                    ModuleBaseCls module = null;
                    enHomeState eState = enHomeState.HOME_NONE;
                    if (arr[1] == "PRB")
                    {
                        module = Global.GetModule(2, 0);
                        if (arr[2] == enWorkingNeedStep.IsDone.ToString()) eState = enHomeState.HOME_OK;
                        else if (arr[2] == enWorkingNeedStep.IsDoing.ToString()) eState = enHomeState.HOMMING;
                        else if (arr[2] == enWorkingNeedStep.IsDoneFail.ToString()) eState = enHomeState.HOME_ERROR;
                        module.HomeSituation = eState;
                        if (eState == enHomeState.HOME_OK) module.ModuleState = enModuleState.STANDBY;
                        else if (eState == enHomeState.HOME_ERROR) module.ModuleState = enModuleState.PROBLEM;
                        foreach (AxisInfoCls axis in Global.STAxis)
                        {
                            if (axis.Parent == "PRA") axis.HomeSituation = eState;
                        }
                    }
                    else if (arr[1] == "CSB")
                    {
                        module = Global.GetModule(1, 0);
                        if (arr[2] == enWorkingNeedStep.IsDone.ToString()) eState = enHomeState.HOME_OK;
                        else if (arr[2] == enWorkingNeedStep.IsDoing.ToString()) eState = enHomeState.HOMMING;
                        else if (arr[2] == enWorkingNeedStep.IsDoneFail.ToString()) eState = enHomeState.HOME_ERROR;
                        module.HomeSituation = eState;
                        if (eState == enHomeState.HOME_OK) module.ModuleState = enModuleState.STANDBY;
                        else if (eState == enHomeState.HOME_ERROR) module.ModuleState = enModuleState.PROBLEM;
                        foreach (AxisInfoCls axis in Global.STAxis)
                        {
                            if (axis.Parent == "CRA") axis.HomeSituation = eState;
                        }
                    }
                    else if (arr[1] == "IFB")
                    {

                    }
                    else if (arr[1] == "SFETrack")
                    {
                        //end
                    }
                }
                else if (eValue == EnumCommand_Status.Chamber___InitialResult)
                {
                    arr = message.Split(':');
                    enHomeState eState = enHomeState.HOMMING;
                    if (arr[4] == enWorkingNeedStep.IsDone.ToString()) eState = enHomeState.HOME_OK;
                    else if (arr[4] == enWorkingNeedStep.IsDoneFail.ToString()) eState = enHomeState.HOME_ERROR;
                    else if (arr[4] == enWorkingNeedStep.IsDoing.ToString()) eState = enHomeState.HOMMING;

                    if (arr[1] == "Chamber")
                    {
                        int block = Convert.ToInt32(arr[2]);
                        int module = Convert.ToInt32(arr[3]);
                        ModuleBaseCls modulebase = Global.GetModule(block, module);
                        if (modulebase != null)
                        {
                            if (!modulebase.Use) return;
                            if (arr[4] == "IsDone")
                            {
                                modulebase.HomeSituation = eState;
                                modulebase.ModuleState = enModuleState.STANDBY;
                                foreach (AxisInfoCls axis in Global.STAxis)
                                {
                                    if (axis.BlockNo == block && axis.ModuleNo == module) axis.HomeSituation = eState;
                                }
                            }
                        }
                    }
                }
                else if (eValue == EnumCommand_Status.Chamber___OriginMove)
                {
                    arr = message.Split(':');

                    foreach (AxisInfoCls axis in Global.STAxis)
                    {
                        if (axis.AxisID == arr[1])
                        {
                            if (arr[2] == enWorkingNeedStep.IsDone.ToString()) axis.HomeSituation = enHomeState.HOME_OK;
                            else if (arr[2] == enWorkingNeedStep.IsDoing.ToString()) axis.HomeSituation = enHomeState.HOMMING;
                            else if (arr[2] == enWorkingNeedStep.IsDoneFail.ToString()) axis.HomeSituation = enHomeState.HOME_ERROR;
                            break;
                        }
                    }
                }
                else if (eValue == EnumCommand_Status.Cassette___Scan)
                {
                    arr = message.Split(':');
                    if (arr[2] == enWorkingNeedStep.IsDone.ToString())
                    {
                        packet = string.Format("Foup:{0}", arr[1]);
                        _worker.SendCommand(Global.MCS_ID, IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Cassette___MapData, packet);
                    }
                }
                else if (eValue == EnumCommand_Status.Cassette___MapData)
                {
                    arr = message.Split(':');
                    FoupCls foupCls = Global.STModuleList.Find(x => x.ModuleType == enModuleType.FOUP && x.ModuleNo == Convert.ToInt32(arr[1])) as FoupCls;
                    foupCls.IsDetect = true;
                    foupCls.IsScan = true;
                    for (int i = 0; i < arr[2].Length; i++)
                    {
                        if (arr[2][i] == '1') foupCls.FoupWaferList[i].WaferState = enWaferState.WAFER_EXIST;
                        else if (arr[2][i] == '0') foupCls.FoupWaferList[i].WaferState = enWaferState.WAFER_EMPTY;
                    }
                }
                else if (eValue == EnumCommand_Status.UnitStatus___ChamberState)
                {
                    arr = message.Split(':');
                    ModuleBaseCls module = Global.GetModule(Convert.ToInt32(arr[1]), Convert.ToInt32(arr[2]));
                    if (module == null) return;
                    if (!module.Use) return;
                    module.ModuleState = (enModuleState)Convert.ToInt32(arr[3]);
                }
                else if (eValue == EnumCommand_Status.MCS___SerialData)
                {
                    List<string> list;
                    list = Tokenizer.Split(message, false, false, "\r\n");
                    if (AnalyseOwner(list[0], out var typeName, out var name, out var unitID, out var title) == false) return;
                    if (AnalyseSerialDataType(list[1], out var isAll, out var ownID, out var itemID) == false) return;
                    if (title.IndexOf("WaferData") == 0)
                    {
                        list.RemoveRange(0, 2);
                        TreatSerialWaferData(typeName, name, unitID, title, ownID, list);
                    }
                }
                else if (eValue == EnumCommand_Status.UnitStatus___MiniBoardState)
                {
                    arr = message.Split(':');

                    ChamberCls chamber = (ChamberCls)Global.GetModule(Convert.ToInt32(arr[1]), Convert.ToInt32(arr[2]));

                    AxisInfoCls axis = Global.STAxis.Find(x => x.BlockNo == Convert.ToInt32(arr[1]) && x.ModuleNo == Convert.ToInt32(arr[2]) && x.AxisID.ToUpper().IndexOf("_PIN") != -1);

                    if (chamber == null || axis == null) return;

                    chamber.ProcessValue = Convert.ToSingle(arr[3]);
                    chamber.SetValue = Convert.ToSingle(arr[4]);
                    if (Convert.ToInt32(arr[5]) == 1) axis.InMotion = true;
                    else axis.InMotion = false;

                    if (Convert.ToInt32(arr[6]) == 1) axis.MinusLimit = true;
                    else axis.MinusLimit = false;

                    if (Convert.ToInt32(arr[7]) == 1) axis.PlusLimit = true;
                    else axis.PlusLimit = false;

                    if (Convert.ToInt32(arr[8]) == 1) axis.HomeSituation = enHomeState.HOME_OK;
                    else axis.HomeSituation = enHomeState.HOME_NONE;

                    axis.ActualPosition = Convert.ToInt32(arr[9]);
                }
                else if (eValue == EnumCommand_Status.DATA___ChamberMonitoringData)
                {
                    //여기에 모니터링 데이터를 넣어준다.
                }
                else if (eValue == EnumCommand_Status.Chamber___PutReady)
                {
                    packet = string.Format("Chamber:2:1");
                    Global.MachineWorker.SendCommand(Global.CHAMBER_ID, IPCNetClient.DataType.String, EnumCommand.Status, EnumCommand_Status.Chamber___StartProcess, packet);
                }
                else if (eValue == EnumCommand_Status.Chamber___EndProcess)
                {
                    Console.WriteLine("");
                }
            }

            e.SetResult(CommandResult.Success, "done");
        }

        private void _worker_EvtCommandResultComes(object sender, EvtCommandResult e)
        {
            string[] groupArr = null;
            string[] commandArr = null;
            CommandItem item = (CommandItem)sender;
            CommandResult result = e.Result;
            string resultString = e.ResultString;

            if (item.GetGroupName() == EnumCommand.Action.ToString() && item.GetEnumName() == EnumCommand_Action.Move___EncoderPos.ToString())
            {
                groupArr = resultString.Split(':');
                commandArr = groupArr[1].Split(',');
                foreach (AxisInfoCls info in Global.STAxis)
                {
                    if (info.AxisID == commandArr[0])
                    {
                        info.ActualPosition = Convert.ToDouble(commandArr[1]);
                        break;
                    }
                }
            }
            else if(item.GetGroupName() == EnumCommand.Action.ToString() && item.GetEnumName() == EnumCommand_Action.TermManual___Do.ToString())
            {
                Global.ManualMessageClose();
                if(result == CommandResult.Success)
                {
                    //groupArr = resultString.Split(':');
                    //commandArr = groupArr[1].Split(' ');

                    //if (commandArr[0] == "DoScanCassette")
                    {
                        //FoupCls foupCls = Global.STModuleList.Find(x => x.ModuleType == enModuleType.FOUP && x.ModuleNo == Convert.ToInt32(commandArr[1])) as FoupCls;
                        //foupCls.IsDetect = true;
                        //foupCls.IsScan = true;
                        //for (int i = 0; i < groupArr[2].Length; i++)
                        //{
                        //    if (groupArr[2][i] == '1') foupCls.FoupWaferList[i].WaferState = enWaferState.WAFER_EXIST;
                        //    else if (groupArr[2][i] == '0') foupCls.FoupWaferList[i].WaferState = enWaferState.WAFER_EMPTY;
                        //}
                    }
                }
            }

        }

        private void _worker_EvtCommandStatusChanged(object sender, EvtCommandStatus e)
        {
            CommandItem item = (CommandItem)sender;
            CommandStatus status = e.Status;
        }

        private void _worker_EvtPrgCfgReloaded(object sender, LibEvtArgs e)
        {
            PrgCfgItem item = (PrgCfgItem)e.GetParam();
            if (Enum.TryParse<EnumConfigGroup>(item.GroupName, out var enumGroup) == false) return;
            if (enumGroup == EnumConfigGroup.Environment)
            {
                if (Enum.TryParse<EnumConfig_Environment>(item.Name, out var enumItem) == false) return;
                if (enumItem == EnumConfig_Environment.Operation)
                {
                    Console.WriteLine("");
                }
                else if (enumItem == EnumConfig_Environment.DummyLinkRecipeInfo)
                {
                    Console.WriteLine("");
                }
                else if (enumItem == EnumConfig_Environment.RecipeTransperInfo)
                {
                    Console.WriteLine("");
                }
                else if(enumItem == EnumConfig_Environment.TowerLamp)
                {
                    GetTowerLamp(item);
                }
            }
            else if (enumGroup == EnumConfigGroup.Lot)
            {

            }
            else if (enumGroup == EnumConfigGroup.Motor_CRA_X)
            {
                if (Enum.TryParse<EnumConfig_Motor_CRA_X>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_CRA_Y)
            {
                if (Enum.TryParse<EnumConfig_Motor_CRA_Y>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_CRA_Z)
            {
                if (Enum.TryParse<EnumConfig_Motor_CRA_Z>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_PRA_X1)
            {
                if (Enum.TryParse<EnumConfig_Motor_PRA_X1>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_PRA_X2)
            {
                if (Enum.TryParse<EnumConfig_Motor_PRA_X2>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_PRA_X3)
            {
                if (Enum.TryParse<EnumConfig_Motor_PRA_X3>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_PRA_Y)
            {
                if (Enum.TryParse<EnumConfig_Motor_PRA_Y>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_PRA_Z)
            {
                if (Enum.TryParse<EnumConfig_Motor_PRA_Z>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_PRA_T)
            {
                if (Enum.TryParse<EnumConfig_Motor_PRA_T>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_D1_X1)
            {
                if (Enum.TryParse<EnumConfig_Motor_D1_X1>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_D1_X2)
            {
                if (Enum.TryParse<EnumConfig_Motor_D1_X2>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_C1_X1)
            {
                if (Enum.TryParse<EnumConfig_Motor_C1_X1>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_C1_X2)
            {
                if (Enum.TryParse<EnumConfig_Motor_C1_X2>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_C1_BT)
            {
                if (Enum.TryParse<EnumConfig_Motor_C1_BT>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_C1_PP)
            {
                if (Enum.TryParse<EnumConfig_Motor_C1_PP>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_D2_X1)
            {
                if (Enum.TryParse<EnumConfig_Motor_D2_X1>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_D2_X2)
            {
                if (Enum.TryParse<EnumConfig_Motor_D2_X2>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_CB01_Pin)
            {
                if (Enum.TryParse<EnumConfig_Motor_CB01_Pin>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_CB02_Pin)
            {
                if (Enum.TryParse<EnumConfig_Motor_CB02_Pin>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_CB03_Pin)
            {
                if (Enum.TryParse<EnumConfig_Motor_CB03_Pin>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_CB04_Pin)
            {
                if (Enum.TryParse<EnumConfig_Motor_CB04_Pin>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_CB05_Pin)
            {
                if (Enum.TryParse<EnumConfig_Motor_CB05_Pin>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_CB06_Pin)
            {
                if (Enum.TryParse<EnumConfig_Motor_CB06_Pin>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_CB07_Pin)
            {
                if (Enum.TryParse<EnumConfig_Motor_CB07_Pin>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_CB08_Pin)
            {
                if (Enum.TryParse<EnumConfig_Motor_CB08_Pin>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_CB09_Pin)
            {
                if (Enum.TryParse<EnumConfig_Motor_CB09_Pin>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_CB10_Pin)
            {
                if (Enum.TryParse<EnumConfig_Motor_CB10_Pin>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_CB11_Pin)
            {
                if (Enum.TryParse<EnumConfig_Motor_CB11_Pin>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
            else if (enumGroup == EnumConfigGroup.Motor_CB12_Pin)
            {
                if (Enum.TryParse<EnumConfig_Motor_CB12_Pin>(item.Name, out var enumItem) == false) return;
                SetTeachingData(enumGroup.ToString(), enumItem.ToString(), item);
            }
        }

        private void _worker_EvtInfoReloaded(object sender, EventArgs e)
        {

        }

        private bool AnalyseSerialDataType(string message, out bool isAll, out int ownID, out int itemID)
        {
            int index = 0;
            isAll = false;
            ownID = -1;
            itemID = -1;

            List<string> list = Tokenizer.Split(message, false, false, ":");
            if (list.Count < 2) return false;
            if (list[index++] == "ALL") isAll = true;
            ownID = Convert.ToInt32(list[index++]);
            if (list.Count > index) itemID = Convert.ToInt32(list[index++]);
            return true;
        }

        private void TreatSerialWaferData(NameBase.TYPE_NAME typeName, string name, int unitID, string title, int arrayID, List<string> datList)
        {
            List<string> list = Tokenizer.Split(title, "_");
            string middleName = list[1];
            ModuleBaseCls moduleBase = null;
            if (typeName == NameBase.TYPE_NAME.ASSY)
            {
                if (name == "CRA")
                {
                    if (middleName == "Cassette")
                    {
                        moduleBase = Global.STModuleList.Find(x => x.ModuleType == enModuleType.FOUP && x.BlockNo == 1 && x.ModuleNo == (arrayID + 1));
                    }
                    else if (middleName == "Robot")
                    {
                        moduleBase = Global.STModuleList.Find(x => x.ModuleType == enModuleType.CRA);
                    }
                }
                else if (name == "PRA")
                {
                    moduleBase = Global.STModuleList.Find(x => x.ModuleType == enModuleType.PRA);
                }
                else if (name == "IRA")
                {
                    return;
                }
                else if (name == "Developer")
                {
                    moduleBase = Global.STModuleList.Find(x => x.ModuleType == enModuleType.SPINCHAMBER && x.ModuleNo == (arrayID + 2));
                }
                else if (name == "Coater")
                {
                    if (arrayID == 1) return;
                    moduleBase = Global.STModuleList.Find(x => x.ModuleType == enModuleType.SPINCHAMBER && x.ModuleNo == (arrayID + 1));
                }
                else if (name == "Chamber")
                {
                    if (middleName == "Process")
                    {
                        moduleBase = Global.STModuleList.Find(x => x.ModuleType == enModuleType.CHAMBER && x.ModuleNo == (arrayID + 5));
                    }
                }
                else if (name == "Interface")
                {
                    return;
                }

                if (moduleBase == null) return;

                foreach (string str in datList)
                {
                    SetDataInWaferDataArray(name, middleName, moduleBase, str);
                }
            }
        }

        private void SetDataInWaferDataArray(string moduleName, string middleName, ModuleBaseCls moduleBase, string strValue)
        {
            WaferCls wafer = moduleBase.Wafer;
            List<string> list = Tokenizer.Split(strValue, ",");

            int id = Convert.ToInt32(list[0]);
            bool exist = false; if (Convert.ToInt32(list[1]) == 1) exist = true;
            string name = list[6];
            list.RemoveRange(0, 7);

            if (middleName == "Cassette")
            {
                FoupCls foup = (FoupCls)moduleBase;
                wafer = foup.FoupWaferList[id];
            }

            if (moduleName == "Chamber")
            {
                moduleBase = Global.STModuleList.Find(x => x.ModuleType == enModuleType.CHAMBER && x.ModuleNo == (id + 5));
                wafer = moduleBase.Wafer;
            }

            wafer.Recipe.Name = name;
            wafer.IsWafer = exist.Equals(true) ? Visibility.Visible : Visibility.Hidden;

            for (int i = 0; i < wafer._RecipeInfos.Length; i++) wafer._RecipeInfos[i].Init();

            for (int i = 0; i < list.Count; i++)
            {
                if (AnalyseWaferData(list[i], out var recipeID, out var recipeName, out var recipeType, out var recipeStep, out var blockNo, out var moduleNoList) == false) continue;
                wafer._RecipeInfos[recipeID].SetInfo(recipeName, recipeType, recipeStep, blockNo, moduleNoList);
            }

            if (middleName == "Cassette")
            {
                wafer.SetCstWaferState();
            }
            else
            {
                if (wafer.IsWafer == Visibility.Visible) moduleBase.SetWaferState();
                else wafer.WaferState = enWaferState.WAFER_NONE;
            }
        }

        private bool AnalyseWaferData(string message, out int recipeID, out string recipeName, out EnumRecipeDetailType recipeType, out WorkStep step, out int blockNo, out List<int> moduleNo)
        {
            int index = 0;
            recipeID = -1;
            recipeName = "";
            recipeType = EnumRecipeDetailType.Not;
            step = WorkStep.IsNot;
            blockNo = -1;
            moduleNo = new List<int>();
            List<string> list = Tokenizer.Split(message, false, false, ":");
            if (list.Count < 6) return false;

            recipeID = Convert.ToInt32(list[index++]);
            recipeName = list[index++];
            recipeType = (EnumRecipeDetailType)Convert.ToInt32(list[index++]);
            step = (WorkStep)Convert.ToInt32(list[index++]);
            blockNo = Convert.ToInt32(list[index++]);

            list.RemoveRange(0, index);

            for (int i = 0; i < list.Count; i++)
            {
                moduleNo.Add(Convert.ToInt32(list[i]));
            }


            return true;
        }

        protected bool AnalyseOwner(string message, out NameBase.TYPE_NAME typeName, out string name, out int unitID, out string remain)
        {
            int index = 0;
            typeName = NameBase.TYPE_NAME.MACHINE;
            name = "";
            unitID = -1;
            remain = "";
            List<string> list = Tokenizer.Split(message, false, false, ":");
            if (list.Count < 2) return false;
            typeName = MachineReader.Instance.GetTypeName(list[index++]);
            name = list[index++];

            if (list.Count > 2) unitID = Convert.ToInt32(list[index++]);
            if (list.Count > 3) remain = list[index++];

            return true;
        }

        private void SetTeachingData(string group, string name, PrgCfgItem item)
        {
            group = group.Replace("Motor_", "");
            name = name.Replace("Teaching_", "");
            foreach (TeachingDataCls data in Global.STTeachingData)
            {
                if (data.Motor.MyNameInfo.Name == group && data.TeachingName == name)
                {
                    string[] arr = item.Contents.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    data.Pos = Convert.ToDouble(arr[0]);
                    data.Vel = Convert.ToDouble(arr[1]);
                    data.Acc = Convert.ToDouble(arr[2]);
                    data.Dec = Convert.ToDouble(arr[3]);
                    data.TimeOut = Convert.ToInt32(arr[4]);
                }
            }
        }
    }
}