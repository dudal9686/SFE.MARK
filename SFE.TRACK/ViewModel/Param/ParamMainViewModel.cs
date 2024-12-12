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
        int selecteModuleIndex = 0;
        int selectTeachingedIndex = 0;
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

            TeachingGroupName = o.Name;

            if (TeachingModuleList.Count != 0)
            {
                ParamModule = TeachingModuleList[0];
                selecteModuleIndex = 0;
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
            TeachingData = Global.STTeachingData.FindAll(x => x.MainTitle == Global.STTeachModuleMessage.Name && x.BlockNo == ParamModule.BlockNo && x.ModuleNo == ParamModule.ModuleNo);
            if(TeachingData.Count != 0)
            {
                ParamData = TeachingData[0];
                SelectTeachingedIndex = 0;
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
                    GetTeachingData();
                    TeachingName = paramModule.ModuleName;
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
        public int SelecteModuleIndex
        {
            get { return selecteModuleIndex; }
            set { selecteModuleIndex = value; RaisePropertyChanged("SelecteModuleIndex"); }
        }
        public int SelectTeachingedIndex
        {
            get { return selectTeachingedIndex; }
            set { selectTeachingedIndex = value; RaisePropertyChanged("SelectTeachingedIndex"); }
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
                    if(Global.JogTeachingOpen(ParamData.Pos, ParamData.Motor))
                    {
                        ParamData.Pos = Global.STTeachingMessage.Position;
                    }
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
