using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SFE.TRACK.Model;

namespace SFE.TRACK.ViewModel.Util
{
    public enum enDispenseEdit
    {
        PUMP,
        FLOW,
        AUTOSUPPLY,
        AUTODRAIN
    };
    public class DispenseConfigViewModel : ViewModelBase
    {
        List<DispenseInfoCls> DispenseList_ { get; set; }
        DispenseInfoCls dispenseInfo { get; set; }
        public List<DispenseInfoCls> ModuleList { get; set; } = new List<DispenseInfoCls>();
        public DispenseInfoCls ModuleInfo { get; set; } = null;
        public RelayCommand SaveDispenseRelayCommand { get; set; }
        public RelayCommand<object> DispenseDetailDoubleClickRelayCommand { get; set; }
        public RelayCommand PumpEditRelayCommand { get; set; }
        public RelayCommand FlowSensorEditRelayCommand { get; set; }
        public RelayCommand AutoSupplyEditRelayCommand { get; set; }
        public RelayCommand AutoDrainEditRelayCommand { get; set; }
        int SelectedIndex_ = 0;
        int DispSelectedIndex_ = 0;
        ObservableCollection<bool> isEnableDisp { get; set; }

        public DispenseConfigViewModel()
        {
            SaveDispenseRelayCommand = new RelayCommand(SaveDispenseCommand);
            isEnableDisp = new ObservableCollection<bool>();
            for (int i = 0; i < 4; i++) isEnableDisp.Add(false);

            PumpEditRelayCommand = new RelayCommand(PumpEditCommand);
            FlowSensorEditRelayCommand = new RelayCommand(FlowSensorEditCommand);
            AutoSupplyEditRelayCommand = new RelayCommand(AutoSupplyEditCommand);
            AutoDrainEditRelayCommand = new RelayCommand(AutoDrainEditCommand);

            DispenseDetailDoubleClickRelayCommand = new RelayCommand<object>(DispenseDetailDoubleClickCommand);
            //ModuleList = Global.STModuleList.FindAll(x => x.MachineName.IndexOf("DEV") != -1 || x.MachineName.IndexOf("COT") != -1 || x.MachineName.IndexOf("ADH") != -1);
            DispenseInfoCls info = new DispenseInfoCls(); info.Type = "COT"; ModuleList.Add(info);
            info = new DispenseInfoCls(); info.Type = "DEV"; ModuleList.Add(info);
            info = new DispenseInfoCls(); info.Type = "ADH"; ModuleList.Add(info);

            if (ModuleList.Count == 0) SelectedIndex = -1;
            else
            {
                ModuleInfo = ModuleList[0];
                SelectedIndex = 0;
            }
        }

        public List<DispenseInfoCls> DispenseList
        {
            get { return DispenseList_; }
            set { DispenseList_ = value; RaisePropertyChanged("DispenseList"); }
        }

        public DispenseInfoCls DispenseInfo
        {
            get { return dispenseInfo; }
            set { dispenseInfo = value;

                if (dispenseInfo != null)
                {
                    if (dispenseInfo.Type == "COT")
                    {
                        if(dispenseInfo.DispName.IndexOf("RESIST") != -1)
                        {
                            IsEnableDisp[(int)enDispenseEdit.PUMP] = true;
                            IsEnableDisp[(int)enDispenseEdit.FLOW] = false;
                            IsEnableDisp[(int)enDispenseEdit.AUTOSUPPLY] = false;
                            IsEnableDisp[(int)enDispenseEdit.AUTODRAIN] = false;
                        }
                        else
                        {
                            IsEnableDisp[(int)enDispenseEdit.PUMP] = false;
                            IsEnableDisp[(int)enDispenseEdit.FLOW] = true;
                            IsEnableDisp[(int)enDispenseEdit.AUTOSUPPLY] = true;
                            IsEnableDisp[(int)enDispenseEdit.AUTODRAIN] = false;
                        }
                    }
                    else if (dispenseInfo.Type == "DEV")
                    {
                        IsEnableDisp[(int)enDispenseEdit.PUMP] = false;
                        IsEnableDisp[(int)enDispenseEdit.FLOW] = true;
                        IsEnableDisp[(int)enDispenseEdit.AUTOSUPPLY] = true;
                        IsEnableDisp[(int)enDispenseEdit.AUTODRAIN] = false;
                    }
                    else if (dispenseInfo.Type == "ADH")
                    {
                        if (dispenseInfo.DispName.IndexOf("HMDS") != -1)
                        {
                            IsEnableDisp[(int)enDispenseEdit.PUMP] = false;
                            IsEnableDisp[(int)enDispenseEdit.FLOW] = true;
                            IsEnableDisp[(int)enDispenseEdit.AUTOSUPPLY] = true;
                            IsEnableDisp[(int)enDispenseEdit.AUTODRAIN] = false;
                        }
                        else
                        {
                            IsEnableDisp[(int)enDispenseEdit.PUMP] = false;
                            IsEnableDisp[(int)enDispenseEdit.FLOW] = false;
                            IsEnableDisp[(int)enDispenseEdit.AUTOSUPPLY] = false;
                            IsEnableDisp[(int)enDispenseEdit.AUTODRAIN] = false;
                        }
                    }
                }
                else 
                {
                    for (int i = 0; i < 4; i++) IsEnableDisp[i] = false;
                }

                RaisePropertyChanged("DispenseInfo");
            }
        }

        public ObservableCollection<bool> IsEnableDisp
        {
            get { return isEnableDisp; }
            set { isEnableDisp = value; RaisePropertyChanged("IsEnableDisp"); }
        }

        public int SelectedIndex
        {
            get { return SelectedIndex_; }
            set 
            { 
                SelectedIndex_ = value;
                if (SelectedIndex != -1)
                {
                    ModuleInfo = ModuleList[SelectedIndex];
                    DispenseList = Global.STDispenseList.FindAll(x=>x.Type == ModuleInfo.Type).ToList();
                }

                RaisePropertyChanged("SelectedIndex"); 
            }
        }

        public int DispSelectedIndex
        {
            get { return DispSelectedIndex_; }
            set { DispSelectedIndex_ = value; RaisePropertyChanged("DispSelectedIndex"); }
        }

        #region command
        private void PumpEditCommand()
        {
            View.Util.EditPumpControl editPump = new View.Util.EditPumpControl();
            editPump.Owner = (MainWindow)System.Windows.Application.Current.MainWindow;
            CommonServiceLocator.ServiceLocator.Current.GetInstance<EditPumpControlViewModel>().DispenseInfo = DispenseInfo;
            editPump.ShowDialog();
        }

        private void FlowSensorEditCommand()
        {
            View.Util.EditFlowSensorControl flowSensor = new View.Util.EditFlowSensorControl();
            flowSensor.Owner = (MainWindow)System.Windows.Application.Current.MainWindow;
            CommonServiceLocator.ServiceLocator.Current.GetInstance<EditFlowSensorControlViewModel>().DispenseInfo = DispenseInfo;
            flowSensor.ShowDialog();
        }

        private void AutoSupplyEditCommand()
        {
            View.Util.EditAutoSupplyControl autoSupply = new View.Util.EditAutoSupplyControl();
            autoSupply.Owner = (MainWindow)System.Windows.Application.Current.MainWindow;
            CommonServiceLocator.ServiceLocator.Current.GetInstance<EditAutoSupplyControlViewModel>().DispenseInfo = DispenseInfo;
            autoSupply.ShowDialog();
        }

        private void AutoDrainEditCommand()
        {

        }

        private void SaveDispenseCommand()
        {
            if(ModuleInfo == null)
            {
                Global.MessageOpen(enMessageType.OK, "Please select a module.");
            }
            else
            {
                if (Global.STDataAccess.SaveDispenseInfo()) Global.MessageOpen(enMessageType.OK, "It has been saved.");
                else Global.MessageOpen(enMessageType.OK, "Not saved.");
            }
        }

        private void DispenseDetailDoubleClickCommand(object o)
        {
            DataGrid grid = o as DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            switch(index)
            {
                case 3:
                    if(Global.MessageOpen(enMessageType.OKCANCEL, "[Use]\n\n Would you like to change the properties?")) DispenseInfo.IsUse = DispenseInfo.IsUse.Equals(true) ? false : true;
                    break;
                case 4:
                    if (Global.MessageOpen(enMessageType.OKCANCEL, "[UseDummy]\n\n Would you like to change the properties?"))  DispenseInfo.IsUseDummy = DispenseInfo.IsUseDummy.Equals(true) ? false : true;
                    break;
                case 5:
                    if (Global.MessageOpen(enMessageType.OKCANCEL, "[UseRecipe]\n\n Would you like to change the properties?")) DispenseInfo.IsUseRecipe = DispenseInfo.IsUseRecipe.Equals(true) ? false : true;
                    break;
            }
        }
        #endregion

    }
}
