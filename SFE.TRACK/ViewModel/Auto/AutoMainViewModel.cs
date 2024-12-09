using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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

                if (!isStart) Global.MessageOpen(enMessageType.OK, "Please select a recipe");
            }
        }

        private void JobStartCommand()
        {
            Global.STJobInfo.Clear();
            foreach (WaferCls wafer in Global.STWaferList) wafer.Recipe.Name = string.Empty;
            
            View.Auto.JobStart jobStart = new View.Auto.JobStart();
            jobStart.Owner = Application.Current.MainWindow;
            jobStart.ShowDialog();
        }

        private void InitialCommand()
        {
            View.Auto.MotorInitail initMotor = new View.Auto.MotorInitail();
            initMotor.Owner = Application.Current.MainWindow;
            initMotor.ShowDialog();
        }

        private void StopCommand()
        {

        }
    }
}
