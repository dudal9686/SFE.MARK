using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFE.TRACK.DB;
using SFE.TRACK.Model;
using System.Threading;
using System.Windows.Forms;
using System.Windows;
using System.IO;
using SFE.TRACK.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using CoreCSRunSim;

namespace SFE.TRACK
{
    public class Global
    {
        public static MachineReaderWorker MachineWorker = null;
        public static List<Model.ModuleBaseCls> STModuleList = new List<Model.ModuleBaseCls>();
        public static List<Model.WaferCls> STWaferList = new List<Model.WaferCls>();
        public static AccessDBCls STAccessDB = new AccessDBCls();
        public static DataAccessCls STDataAccess = new DataAccessCls();
        public static Log.LogCls STLog = new Log.LogCls();
        public static List<AxisInfoCls> STAxis = new List<AxisInfoCls>();
        public static JobInfoCls STJobInfo = new JobInfoCls();
        
        public static List<IODataCls> STDIList = new List<IODataCls>();
        public static List<IODataCls> STDOList = new List<IODataCls>();
        public static List<AIODataCls> STAIOList = new List<AIODataCls>();

        public static List<TeachingDataCls> STTeachingData = new List<TeachingDataCls>();
        public static List<MonitoringDataCls> STMonitoringList = new List<MonitoringDataCls>();

        public static ObservableCollection<AlarmLogCls> STAlarmList { get; set; } = new ObservableCollection<AlarmLogCls>();
        public static ObservableCollection<AlarmLogCls> STWarningList { get; set; } = new ObservableCollection<AlarmLogCls>();
        public static List<DispenseInfoCls> STDispenseList { get; set; } = new List<DispenseInfoCls>();
        //RegisterViewMain 에서 단독으로 하려 했으나 ViewModel에서 메모리를 가지고 있기 떄문에 글로벌에서 가지고 있는다.
        public static List<LoginInfoCls> STUserList = new List<LoginInfoCls>();
        public static LoginInfoCls STLoginInfo = new LoginInfoCls(); //로그인 후 권한 설정
        
        public static int MCS_ID = 1;
        public static int CHAMBER_ID = 55;
        public static int MMI_ID = 1000;
        public static int HOME_TIMEOUT = 90000;
        public static bool IsShutDown = false;
        //Maint Suppot List
        public static List<MaintSupportCls> STMaintSupportList = new List<MaintSupportCls>();
        //AlarmMessage
        private static Alarm.AlarmMessage STAlarmMessage = null;

        #region Recipe 폴더 리스트 (여러곳에서 사용하기 때문에 한군데 에서만 가져온다.
        public static ObservableCollection<DirFileListCls> WaferFlowRecipeFileList = new ObservableCollection<DirFileListCls>();
        public static ObservableCollection<DirFileListCls> ADHDummyCondRecipeFileList = new ObservableCollection<DirFileListCls>();
        public static ObservableCollection<DirFileListCls> ADHDummySeqRecipeFileList = new ObservableCollection<DirFileListCls>();
        public static ObservableCollection<DirFileListCls> ADHProcessRecipeFileList = new ObservableCollection<DirFileListCls>();
        public static ObservableCollection<DirFileListCls> CleanCondRecipeFileList = new ObservableCollection<DirFileListCls>();
        public static ObservableCollection<DirFileListCls> CoaterCleanRecipeFileList = new ObservableCollection<DirFileListCls>();
        public static ObservableCollection<DirFileListCls> CoaterDummyCondRecipeFileList = new ObservableCollection<DirFileListCls>();
        public static ObservableCollection<DirFileListCls> CoaterDummySeqRecipeFileList = new ObservableCollection<DirFileListCls>();
        public static ObservableCollection<DirFileListCls> CoaterProcessRecipeFileList = new ObservableCollection<DirFileListCls>();
        public static ObservableCollection<DirFileListCls> CPLProcessRecipeFileList = new ObservableCollection<DirFileListCls>();
        public static ObservableCollection<DirFileListCls> DevCleanRecipeFileList = new ObservableCollection<DirFileListCls>();
        public static ObservableCollection<DirFileListCls> DevDummyCondRecipeFileList = new ObservableCollection<DirFileListCls>();
        public static ObservableCollection<DirFileListCls> DevDummySeqRecipeFileList = new ObservableCollection<DirFileListCls>();
        public static ObservableCollection<DirFileListCls> DevProcessRecipeFileList = new ObservableCollection<DirFileListCls>();
        public static ObservableCollection<DirFileListCls> DummyCondLinkRecipeFileList = new ObservableCollection<DirFileListCls>();
        public static ObservableCollection<DirFileListCls> HHPProcessRecipeFileList = new ObservableCollection<DirFileListCls>();
        public static ObservableCollection<DirFileListCls> LHPProcessRecipeFileList = new ObservableCollection<DirFileListCls>();
        public static ObservableCollection<DirFileListCls> PumpRecipeFileList = new ObservableCollection<DirFileListCls>();
        public static ObservableCollection<DirFileListCls> SystemRecipeFileList = new ObservableCollection<DirFileListCls>();
        public static ObservableCollection<DirFileListCls> TCPProcessRecipeFileList = new ObservableCollection<DirFileListCls>();
        public static ObservableCollection<DirFileListCls> JobInfoFileList = new ObservableCollection<DirFileListCls>();
        #endregion

        public static uint[] STDispenseIndex = new uint[32]{
            (uint)enDispenseNo.DISPENSE_01, (uint)enDispenseNo.DISPENSE_02, (uint)enDispenseNo.DISPENSE_03, (uint)enDispenseNo.DISPENSE_04, (uint)enDispenseNo.DISPENSE_05,
            (uint)enDispenseNo.DISPENSE_06, (uint)enDispenseNo.DISPENSE_07, (uint)enDispenseNo.DISPENSE_08, (uint)enDispenseNo.DISPENSE_09, (uint)enDispenseNo.DISPENSE_10,
            (uint)enDispenseNo.DISPENSE_11, (uint)enDispenseNo.DISPENSE_12, (uint)enDispenseNo.DISPENSE_13, (uint)enDispenseNo.DISPENSE_14, (uint)enDispenseNo.DISPENSE_15,
            (uint)enDispenseNo.DISPENSE_16, (uint)enDispenseNo.DISPENSE_17, (uint)enDispenseNo.DISPENSE_18, (uint)enDispenseNo.DISPENSE_19, (uint)enDispenseNo.DISPENSE_20,
            (uint)enDispenseNo.DISPENSE_21, (uint)enDispenseNo.DISPENSE_22, (uint)enDispenseNo.DISPENSE_23, (uint)enDispenseNo.DISPENSE_24, (uint)enDispenseNo.DISPENSE_25,
            (uint)enDispenseNo.DISPENSE_26, (uint)enDispenseNo.DISPENSE_27, (uint)enDispenseNo.DISPENSE_28, (uint)enDispenseNo.DISPENSE_29, (uint)enDispenseNo.DISPENSE_30,
            (uint)enDispenseNo.DISPENSE_31, (uint)enDispenseNo.DISPENSE_32,                                                                  
        };

        

        #region ViewModelMessage
        public static PopUpModuleTypeCls STModulePopUp = new PopUpModuleTypeCls();
        public static PopUpMessageCls STMessagePopUp = new PopUpMessageCls();
        public static PopUpRecipeCls STRecipePopUp = new PopUpRecipeCls();
        public static PopUpArmPositionCls STArmPositionPopUp = new PopUpArmPositionCls();
        public static PopUpDispenseCls STDispensePopUp = new PopUpDispenseCls();
        public static SelectModuleCls STSelectModuleMessage = new SelectModuleCls();
        public static PopUpUserRegistCls STUserRegistMessage = new PopUpUserRegistCls();
        public static TeachModuleMessageCls STTeachModuleMessage = new TeachModuleMessageCls();
        public static MotorIOMessageCls STMotorIODataMessage = new MotorIOMessageCls();
        public static DateMessageCls STDateMessage = new DateMessageCls();
        public static TeachingDataMessageCls STTeachingMessage = new TeachingDataMessageCls();
        #endregion

        public static int GetDispenseIndex(uint value)
        {
            int returnValue = -1;
            for (int i = 0; i < STDispenseIndex.Length; i++)
            {
                if (value == STDispenseIndex[i]) { returnValue = i + 1; break; }
            }

            return returnValue;
        }

        
        public static bool MessageOpen(enMessageType msgType, string message)
        {
            bool isReturn = false;
            //Alarm.AlarmMessage alarmMessage = null; ;
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                STAlarmMessage = new Alarm.AlarmMessage();
                STAlarmMessage.Owner = (MainWindow)System.Windows.Application.Current.MainWindow;
                STMessagePopUp.MessageType = msgType;
                STMessagePopUp.Message = message;
                Messenger.Default.Send(STMessagePopUp);
                STAlarmMessage.ShowDialog();
                if (STAlarmMessage.DialogResult.HasValue && STAlarmMessage.DialogResult.Value) isReturn = true;
            });
            
            return isReturn;
        }

        public static void MessageClose()
        {
            if (STAlarmMessage != null) CommonServiceLocator.ServiceLocator.Current.GetInstance<AlarmMessageViewModel>().OKCommand((Window)STAlarmMessage);
        }

        public static bool KeyBoard(ref string value)
        {
            bool reValue = false;

            Pad.KeyBoard keyBoard = new Pad.KeyBoard(value);
            keyBoard.Owner = (MainWindow)System.Windows.Application.Current.MainWindow;
            keyBoard.ShowDialog();

            if (keyBoard.DialogResult.HasValue && keyBoard.DialogResult.Value)
            {
                value = keyBoard.totalValue;
                reValue = true;
            }
            keyBoard = null;
            return reValue;
        }

        public static bool GetModuleOpen(string type, WaferStepCls waferStep = null)
        {
            STModulePopUp.Clear();
            STModulePopUp.ModuleType = type;
            STModulePopUp.waferStep = waferStep;
            View.Recipe.SelectModule selectModule = new View.Recipe.SelectModule();
            selectModule.Owner = (MainWindow)System.Windows.Application.Current.MainWindow;
            Messenger.Default.Send(Global.STModulePopUp);
            selectModule.ShowDialog();
            if (selectModule.DialogResult.HasValue && selectModule.DialogResult.Value) return true;
            return false;
        }

        public static bool GetModuleOpen(string type, int moduleno = 0, int blockno = 2)
        {
            STModulePopUp.Clear();
            STModulePopUp.ModuleType = type;
            STModulePopUp.BlockNo = blockno;
            STModulePopUp.ModuleNo = moduleno;
            View.Recipe.SelectModule selectModule = new View.Recipe.SelectModule();
            selectModule.Owner = (MainWindow)System.Windows.Application.Current.MainWindow;
            Messenger.Default.Send(Global.STModulePopUp);
            selectModule.ShowDialog();
            if (selectModule.DialogResult.HasValue && selectModule.DialogResult.Value) return true;
            return false;
        }

        public static bool SystemControlTargetOpen(string type, string contTarget, int moduleno = 0, int blockno = 2)
        {
            STModulePopUp.Clear();
            STModulePopUp.ModuleType = type;
            STModulePopUp.ControlTarget = contTarget;
            STModulePopUp.BlockNo = blockno;
            STModulePopUp.ModuleNo = moduleno;
            View.Recipe.SelectModule selectModule = new View.Recipe.SelectModule();
            selectModule.Owner = (MainWindow)System.Windows.Application.Current.MainWindow;
            Messenger.Default.Send(Global.STModulePopUp);
            selectModule.ShowDialog();
            if (selectModule.DialogResult.HasValue && selectModule.DialogResult.Value) return true;
            return false;
        }

        public static bool DispenseInfoOpen(enDispenseModule moduleType, uint value, string type = "RECIPEUSE", bool isMultiSelect = true)
        {
            STDispensePopUp.ModuleType = moduleType;
            STDispensePopUp.DispenseValue = value;
            STDispensePopUp.SelectDispenseValue = 0;
            STDispensePopUp.DummyOrRecipeUse = type;
            STDispensePopUp.IsMultiSelect = isMultiSelect;
            View.Recipe.SelectDispense dispenseInfo = new View.Recipe.SelectDispense();
            dispenseInfo.Owner = (MainWindow)System.Windows.Application.Current.MainWindow;
            Messenger.Default.Send(Global.STDispensePopUp);
            dispenseInfo.ShowDialog();
            if (dispenseInfo.DialogResult.HasValue && dispenseInfo.DialogResult.Value) return true;
            return false;
        }

        public static bool ArmPositionOpen(enArmTpe armType, string armDisp)
        {
            STArmPositionPopUp.ArmType = armType;
            STArmPositionPopUp.ArmPosition = armDisp;
            STArmPositionPopUp.SelectArmPosition = string.Empty;
            View.Recipe.ArmPosition armPosition = new View.Recipe.ArmPosition();
            armPosition.Owner = (MainWindow)System.Windows.Application.Current.MainWindow;
            Messenger.Default.Send(Global.STArmPositionPopUp);
            armPosition.ShowDialog();
            if (armPosition.DialogResult.HasValue && armPosition.DialogResult.Value) return true;
            return false;
        }

        public static bool RecipeOpen(enRecipeMenu recipeType, string recipeName = "")
        {
            STRecipePopUp.RecipeMenu = recipeType;
            STRecipePopUp.RecipeName = recipeName;
            STRecipePopUp.SelectRecipeName = string.Empty;
            View.Recipe.SelectRecipe selRecipe = new View.Recipe.SelectRecipe();
            selRecipe.Owner = (MainWindow)System.Windows.Application.Current.MainWindow;
            Messenger.Default.Send(Global.STRecipePopUp);            
            selRecipe.ShowDialog();
            if (selRecipe.DialogResult.HasValue && selRecipe.DialogResult.Value) return true;
            return false;
        }

        public static bool JogTeachingOpen(double position, AxisInfoCls axis = null)
        {
            STTeachingMessage.Position = position;
            STTeachingMessage.Axis = axis;
            View.Jog.JogControl jog = new View.Jog.JogControl();
            jog.Owner = (MainWindow)System.Windows.Application.Current.MainWindow;
            Messenger.Default.Send(STTeachingMessage);
            jog.ShowDialog();
            if (jog.DialogResult.HasValue && jog.DialogResult.Value) return true;
            return false;
        }

        public static bool GetDateOpen(string date)
        {
            STDateMessage.Date = Convert.ToDateTime(date);
            View.Maint.SelSupportDate selDate = new View.Maint.SelSupportDate();
            selDate.Owner = (MainWindow)System.Windows.Application.Current.MainWindow;
            Messenger.Default.Send(Global.STDateMessage);
            selDate.ShowDialog();
            if (selDate.DialogResult.HasValue && selDate.DialogResult.Value) return true;
            return false;
        }

        public static float KeyPad(float value = 0)
        {
            float reValue = value;
            Pad.KeyPad keyPad = new Pad.KeyPad(value);
            keyPad.Owner = (MainWindow)System.Windows.Application.Current.MainWindow;
            keyPad.ShowDialog();

            if (keyPad.DialogResult.HasValue && keyPad.DialogResult.Value)
            {
                reValue = keyPad.totalValue;
            }
            keyPad = null;
            return reValue;
        }

        public static void GetDirectoryFile(string dirPath, ref ObservableCollection<DirFileListCls> list,string extension = ".csv")
        {
            list.Clear();
            DirectoryInfo di = new DirectoryInfo(dirPath);
            if (!di.Exists) di.Create();
            int index = 1;
            foreach(FileInfo file in di.GetFiles())
            {
                if (file.Extension.ToLower().CompareTo(extension.ToLower()) == 0)
                {
                    DirFileListCls dirfile = new DirFileListCls();
                    dirfile.Index = index++;
                    dirfile.FileName = Path.GetFileNameWithoutExtension(file.Name);
                    dirfile.FileExtension = Path.GetExtension(file.Name);
                    dirfile.FileFullName = file.FullName;
                    dirfile.FilePath = file.DirectoryName + @"\";
                    list.Add(dirfile);
                }
                    
            }
        }

        //현재 웨이퍼 레시피에서 알수 있는게 FullName 밖에 없다.
        public static string GetModuleFullNameToName(string fullName)
        {
            string returnName = string.Empty;

            foreach(ModuleBaseCls module in Global.STModuleList)
            {
                if(module.MachineFullName == fullName)
                {
                    returnName = module.MachineName;
                    break;
                }
            }

            return returnName;
        }

        public static ModuleBaseCls GetModule(int blockNo, int moduleNo)
        {
            ModuleBaseCls module = null;

            foreach (ModuleBaseCls mod in Global.STModuleList)
            {
                if (mod.BlockNo == blockNo && mod.ModuleNo == moduleNo)
                {
                    module = mod;
                    break;
                }
            }

            return module;
        }
    }
}
