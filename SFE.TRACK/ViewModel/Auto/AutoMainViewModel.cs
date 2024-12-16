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

namespace SFE.TRACK.ViewModel.Auto
{
    public class AutoMainViewModel : ViewModelBase
    {
        public RelayCommand LotStartRelayCommand { get; set; }
        public RelayCommand JobStartRelayCommand { get; set; }
        public RelayCommand InitialRelayCommand { get; set; }
        public RelayCommand StopRelayCommand { get; set; }

        public AutoMainViewModel()
        {
            LotStartRelayCommand = new RelayCommand(LotStartCommand);
            JobStartRelayCommand = new RelayCommand(JobStartCommand);
            InitialRelayCommand = new RelayCommand(InitialCommand);
            StopRelayCommand = new RelayCommand(StopCommand);

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
            View.Auto.MotorInitail initMotor = new View.Auto.MotorInitail();
            initMotor.Owner = Application.Current.MainWindow;
            initMotor.ShowDialog();
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
            }

            jobList.Clear();
            waferDictionary.Clear();
        }
    }
}
