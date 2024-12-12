using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SFE.TRACK.Model;


namespace SFE.TRACK.ViewModel.Recipe
{
    /// <summary>
    /// Wafer Process Recipe 에서만 사용하는 모듈 정보
    /// </summary>
    public class SelectModuleViewModel : ViewModelBase
    {
        
        private List<CheckModuleCls> ModuleList_ { get; set; } = new List<CheckModuleCls>();//ObservableCollection
        public RelayCommand<Window> OKRelayCommand { get; set; }
        public RelayCommand<Window> CancelRelayCommand { get; set; }
        public RelayCommand<object> ModuleDoubleClickRelayCommand { get; set; }
        public CheckModuleCls itemModule { get; set; }
        int SelectedIndex_ = -1;

        #region ViewModel에서 넘어 온 파라미터 설정
        WaferStepCls waferStep = null;
        string moduleType = string.Empty;
        #endregion

        public SelectModuleViewModel()
        {
            Messenger.Default.Register<PopUpModuleTypeCls>(this, OnReceiveMessageAction);
            OKRelayCommand = new RelayCommand<Window>(OKCommand);
            CancelRelayCommand = new RelayCommand<Window>(CancelCommand);
            ModuleDoubleClickRelayCommand = new RelayCommand<object>(ModuleDoubleClickCommand);
        }
        
        ~SelectModuleViewModel()
        {
            Messenger.Default.Unregister<PopUpModuleTypeCls>(this);
        }

        public List<CheckModuleCls> ModuleList
        {
            get { return ModuleList_; }
            set { ModuleList_ = value; RaisePropertyChanged("ModuleList"); }
        }

        public int SelectedIndex
        {
            get { return SelectedIndex_; }
            set 
            { 
                SelectedIndex_ = value; 
                RaisePropertyChanged("SelectedIndex");
            }
        }

        public CheckModuleCls ItemModule
        {
            get { return itemModule; }
            set { 
                itemModule = value;
                RaisePropertyChanged("ItemModule");
            }
        }

        private void OnReceiveMessageAction(PopUpModuleTypeCls obj)
        {
            //obj.Clear();
            moduleType = obj.ModuleType;
            waferStep = obj.waferStep;
            CheckModuleCls checkModule;
            bool isFind = false;
            ModuleList.Clear();
            int index = 1;
            switch(moduleType)
            {
                case "ALL":                    
                    List<ModuleBaseCls> list = Global.STModuleList.FindAll(x => x.BlockNo != 1 && x.ModuleNo != 0 && x.Use == true);
                    foreach (ModuleBaseCls modBase in list)
                    {
                        isFind = false;
                        foreach (CheckModuleCls check in ModuleList)
                        {
                            if (modBase.MachineFullName == check.Display)
                            {
                                isFind = true;
                                break;
                            }
                        }
                        if (!isFind)
                        {
                            checkModule = new CheckModuleCls();
                            checkModule.Index = index;
                            checkModule.Display = modBase.MachineFullName;
                            checkModule.BlockNo = modBase.BlockNo;
                            checkModule.ModuleNo = modBase.ModuleNo;
                            if (waferStep.Name == checkModule.Display) checkModule.IsCheck = true;
                            else checkModule.IsCheck = false;
                            index++;
                            ModuleList.Add(checkModule);
                        }
                    }
                    break;
                case "SYSTEM":
                case "DUMMY":
                    if(moduleType == "SYSTEM") list = Global.STModuleList.FindAll(x => x.BlockNo != 1 && x.ModuleNo != 0 && x.Use == true && 
                        (x.MachineName.IndexOf("DEV") != -1 || x.MachineName.IndexOf("COT") != -1));
                    else list = Global.STModuleList.FindAll(x => x.BlockNo != 1 && x.ModuleNo != 0 && x.Use == true &&
                        (x.MachineName.IndexOf("DEV") != -1 || x.MachineName.IndexOf("COT") != -1 || x.MachineName.IndexOf("ADH") != -1));
                    foreach (ModuleBaseCls modBase in list)
                    {
                        checkModule = new CheckModuleCls();
                        checkModule.Index = index;
                        checkModule.Display = string.Format("{0} {1}-{2}", modBase.MachineName, modBase.BlockNo, modBase.ModuleNo);
                        checkModule.BlockNo = modBase.BlockNo;
                        checkModule.ModuleNo = modBase.ModuleNo;
                        if (modBase.ModuleNo == obj.ModuleNo) checkModule.IsCheck = true;
                        checkModule.IsCheck = false;
                        index++;
                        ModuleList.Add(checkModule);
                    }
                    break;
                case "DUMMYLOOP":
                    checkModule = new CheckModuleCls();
                    checkModule.Index = 1;
                    checkModule.Display = "NO";
                    checkModule.IsCheck = false;
                    if (obj.ControlTarget == checkModule.Display) checkModule.IsCheck = true;
                    ModuleList.Add(checkModule);
                    checkModule = new CheckModuleCls();
                    checkModule.Index = 2;
                    checkModule.Display = "START";
                    checkModule.IsCheck = false;
                    if (obj.ControlTarget == checkModule.Display) checkModule.IsCheck = true;
                    ModuleList.Add(checkModule);
                    checkModule = new CheckModuleCls();
                    checkModule.Index = 3;
                    checkModule.Display = "END";
                    checkModule.IsCheck = false;
                    if (obj.ControlTarget == checkModule.Display) checkModule.IsCheck = true;
                    ModuleList.Add(checkModule);
                    break;
                case "SYSTEMCONTROL":
                    if (obj.ModuleNo == 0) break;
                    Model.ModuleBaseCls m = Global.GetModule(obj.BlockNo, obj.ModuleNo);
                    if(m.MachineName.IndexOf("DEV") != -1)
                    {
                        checkModule = new CheckModuleCls();
                        checkModule.Index = 1;
                        checkModule.Display = "Developer temp";
                        checkModule.IsCheck = false;
                        if (obj.ControlTarget == checkModule.Display) checkModule.IsCheck = true;
                        ModuleList.Add(checkModule);
                        checkModule = new CheckModuleCls();
                        checkModule.Index = 2;
                        checkModule.Display = "Spin rpm";
                        checkModule.IsCheck = false;
                        if (obj.ControlTarget == checkModule.Display) checkModule.IsCheck = true;
                        ModuleList.Add(checkModule);
                    }
                    else if(m.MachineName.IndexOf("COT") != -1)
                    {
                        checkModule = new CheckModuleCls();
                        checkModule.Index = 1;
                        checkModule.Display = "Resist temp";
                        checkModule.IsCheck = false;
                        if (obj.ControlTarget == checkModule.Display) checkModule.IsCheck = true;
                        ModuleList.Add(checkModule);
                        checkModule = new CheckModuleCls();
                        checkModule.Index = 2;
                        checkModule.Display = "Cup temp";
                        checkModule.IsCheck = false;
                        if (obj.ControlTarget == checkModule.Display) checkModule.IsCheck = true;
                        ModuleList.Add(checkModule);
                        checkModule = new CheckModuleCls();
                        checkModule.Index = 3;
                        checkModule.Display = "Cup humidity";
                        checkModule.IsCheck = false;
                        if (obj.ControlTarget == checkModule.Display) checkModule.IsCheck = true;
                        ModuleList.Add(checkModule);
                        checkModule = new CheckModuleCls();
                        checkModule.Index = 4;
                        checkModule.Display = "Spin rpm";
                        checkModule.IsCheck = false;
                        if (obj.ControlTarget == checkModule.Display) checkModule.IsCheck = true;
                        ModuleList.Add(checkModule);
                    }                
                    break;
                default:
                    list = Global.STModuleList.FindAll(x => x.MachineFullName == moduleType && x.Use == true);
                    foreach (ModuleBaseCls modBase in list)
                    {

                        checkModule = new CheckModuleCls();
                        checkModule.Index = index;
                        checkModule.Display = string.Format("{0}-{1}", modBase.BlockNo, modBase.ModuleNo);
                        checkModule.BlockNo = modBase.BlockNo;
                        checkModule.ModuleNo = modBase.ModuleNo;
                        checkModule.IsCheck = false;
                        index++;
                        ModuleList.Add(checkModule);
                    }                    
                    break;
            }
        }

        private void OKCommand(Window window)
        {
            int index = 0;
            if(moduleType == "ALL") waferStep.Name = GetModuleName();
            else if(moduleType == "SYSTEM" || moduleType == "DUMMY")
            {
                for (int i = 0; i < ModuleList.Count; i++)
                {
                    CheckModuleCls checkMod = ModuleList[i] as CheckModuleCls;
                    if (checkMod.IsCheck)
                    {
                        Global.STModulePopUp.BlockNo = checkMod.BlockNo;
                        Global.STModulePopUp.ModuleNo = checkMod.ModuleNo;
                        index++;
                    }
                }

                if (index == 0) return;
            }
            else if (moduleType == "SYSTEMCONTROL" || moduleType == "DUMMYLOOP")
            {
                for (int i = 0; i < ModuleList.Count; i++)
                {
                    CheckModuleCls checkMod = ModuleList[i] as CheckModuleCls;
                    if (checkMod.IsCheck)
                    {
                        Global.STModulePopUp.ControlTarget = checkMod.Display;
                        index++;
                        break;
                    }
                }

                if (index == 0) return;
            }           
            else
            {
                for (int i = 0; i < ModuleList.Count; i++) // CheckModuleCls checkMod in ModuleList)
                {
                    CheckModuleCls checkMod = ModuleList[i] as CheckModuleCls;
                    if (checkMod.IsCheck) index++;
                }

                if (index == 0) return;

                index = 0;

                waferStep.ModuleListDescription = string.Empty;
                for (int i = 0; i < ModuleList.Count; i++) // CheckModuleCls checkMod in ModuleList)
                {
                    CheckModuleCls checkMod = ModuleList[i] as CheckModuleCls;
                    if (checkMod.IsCheck)
                    {
                        waferStep.BlokNo = checkMod.BlockNo;
                        waferStep.ModuleNoList[index] = checkMod.ModuleNo;
                        waferStep.ModuleListDescription += "2-" + waferStep.ModuleNoList[index].ToString() + ", ";
                        index++;
                    }
                }

                if (waferStep.ModuleListDescription.Length > 0) waferStep.ModuleListDescription = waferStep.ModuleListDescription.Substring(0, waferStep.ModuleListDescription.Length - 2);
                waferStep.ModuleCount = index;
            }

            window.DialogResult = true;
        }

        private void CancelCommand(Window window)
        {
            window.DialogResult = false;
        }

        private void ModuleDoubleClickCommand(object o)
        {
            if (moduleType == "ALL" || moduleType == "SYSTEM" || moduleType == "SYSTEMCONTROL" || moduleType == "DUMMY")
            {
                if (SelectedIndex != -1)
                {
                    for (int i = 0; i < ModuleList.Count; i++)
                    {
                        CheckModuleCls checkModule = ModuleList[i] as CheckModuleCls;
                        if (SelectedIndex == i) checkModule.IsCheck = true;
                        else checkModule.IsCheck = false;
                    }
                }
            }
            else if (moduleType == "DUMMYLOOP")
            {
                for (int i = 0; i < ModuleList.Count; i++) // CheckModuleCls checkMod in ModuleList)
                {
                    CheckModuleCls checkMod = ModuleList[i] as CheckModuleCls;
                    if (itemModule == checkMod)
                    {
                        itemModule.IsCheck = itemModule.IsCheck.Equals(true) ? false : true;
                    }
                    else checkMod.IsCheck = false;
                }
            }
            else
            {
                if (SelectedIndex != -1)
                {
                    CheckModuleCls checkModule = ModuleList[SelectedIndex] as CheckModuleCls;
                    if(checkModule.IsCheck) checkModule.IsCheck = false;
                    else checkModule.IsCheck = true;
                }
            }

        }

        private string GetModuleName()
        {
            string modulename = string.Empty;
            foreach(CheckModuleCls checkMod in ModuleList)
            {
                if(checkMod.IsCheck)
                {
                    modulename = checkMod.Display;
                    break;
                }
            }

            return modulename;
        }
    }

    public class CheckModuleCls : ViewModelBase
    {
        int index = 0;
        string display = string.Empty;
        int blockno = 0;
        int moduleno = 0;
        bool isCheck = false;

        public int Index
        {
            get { return index; }
            set { index = value; RaisePropertyChanged("Index"); }
        }

        public string Display
        {
            get { return display; }
            set { display = value; RaisePropertyChanged("Display"); }
        }

        public int BlockNo
        {
            get { return blockno; }
            set { blockno = value; RaisePropertyChanged("BlockNo"); }
        }

        public int ModuleNo
        {
            get { return moduleno; }
            set { moduleno = value; RaisePropertyChanged("ModuleNo"); }
        }

        public bool IsCheck
        {
            get { return isCheck; }
            set { isCheck = value; RaisePropertyChanged("IsCheck"); }
        }
    }
}
