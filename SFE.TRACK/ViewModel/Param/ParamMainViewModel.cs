using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SFE.TRACK.Model;
namespace SFE.TRACK.ViewModel.Param
{
    public class ParamMainViewModel : ViewModelBase
    {
        List<ParamMenuCls> teachingTypeList = new List<ParamMenuCls>();
        ObservableCollection<ParamModuleCls> teachingModuleList = new ObservableCollection<ParamModuleCls>();
        public RelayCommand<object> TeachDataDoubleClickRelayCommand { get; set; }
        public RelayCommand SaveTeachingRelayCommand { get; set; }
        List<TeachingDataCls> teachingData { get; set; }
        int selectedModuleIndex = 0;
        int selectedTeachingedIndex = 0;
        ParamModuleCls paramModule { get; set; }
        TeachingDataCls paramData { get; set; }
        string teachingName = string.Empty;
        string teachingGroupName = string.Empty;

        public ParamMainViewModel()
        {
            List<string> list = new List<string>();
            foreach (TeachingDataCls data in Global.STTeachingData)
            {
                if (!list.Contains(data.MainTitle)) list.Add(data.MainTitle);
            }

            foreach(string Name in list)
            {
                ParamMenuCls param = new ParamMenuCls();
                param.Title = Name;
                teachingTypeList.Add(param);
            }

            list.Clear();

            Messenger.Default.Register<TeachModuleMessageCls>(this, OnReceiveMessageAction);
            SaveTeachingRelayCommand = new RelayCommand(SaveTeachingCommand);
            TeachDataDoubleClickRelayCommand = new RelayCommand<object>(TeachDataDoubleClickCommand);
        }
        ~ParamMainViewModel()
        {

        }

        private void OnReceiveMessageAction(TeachModuleMessageCls o)
        {
            Console.WriteLine(o.Name);

            List<TeachingDataCls> list = Global.STTeachingData.FindAll(x => x.MainTitle == o.Name);
            TeachingModuleList.Clear();

            //COT, DEV 는 모터 타입으로 나뉜다.
            if (o.Name.IndexOf("COT") != -1 || o.Name.IndexOf("DEV") != -1)
            {
                foreach (TeachingDataCls data in list)
                {
                    if (!ModuleCompareList(data.Motor.MyNameInfo.Name))
                    {
                        ParamModuleCls param = new ParamModuleCls();
                        param.BlockNo = data.BlockNo;
                        param.ModuleNo = data.ModuleNo;
                        param.ModuleName = data.Motor.MyNameInfo.Name;
                        TeachingModuleList.Add(param);
                    }
                }
            }
            else
            {
                foreach (TeachingDataCls data in list)
                {
                    if (!ModuleCompareList(data.ModuleName))
                    {
                        ParamModuleCls param = new ParamModuleCls();
                        param.BlockNo = data.BlockNo;
                        param.ModuleNo = data.ModuleNo;
                        param.ModuleName = data.ModuleName;
                        TeachingModuleList.Add(param);
                    }
                }
            }

            TeachingGroupName = o.Name;

            if (TeachingModuleList.Count != 0)
            {
                ParamModule = TeachingModuleList[0];
                SelectedModuleIndex = 0;
            }
        }

        private bool ModuleCompareList(string name)
        {
            bool isFind = false;
            foreach(ParamModuleCls param in TeachingModuleList)
            {
                if(param.ModuleName == name)
                {
                    isFind = true;
                    break;
                }
            }

            return isFind;
        }

        public string TeachingName
        {
            get { return teachingName; }
            set { teachingName = value; RaisePropertyChanged("TeachingName"); }
        }

        public string TeachingGroupName
        {
            get { return teachingGroupName; }
            set { teachingGroupName = value; RaisePropertyChanged("TeachingGroupName"); }
        }

        private void GetTeachingData()
        {
            //COT, DEV는 모터 타입으로 나뉜다.
            if(Global.STTeachModuleMessage.Name.IndexOf("COT") != -1 || Global.STTeachModuleMessage.Name.IndexOf("DEV") != -1)
                TeachingData = Global.STTeachingData.FindAll(x => x.Motor.MyNameInfo.Name == TeachingName && x.BlockNo == ParamModule.BlockNo && x.ModuleNo == ParamModule.ModuleNo);
            else 
                TeachingData = Global.STTeachingData.FindAll(x => x.MainTitle == Global.STTeachModuleMessage.Name && x.BlockNo == ParamModule.BlockNo && x.ModuleNo == ParamModule.ModuleNo);
            
            if(TeachingData.Count != 0)
            {
                ParamData = TeachingData[0];
                SelectedTeachingedIndex = 0;
            }
        }

        public List<ParamMenuCls> TeachingTypeList
        {
            get { return teachingTypeList; }
            set { teachingTypeList = value; RaisePropertyChanged("TeachingTypeList"); }
        }
        public ObservableCollection<ParamModuleCls> TeachingModuleList
        {
            get { return teachingModuleList; }
            set { teachingModuleList = value; RaisePropertyChanged("TeachingModuleList"); }
        }
        public ParamModuleCls ParamModule
        {
            get { return paramModule; }
            set { paramModule = value;
                if (ParamModule != null)
                {
                    TeachingName = paramModule.ModuleName;
                    GetTeachingData();                    
                }
                RaisePropertyChanged("ParamModule");
            }
        }
        public TeachingDataCls ParamData
        {
            get { return paramData; }
            set
            {
                paramData = value;                
                RaisePropertyChanged("ParamData");
            }
        }
        public int SelectedModuleIndex
        {
            get { return selectedModuleIndex; }
            set { selectedModuleIndex = value; RaisePropertyChanged("SelectedModuleIndex"); }
        }
        public int SelectedTeachingedIndex
        {
            get { return selectedTeachingedIndex; }
            set { selectedTeachingedIndex = value; RaisePropertyChanged("SelectedTeachingedIndex"); }
        }
        public List<TeachingDataCls> TeachingData
        {
            get { return teachingData; }
            set { teachingData = value; RaisePropertyChanged("TeachingData"); }
        }

        private void SaveTeachingCommand()
        {
            if (teachingData == null) return;

            foreach(TeachingDataCls data in TeachingData)
            {
                data.SetData();
            }

            Global.MessageOpen(enMessageType.OK, "Saved.");
        }

        private void TeachDataDoubleClickCommand(object o)
        {
            if (ParamData == null) return;
            DataGrid grid = o as DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;
            switch(index)
            {
                case 0:
                    AxisInfoCls axis = Global.STAxis.Find(x => x.AxisID == ParamData.Motor.MyNameInfo.Name);
                    if (axis == null) break;
                    if (Global.JogTeachingOpen(ParamData.Pos, axis))
                    {
                        ParamData.Pos = Global.STTeachingMessage.Position;
                    }
                    break;
                case 1:
                    break;
                case 2:
                    ParamData.Acc = Global.KeyPad();
                    break;
                case 3:
                    ParamData.Dec = Global.KeyPad();
                    break;
                case 4:
                    ParamData.Vel = Global.KeyPad();
                    break;
                case 5:                    
                    float pos = Convert.ToSingle(ParamData.Pos);
                    ParamData.Pos = Global.KeyPad(pos);
                    break;

            }
        }
    }

    public class ParamMenuCls : ViewModelBase
    {
        string title = string.Empty;
        string icon = string.Empty;
        bool isSelected = false;
        public RelayCommand ClickRelayCommand { get; set; }
        public ParamMenuCls()
        {
            ClickRelayCommand = new RelayCommand(ClickCommand);
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; RaisePropertyChanged("IsSelected"); }
        }
        public string Title
        {
            get { 
                return title; }
            set { title = value; 
                RaisePropertyChanged("Title"); }
        }

        public string ICon
        {
            get { return icon; }
            set { icon = value; RaisePropertyChanged("ICon"); }
        }
        private void ClickCommand()
        {
            Global.STTeachModuleMessage.Name = Title;
            Messenger.Default.Send(Global.STTeachModuleMessage);
        }
    }

    public class ParamModuleCls : ViewModelBase
    {
        int blockNo = 0;
        int moduleNo = 0;
        string moduleName = string.Empty;

        public int BlockNo
        {
            get { return blockNo; }
            set { blockNo = value; RaisePropertyChanged("BlockNo"); }
        }
        public int ModuleNo
        {
            get { return moduleNo; }
            set { moduleNo = value; RaisePropertyChanged("ModuleNo"); }
        }
        public string ModuleName
        {
            get { return moduleName; }
            set { moduleName = value; RaisePropertyChanged("ModuleName"); }
        }
    }
}
