﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;

namespace SFE.TRACK.Model
{    
    public class ModuleBaseCls : ViewModelBase
    {
        float sizeX = 0;
        float sizeY = 0;
        float screenX = 0;
        float screenY = 0;
        float absolutePointX = 0;
        float absolutePointY = 0;        

        string machineDesc = string.Empty;
        string machineName = string.Empty;
        string machineTitle = string.Empty;
        string machineFullName = string.Empty;
        string machineMemo = string.Empty; //표시해야 할 것들, UI 에 보여준다.

        enModuleType moduleType = enModuleType.CHAMBER;
        enModuleState moduleState = enModuleState.NOTINITIAL;

        int blockNo = 0;
        int moduleNo = 0;

        bool use = true;
        bool isUseCRA = false; //챔버마다 사용할 암
        bool isUseIRA = false;
        bool isUsePRA = false;
        enMaintenanceMode maintMode = enMaintenanceMode.NONE;
        System.Windows.Media.SolidColorBrush moduleColor = new System.Windows.Media.SolidColorBrush();
        WaferCls wafer { get; set; }

        public RelayCommand ModuleClickRelayCommand { get; set; }

        public ObservableCollection<DispenseInfoCls> DispenseList { get; set; } = new ObservableCollection<DispenseInfoCls>();

        public ModuleBaseCls() 
        {
            wafer = new WaferCls();
            Wafer.WaferState = enWaferState.WAFER_NONE;
            moduleColor = System.Windows.Media.Brushes.DarkGray;            
            ModuleClickRelayCommand = new RelayCommand(ModuleClickCommand);
        }

        ~ModuleBaseCls() { }

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

        public bool Use
        {
            get { return use; }
            set { use = value; RaisePropertyChanged("Use"); }
        }

        public bool IsUseCRA
        {
            get { return isUseCRA; }
            set { isUseCRA = value; RaisePropertyChanged("IsUseCRA"); }
        }

        public bool IsUsePRA
        {
            get { return isUsePRA; }
            set { isUsePRA = value; RaisePropertyChanged("IsUsePRA"); }
        }

        public bool IsUseIRA
        {
            get { return isUseIRA; }
            set { isUseIRA = value; RaisePropertyChanged("IsUseIRA"); }
        }

        public enMaintenanceMode MaintMode
        {
            get { return maintMode; }
            set {
                maintMode = value;
                if (MaintMode == enMaintenanceMode.NONE) ModuleState = enModuleState.NONE;
                else ModuleState = enModuleState.MAINTENANCE;
                RaisePropertyChanged("MaintMode"); }
        }

        public float SizeX
        {
            get { return sizeX; }
            set { sizeX = value; RaisePropertyChanged("SizeX"); }
        }

        public float SizeY
        {
            get { return sizeY; }
            set { sizeY = value; RaisePropertyChanged("SizeY"); }
        }

        public float ScreenX
        {
            get { return screenX; }
            set { screenX = value; RaisePropertyChanged("ScreenX"); }
        }

        public float ScreenY
        {
            get { return screenY; }
            set { screenY = value; RaisePropertyChanged("ScreenY"); }
        }

        public string MachineDesc
        {
            get { return machineDesc; }
            set { machineDesc = value; RaisePropertyChanged("MachineDesc"); }
        }

        public string MachineName
        {
            get { return machineName; }
            set { machineName = value; RaisePropertyChanged("MachineName"); }
        }

        public string MachineTitle
        {
            get { return machineTitle; }
            set { machineTitle = value; RaisePropertyChanged("MachineTitle"); }
        }

        public string MachineFullName
        {
            get { return machineFullName; }
            set { machineFullName = value; RaisePropertyChanged("MachineFullName"); }
        }

        public string MachineMemo //나중에 Machine Status 에서 Display 할 내용
        {
            get { return machineMemo; }
            set { machineMemo = value; RaisePropertyChanged("MachineMemo"); }
        }

        public float AbsolutePointX
        {
            get { return absolutePointX; }
            set { absolutePointX = value; RaisePropertyChanged("AbsolutePointX"); }
        }

        public float AbsolutePointY
        {
            get { return absolutePointY; }
            set { absolutePointX = value; RaisePropertyChanged("AbsolutePointY"); }
        }

        public System.Windows.Media.SolidColorBrush ModuleColor
        {
            get { return moduleColor; }
            set { moduleColor = value; RaisePropertyChanged("ModuleColor"); }
        }

        public enModuleType ModuleType
        { 
            get { return moduleType; }
            set { moduleType = value; RaisePropertyChanged("ModuleType"); }
        }

        public enModuleState ModuleState
        {
            get { return moduleState; }
            set { moduleState = value; SetModuleColor(moduleState); RaisePropertyChanged("ModuleState"); }
        }

        public WaferCls Wafer
        {
            get { return wafer; }
            set { wafer = value; RaisePropertyChanged("Wafer"); }
        }

        public void SetModuleColor(enModuleState state)
        {
            switch(state)
            {
                case enModuleState.NOTINITIAL:
                    ModuleColor = System.Windows.Media.Brushes.DarkGray;
                    break;
                case enModuleState.STANDBY:
                    ModuleColor = System.Windows.Media.Brushes.Gray;
                    break;
                case enModuleState.OPERATING:
                    ModuleColor = System.Windows.Media.Brushes.AntiqueWhite;
                    break;
                case enModuleState.PREPROCESS:
                    ModuleColor = System.Windows.Media.Brushes.Green;
                    break;
                case enModuleState.IDLE:
                    ModuleColor = System.Windows.Media.Brushes.GreenYellow;
                    break;
                case enModuleState.PAUSE:
                    ModuleColor = System.Windows.Media.Brushes.Yellow;
                    break;
                case enModuleState.MAINTENANCE:
                    ModuleColor = System.Windows.Media.Brushes.MediumPurple;
                    break;
                case enModuleState.PROBLEM:
                    ModuleColor = System.Windows.Media.Brushes.Red;
                    break;
                case enModuleState.NONE:
                    ModuleColor = System.Windows.Media.Brushes.Navy;
                    break;
                default:
                    break;
            }
        }        

        private void ModuleClickCommand()
        {
            Global.STSelectModuleMessage.BlockNo = BlockNo;
            Global.STSelectModuleMessage.ModuleNo = ModuleNo;
            Messenger.Default.Send(Global.STSelectModuleMessage);
        }
    }
}