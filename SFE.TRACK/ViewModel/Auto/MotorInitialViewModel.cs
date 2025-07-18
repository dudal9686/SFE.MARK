﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using CoreCSBase.IPC;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SFE.TRACK.Model;
using MachineDefine;

namespace SFE.TRACK.ViewModel.Auto
{
    public class MotorInitialViewModel : ViewModelBase
    {
        public RelayCommand<Window> OKRelayCommand { get; set; }
        public RelayCommand<Window> CancelRelayCommand { get; set; }
        public RelayCommand<object> HomeCheckDoubleClickRelayCommand { get; set; }
        public RelayCommand<object> HomeCheckClickRelayCommand { get; set; }
        public RelayCommand AllCheckRelayCommand { get; set; }
        public ModuleBaseCls Module { get; set; }
        public List<ModuleBaseCls> ModuleList { get; set; }
        int selectedIndex = 0;
        bool check = false;
        Visibility isVisible = Visibility.Visible;

        public MotorInitialViewModel()
        {
            OKRelayCommand = new RelayCommand<Window>(OKCommand);
            CancelRelayCommand = new RelayCommand<Window>(CancelCommand);
            HomeCheckDoubleClickRelayCommand = new RelayCommand<object>(HomeCheckDoubleClickCommand);
            HomeCheckClickRelayCommand = new RelayCommand<object>(HomeCheckClickCommand);
            AllCheckRelayCommand = new RelayCommand(AllCheckCommand);

            ModuleList = Global.STModuleList.FindAll(x => (x.ModuleType == enModuleType.CHAMBER || x.ModuleType == enModuleType.SPINCHAMBER || x.ModuleType == enModuleType.CRA || x.ModuleType == enModuleType.PRA) && x.Use == true);
            //foreach (ModuleBaseCls module in ModuleList) module.HomeSituation = enHomeState.HOME_NONE;
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged("SelectedIndex"); }
        }

        private void OKCommand(Window window)
        {
            string message = string.Empty;
            //Global.SendCommand(IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move__OriginMove, moduleName);
            bool isRobot = false;
            foreach (Model.ModuleBaseCls module in ModuleList)
            {
                if (!module.IsHomeChecked) continue;
                message = string.Format("Module:{0},{1},{2}", module.MachineName, module.BlockNo, module.ModuleNo);
                Console.WriteLine(message);
                module.HomeSituation = enHomeState.HOMMING;
                module.ModuleState = enModuleState.HOMMING;
                if (module.ModuleNo == 0)
                {
                    if (!isRobot)
                    {
                        isRobot = true;
                        if (Global.MachineWorker.GetController("SFETrack").GetCurrentRunStatus() == CoreCSRunSim.RunStatus.EnumRunningStatus.IsRun) continue;
                        Global.SendCommand(Global.MCS_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Request__Initialize, "Do", true);
                        if(Global.MachineWorker.GetController("SFETrack").GetCurrentRunStatus() == CoreCSRunSim.RunStatus.EnumRunningStatus.IsIdle ||
                            Global.MachineWorker.GetController("SFETrack").GetCurrentRunStatus() == CoreCSRunSim.RunStatus.EnumRunningStatus.IsStop)
                        {
                            Global.MachineWorker.GetController("SFETrack").StartMachine();
                        }
                    }
                }
                else
                    Global.SendCommand(Global.CHAMBER_ID, IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move__ModuleOriginMove, message, true);
            
                foreach(AxisInfoCls axis in Global.STAxis)
                {
                    if (module.BlockNo == axis.BlockNo && module.ModuleNo == axis.ModuleNo) axis.HomeSituation = enHomeState.HOME_NONE;
                }
            }
            Global.STMachineStatus = enMachineStatus.HOME;
            window.DialogResult = true;
        }

        public Visibility IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; RaisePropertyChanged("IsVisible"); }
        }

        public void Close(Window window)
        {
            window.DialogResult = false;
        }

        private void CancelCommand(Window window)
        {
            window.DialogResult = false;
        }

        private void AllCheckCommand()
        {
            foreach (Model.ModuleBaseCls module in ModuleList)
            {
                module.IsHomeChecked = !check;

                //홈밍 트리거 신호

                //if (axis.HomeSituation == enHomeState.HOME_NONE) axis.HomeSituation = enHomeState.HOMMING;
                //else if (axis.HomeSituation == enHomeState.HOMMING) axis.HomeSituation = enHomeState.HOME_OK;
                //else if (axis.HomeSituation == enHomeState.HOME_OK) axis.HomeSituation = enHomeState.HOME_ERROR;
                //else if (axis.HomeSituation == enHomeState.HOME_ERROR) axis.HomeSituation = enHomeState.HOME_NONE;
            }
            check = !check;
        }

        private void HomeCheckClickCommand(object o)
        {
            System.Windows.Controls.DataGrid grid = o as System.Windows.Controls.DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            if (index == 0)
            {
                Module.IsHomeChecked = !Module.IsHomeChecked;
            }
        }

        private void HomeCheckDoubleClickCommand(object o)
        {
            System.Windows.Controls.DataGrid grid = o as System.Windows.Controls.DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            switch(index)
            {
                //case 0:
                case 1:
                case 2:
                    Module.IsHomeChecked = !Module.IsHomeChecked;
                    break;
                
            }
        }
    }
}
