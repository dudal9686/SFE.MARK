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
using MachineDefine;
using CoreCSRunSim;

namespace SFE.TRACK.ViewModel.Param
{
    public class ParamMainViewModel : ViewModelBase
    {
        List<ParamMenuCls> teachingTypeList = new List<ParamMenuCls>();
        List<ParamModuleCls> teachingModuleList = new List<ParamModuleCls>();
        public RelayCommand<object> TeachDataDoubleClickRelayCommand { get; set; }
        public RelayCommand SaveTeachingRelayCommand { get; set; }
        public RelayCommand MoveTeachingRelayCommand { get; set; }
        List<TeachingDataCls> teachingData { get; set; }
        int selectedModuleIndex = 0;
        int selectedTeachingedIndex = 0;
        ParamModuleCls paramModule { get; set; }
        TeachingDataCls paramData { get; set; }
        string teachingName = string.Empty;
        string teachingGroupName = string.Empty;
        AxisInfoCls axisInfo { get; set; }
        TSpeedPack speedPack = new TSpeedPack();
        public ParamMainViewModel()
        {
            ParamMenuCls param;
            List<string> list = new List<string>();
            foreach (TeachingDataCls data in Global.STTeachingData)
            {
                if (!list.Contains(data.MainTitle)) list.Add(data.MainTitle);
            }

            foreach(string Name in list)
            {
                param = new ParamMenuCls();
                param.Title = Name;
                teachingTypeList.Add(param);
            }

            list.Clear();

            Messenger.Default.Register<TeachModuleMessageCls>(this, OnReceiveMessageAction);
            SaveTeachingRelayCommand = new RelayCommand(SaveTeachingCommand);
            MoveTeachingRelayCommand = new RelayCommand(MoveTeachingCommand);
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
                TeachingModuleList = TeachingModuleList.OrderBy(x => x.ModuleNo).ToList();
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

        public AxisInfoCls Axis
        {
            get { return axisInfo; }
            set { axisInfo = value; RaisePropertyChanged("Axis"); }
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
        public List<ParamModuleCls> TeachingModuleList
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

                if (paramData != null)
                {
                    foreach (AxisInfoCls axis in Global.STAxis)
                    {
                        if (paramData.Motor.GetName() == axis.Motor.GetName())
                        {
                            Axis = axis;
                            if(Axis.Company == "SFE_CAN")
                                Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.StatusChange__MotorAxisRequest, string.Format("MOTOR:{0}", Axis.AxisID));
                            break;
                        }
                    }
                }

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

        private void MoveTeachingCommand()
        {
            if (paramData == null) return;

            if (!Global.MessageOpen(enMessageType.OKCANCEL, "Would you like to move?")) return;

            string command = string.Empty;
            Console.WriteLine(paramData.Motor.GetName());

            foreach(AxisInfoCls axis in Global.STAxis)
            {
                if(axis.Motor.GetName() == paramData.Motor.GetName())
                {
                    if (axis.Company == "SFE_CAN")
                    {
                        command = string.Format("Motor:{0},{1},{2},{3},{4},{5}", axis.AxisID, paramData.Pos, paramData.Vel, paramData.Acc, paramData.Dec, 10000);
                        Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move__DirectMove, command);
                    }
                    else
                    {
                        speedPack.acc = paramData.Acc * Global.STPulseToUnit;
                        speedPack.dec = paramData.Dec * Global.STPulseToUnit;
                        speedPack.speed = paramData.Vel * Global.STPulseToUnit;
                        speedPack.timeout = 5000;
                        axis.Motor.DoSCurveMove(paramData.Pos * Global.STPulseToUnit, speedPack, UnitMotor.EnumMovePosType.ABSOLUTE);
                    }
                }
            }
        }

        private void TeachDataDoubleClickCommand(object o)
        {
            if (ParamData == null) return;
            DataGrid grid = o as DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;
            switch(index)
            {
                case 0:
                case 1:
                    AxisInfoCls axis = Global.STAxis.Find(x => x.AxisID == ParamData.Motor.MyNameInfo.Name);
                    if (axis == null) break;
                    if (Global.JogTeachingOpen(ParamData.Pos, axis))
                    {
                        double positionValue = 0;
                        if (axis.Parent == "Chamber") positionValue = Global.STTeachingMessage.Position / 500;
                        else positionValue = Global.STTeachingMessage.Position;
                        ParamData.Pos = positionValue;
                    }
                    break;
                case 2:
                    ParamData.Acc = Global.KeyPad((float)ParamData.Acc);
                    break;
                case 3:
                    ParamData.Dec = Global.KeyPad((float)ParamData.Dec);
                    break;
                case 4:
                    ParamData.Vel = Global.KeyPad((float)ParamData.Vel);
                    break;
                case 5:                    
                    ParamData.Pos = Global.KeyPad((float)ParamData.Pos);
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
