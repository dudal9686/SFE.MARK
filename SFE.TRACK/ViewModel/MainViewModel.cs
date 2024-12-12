using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System;
using SFE.TRACK.Model;
using System.Collections.Generic;
using CoreCSBase;
using CoreCSBase.IPC;
using CoreCSMac;
using MachineCSBaseSim;
using MachineDefine;
using System.Windows.Threading;

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
        public RelayCommand ShutDownRelayCommand  { get; set; }
        public RelayCommand LoginRelayCommand { get; set; }
        bool isSelectedAuto = true;
        bool isSelectedRecipe = false;
        bool isSelectedIO = false;
        bool isSelectedParam = false;
        bool isSelectedMaint = false;
        bool isSelectedUtil = false;
        bool isSelectedGem = false;
        bool isSelectedLog = false;
        bool isSelectedAlarm = false;

        List<LMBase> LmList = null;
        List<AssyBase> AssyList = null;
        List<UnitIO> DIList = null;
        List<UnitIO> DOList = null;
        List<UnitAIO> AIOList = null;
        List<UnitMotor> MotorList = null;
        List<UnitCustom> CustomUnitList = null;

        System.Windows.Media.SolidColorBrush TitleColor_ = System.Windows.Media.Brushes.MidnightBlue;
        MachineReaderWorker _worker;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {                      
            ShutDownRelayCommand = new RelayCommand(ShutDownCommand);
            LoginRelayCommand = new RelayCommand(LoginCommand);

            _worker = new MachineReaderWorker();
            Global.MachineWorker = _worker;
            if (_worker.StartWorker(1000, "HMI", @"C:\MachineSet\SFETrack.mm", true) == false)
            {
                MessageBox.Show("Starting worker failed");
                return;
            }

            _worker.EvtCommandLaunched += _worker_EvtCommandLaunched;
            _worker.EvtCommandStatusChanged += _worker_EvtCommandStatusChanged;
            _worker.EvtCommandResultComes += _worker_EvtCommandResultComes;
            _worker.EvtInfoReloaded += _worker_EvtInfoReloaded;
            _worker.EvtPrgCfgReloaded += _worker_EvtPrgCfgReloaded1;
            _worker.Controller.EvtAlarmOccured += Controller_EvtAlarmOccured;

            InitData();
            InitJobProcess();
            InitIO();

            FoupCls foup_ = Global.STModuleList.Find(x => x.ModuleType == enModuleType.FOUP && x.ModuleNo == 1) as FoupCls;
            foup_.IsDetect = true;
            foup_.IsScan = true;

            foup_ = Global.STModuleList.Find(x => x.ModuleType == enModuleType.FOUP && x.ModuleNo == 2) as FoupCls;
            foup_.IsDetect = true;
            foup_.IsScan = true;
        }

        #region MainMenu
        public bool IsSelectedAuto
        {
            get { return isSelectedAuto; }
            set { isSelectedAuto = value; RaisePropertyChanged("IsSelectedAuto"); }
        }

        public bool IsSelectRceipe
        {
            get { return isSelectedRecipe; }
            set { isSelectedRecipe = value; RaisePropertyChanged("IsSelectRceipe"); }
        }

        public bool IsSelectedIO
        {
            get { return isSelectedIO; }
            set { isSelectedIO = value; RaisePropertyChanged("IsSelectedIO"); }
        }

        public bool IsSelectedParam
        {
            get { return isSelectedParam; }
            set { isSelectedParam = value; RaisePropertyChanged("IsSelectedParam"); }
        }

        public bool IsSelectedMaint
        {
            get { return isSelectedMaint; }
            set { isSelectedMaint = value; RaisePropertyChanged("IsSelectedMaint"); }
        }

        public bool IsSelectedUtil
        {
            get { return isSelectedUtil; }
            set { isSelectedUtil = value; RaisePropertyChanged("IsSelectedUtil"); }
        }

        public bool IsSelectedGem
        {
            get { return isSelectedGem; }
            set { isSelectedGem = value; RaisePropertyChanged("IsSelectedGem"); }
        }

        public bool IsSelectedLog
        {
            get { return isSelectedLog; }
            set { isSelectedLog = value; RaisePropertyChanged("IsSelectedLog"); }
        }

        public bool IsSelectedAlarm
        {
            get { return isSelectedAlarm; }
            set { isSelectedAlarm = value; RaisePropertyChanged("IsSelectedAlarm"); }
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

            for(int i = 0; i < 10; i++) Global.STJobInfo.LotInfoList.Add(new LotInfoCls());
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
            Global.STDataAccess.ReadUserInfo();
            Global.STDataAccess.ReadMaintSupportData();
            
            return true;
        }
        //이미 지정된 축 정보를 dll을 통해 가져와야 한다.

        private void SetAxisData()
        {
            foreach (UnitMotor motor in MotorList)
            {
                AxisInfoCls axis = new AxisInfoCls();
                axis.AxisID = motor.MyNameInfo.Name;
                axis.Motor = motor;
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
                            arr = axis.Motor.MyNameInfo.Information.Split('\r');
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
            
            foreach(ModuleBaseCls module in list)
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
            IODataCls data;            
            foreach(UnitIO io in DIList)
            {
                data = new IODataCls();                
                arr = io.MyNameInfo.Information.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                data.IO = io;
                data.BlockNo = 2;
                data.IOType = enIOType.DI;
                data.Alias = io.MyNameInfo.Alias;
                data.Enable = false;
                
                if (io.MyNameInfo.Alias == "CRA" || io.MyNameInfo.Alias == "PRA")
                {
                    if(io.MyNameInfo.Alias == "CRA") data.BlockNo = 1;
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

                Global.STDIList.Add(data);
            }
        }

        private void GetDOData()
        {
            string[] arr;
            int moduleNo = 0;
            ModuleBaseCls module;
            IODataCls data;
            foreach (UnitIO io in DOList)
            {
                data = new IODataCls();
                arr = io.MyNameInfo.Information.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                data.IO = io;
                data.BlockNo = 2;
                data.IOType = enIOType.DI;
                data.Alias = io.MyNameInfo.Alias;
                data.Enable = true;

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
                data.IOType = enIOType.DI;
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
            IsSelectedAlarm = true;
            TitleColor = System.Windows.Media.Brushes.Red;
        }
        public void ClearAlarm(string code = "")
        {
            //_worker.Controller.ClearAlarm();
            if (Global.STAlarmList.Count == 0)
            {
                IsSelectedAlarm = false;
                TitleColor = System.Windows.Media.Brushes.MidnightBlue;
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
            //PrgCfgItem item = _worker.Reader.GetConfigItem(EnumConfigGroup.Lot, EnumConfig_Lot.Job);
            //item.Explain = "Lot";
            //List<string> list = new List<string>();
            //list.Add("A");
            //list.Add("B");
            //item.SetValue(list);
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
            login.Owner = (MainWindow)System.Windows.Application.Current.MainWindow;
            login.ShowDialog();
        }

        private void ShutDownCommand()
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).Close();
        }

        private void Controller_EvtAlarmOccured(object sender, LibEvtArgs e)
        {
            if (e.m_Params.Length == 0) // alarm cleared
            {

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
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    Global.STAlarmList.Add(alarm);
                });
                SetAlarm();
            }             
        }

        private void _worker_EvtCommandLaunched(object sender, EvtCommandLaunch e)
        {
            if (e.Result != CommandResult.Idle) return;
            CommandItem item = (CommandItem)sender;
            IPCNetClient.DataType dataType = e.DataType;

            string message = e.LaunchedMessage.GetString();

            EnumCommand command = item.GetGroup<EnumCommand>();

            if (command == EnumCommand.Action)
            {
                EnumCommand_Action eValue = item.GetCommand<EnumCommand_Action>();
                if (eValue == EnumCommand_Action.Door___DoAction)
                {

                }
            }
            else if (command == EnumCommand.Alarm)
            {
                EnumCommand_Alarm eValue = item.GetCommand<EnumCommand_Alarm>();
            }
            else if (command == EnumCommand.Warning)
            {
                EnumCommand_Warning eValue = item.GetCommand<EnumCommand_Warning>();
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
            }

            e.SetResult(CommandResult.Success, "done");
        }

        private void _worker_EvtCommandResultComes(object sender, EvtCommandResult e)
        {
            CommandItem item = (CommandItem)sender;
            CommandResult result = e.Result;
            string resultString = e.ResultString;
        }

        private void _worker_EvtCommandStatusChanged(object sender, EvtCommandStatus e)
        {
            CommandItem item = (CommandItem)sender;
            CommandStatus status = e.Status;
        }

        private void _worker_EvtPrgCfgReloaded1(object sender, LibEvtArgs e)
        {
            PrgCfgItem item = (PrgCfgItem)e.GetParam();
            if (Enum.TryParse<EnumConfigGroup>(item.GroupName, out var enumGroup) == false) return;
            if (enumGroup == EnumConfigGroup.Environment)
            {
                if (Enum.TryParse<EnumConfig_Environment>(item.Name, out var enumItem) == false) return;
                if (enumItem == EnumConfig_Environment.Operation)
                {

                }
            }
            else if (enumGroup == EnumConfigGroup.Lot)
            {

            }
        }

        private void _worker_EvtPrgCfgReloaded(object sender, EventArgs e)
        {

        }

        private void _worker_EvtInfoReloaded(object sender, EventArgs e)
        {

        }
    }
}