using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CoreCSMac;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MachineDefine;
using SFE.TRACK.Model;
using System.Windows.Threading;
using System.Diagnostics;

namespace SFE.TRACK.ViewModel.Auto
{
    public class AutoMainViewModel : ViewModelBase
    {
        public RelayCommand LotStartRelayCommand { get; set; }
        public RelayCommand JobStartRelayCommand { get; set; }
        public RelayCommand InitialRelayCommand { get; set; }
        public RelayCommand StopRelayCommand { get; set; }
        public RelayCommand DummyRecipeRelayCommand { get; set; }
        public RelayCommand RecipeTransferRelayCommand { get; set; }
        public RelayCommand MonitoringRelayCommand { get; set; }
        public RelayCommand HomeRelayCommand { get; set; }
        DispatcherTimer timer = new DispatcherTimer();
        Stopwatch stopWatch = new Stopwatch();
        public AutoMainViewModel()
        {
            LotStartRelayCommand = new RelayCommand(LotStartCommand);
            JobStartRelayCommand = new RelayCommand(JobStartCommand);
            InitialRelayCommand = new RelayCommand(InitialCommand);
            HomeRelayCommand = new RelayCommand(HomeCommand);
            StopRelayCommand = new RelayCommand(StopCommand);
            DummyRecipeRelayCommand = new RelayCommand(DummyRecipeCommand);
            RecipeTransferRelayCommand = new RelayCommand(RecipeTransferCommand);
            MonitoringRelayCommand = new RelayCommand(MonitoringCommand);
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(500);
        }
        ~AutoMainViewModel()
        { }

        private void LotStartCommand()
        {
            bool isStart = false;
            View.Auto.LotStart lotStart = new View.Auto.LotStart();
            lotStart.Owner = Application.Current.MainWindow;
            lotStart.ShowDialog();

            if (lotStart.DialogResult.HasValue && lotStart.DialogResult.Value)
            {
                List<FoupCls> foupList = Global.STModuleList.FindAll(x => x.ModuleType == enModuleType.FOUP).Cast<FoupCls>().ToList();

                foreach(FoupCls foup in foupList)
                {
                    if(foup.IsDetect && foup.IsScan && foup.RecipeName != string.Empty)
                    {
                        isStart = true;
                        break;
                    }
                }

                if (!isStart)
                {
                    Global.MessageOpen(enMessageType.OK, "Please select a recipe");
                    return;
                }

                SendJobData();
            }
        }

        private void JobStartCommand()
        {
            Global.STJobInfo.Clear();
            foreach (WaferCls wafer in Global.STWaferList) wafer.Recipe.Name = string.Empty;
            
            View.Auto.JobStart jobStart = new View.Auto.JobStart();
            jobStart.Owner = Application.Current.MainWindow;
            jobStart.ShowDialog();

            if(jobStart.DialogResult.HasValue && jobStart.DialogResult.Value)
            {
                SendJobData();
            }
        }

        private void InitialCommand()
        {
            timer.Stop();
            if(Global.MessageOpen(enMessageType.OKCANCEL, "Would you like to initialize your equipment?"))
            {
                foreach (ModuleBaseCls moduleBase in Global.STModuleList)
                {
                    if (!moduleBase.Use || moduleBase.ModuleType == enModuleType.FOUP || moduleBase.MaintMode == enMaintenanceMode.MAINTMODE || moduleBase.MaintMode == enMaintenanceMode.MAINTMODE_USE)
                    {
                        moduleBase.HomeSituation = enHomeState.HOME_NONE;
                        foreach (AxisInfoCls axis in Global.STAxis)
                        {
                            if (moduleBase.BlockNo == axis.BlockNo && moduleBase.ModuleNo == axis.ModuleNo)
                                axis.HomeSituation = enHomeState.HOME_NONE;
                        }
                        continue; 
                    }
                    moduleBase.HomeSituation = enHomeState.HOMMING;
                    moduleBase.ModuleState = enModuleState.NOTINITIAL;
                    moduleBase.IsHomeChecked = true;

                    foreach (AxisInfoCls axis in Global.STAxis)
                    {
                        if (moduleBase.BlockNo == axis.BlockNo && moduleBase.ModuleNo == axis.ModuleNo)
                            axis.HomeSituation = enHomeState.HOMMING;
                    }
                }

                Global.MachineWorker.SendCommand(Global.MCS_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Request___Initialize, "Do", true);
                Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Request___Initialize, "Do", true);
                
                stopWatch.Stop();
                stopWatch.Start();
                timer.Start();

                //Global.MessageOpen(enMessageType.NONE, "Homming...");
            }
        }
        private void HomeCommand()
        {
            timer.Stop();
            foreach (ModuleBaseCls moduleBase in Global.STModuleList) moduleBase.IsHomeChecked = false;
            View.Auto.MotorInitail initMotor = new View.Auto.MotorInitail();
            initMotor.Owner = Application.Current.MainWindow;
            initMotor.ShowDialog();
            if (initMotor.DialogResult.HasValue && initMotor.DialogResult.Value)
            {
                stopWatch.Stop();
                stopWatch.Start();
                timer.Start();
            }
        }

        private void StopCommand()
        {
            View.Auto.StopControl stopView = new View.Auto.StopControl();
            stopView.Owner = Application.Current.MainWindow;
            stopView.ShowDialog();
            if(stopView.DialogResult.HasValue && stopView.DialogResult.Value)
            {
                //Command
            }
        }
        private void RecipeTransferCommand()
        {
            View.Auto.RecipeTransfer trans = new View.Auto.RecipeTransfer();
            trans.Owner = Application.Current.MainWindow;
            trans.ShowDialog();
        }
        private void MonitoringCommand()
        {
            View.Auto.DataMonitoring dataMonit = new View.Auto.DataMonitoring();
            dataMonit.ShowDialog();
        }
        private void DummyRecipeCommand()
        {
            View.Auto.RegistDummyLinkRecipe link = new View.Auto.RegistDummyLinkRecipe();
            link.Owner = Application.Current.MainWindow;
            link.ShowDialog();
        }
        private void SendJobData()
        {
            Dictionary<int, string> waferDictionary = new Dictionary<int, string>();
            List<string> jobList = new List<string>();
            PrgCfgItem item = Global.MachineWorker.Reader.GetConfigItem(EnumConfigGroup.Lot, EnumConfig_Lot.Job);
            item.SetValue(jobList);
            List<FoupCls> list = Global.STModuleList.FindAll(x => x.ModuleType == enModuleType.FOUP).Cast<FoupCls>().ToList();

            foreach(FoupCls foup in list)
            {
                if (!foup.Use || !foup.IsDetect || !foup.IsScan) continue;
                waferDictionary.Clear();
                bool isFind = false;
                foreach(WaferCls wafer in foup.FoupWaferList)
                {
                    if(!wafer.Use || wafer.Recipe.Name == string.Empty)
                    {
                        waferDictionary.Add(wafer.Index, "");
                    }
                    else
                    {
                        waferDictionary.Add(wafer.Index, wafer.Recipe.Name);
                        isFind = true;
                    }
                }

                if(isFind)
                {
                    string strJob = string.Empty;
                    strJob = string.Format("{0},", foup.ModuleNo);
                    foreach (KeyValuePair<int, string> item_ in waferDictionary)
                    {
                        strJob += item_.Value + ",";
                    }

                    strJob = strJob.Substring(0, strJob.Length - 1);
                    jobList.Add(strJob);
                }
            }

            if(jobList.Count > 0)
            {
                item = Global.MachineWorker.Reader.GetConfigItem(EnumConfigGroup.Lot, EnumConfig_Lot.Job);
                item.SetValue(jobList);

                Global.MachineWorker.SendCommand(Global.MCS_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Machine___Run, "Run");
                Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Machine___Run, "Run");
            }

            jobList.Clear();
            waferDictionary.Clear();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            bool isDone = true;
            bool isError = false;
            foreach (ModuleBaseCls moduleBase in Global.STModuleList)
            {
                if (moduleBase.ModuleType == enModuleType.FOUP) continue;

                if (stopWatch.ElapsedMilliseconds > Global.HOME_TIMEOUT)
                {
                    isDone = true;
                    isError = true;
                    break;
                }

                if (moduleBase.IsHomeChecked && moduleBase.Use)
                {
                    if (moduleBase.HomeSituation == enHomeState.HOME_OK) isDone = true;
                    else if (moduleBase.HomeSituation == enHomeState.HOMMING) isDone = false;
                    else if (moduleBase.HomeSituation == enHomeState.HOME_ERROR) isError = true;

                    if (!isDone) break;
                }
                if (!isDone) break;
                
            }
            if (isDone && !isError)
            {
                stopWatch.Stop();
                stopWatch.Reset();
                timer.Stop();
                Global.MessageOpen(enMessageType.OK, "Initialize Success");
            }
            else if(isDone && isError)
            {
                stopWatch.Stop();
                stopWatch.Reset();
                timer.Stop();
                Global.MessageOpen(enMessageType.OK, "Initialize Error");
            }
        }
    }
}
