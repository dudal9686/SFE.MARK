using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SFE.TRACK.Model;

namespace SFE.TRACK.ViewModel.Maint
{
    public class MaintChamberViewModel : ViewModelBase
    {
        List<ModuleBaseCls> ModuleList_ { get; set; }
        ModuleBaseCls ModuleInfo_ { get; set; }
        int SelectedIndex_ = -1;
        string MainTitle_ = string.Empty;
        public RelayCommand TempRelayCommand { get; set; }
        public RelayCommand RunRelayCommand { get; set; }
        public RelayCommand StopRelayCommand { get; set; }
        public RelayCommand AutoTuningRelayCommand { get; set; }
        public RelayCommand PinHomeRelayCommand { get; set; }
        public RelayCommand PinReadyRelayCommand { get; set; }
        public RelayCommand PinDownRelayCommand { get; set; }
        public RelayCommand PinUpRelayCommand { get; set; }
        public RelayCommand ServoOnRelayCommand { get; set; }
        public RelayCommand ServoOffRelayCommand { get; set; }
        public RelayCommand ShutterOpenRelayCommand { get; set; }
        public RelayCommand ShutterCloseRelayCommand { get; set; }
        public MaintChamberViewModel()
        {
            ModuleList = Global.STModuleList.FindAll(x => x.ModuleType == enModuleType.CHAMBER && x.Use == true).ToList();
            if (ModuleList.Count > 0)
            {
                ModuleInfo = ModuleList[0];
                SelectedIndex = 0;
            }

            TempRelayCommand = new RelayCommand(TempCommand);
            RunRelayCommand = new RelayCommand(RunCommand);
            StopRelayCommand = new RelayCommand(StopCommand);
            AutoTuningRelayCommand = new RelayCommand(AutoTuningCommand);
            PinHomeRelayCommand = new RelayCommand(PinHomeCommand);
            PinReadyRelayCommand = new RelayCommand(PinReadyCommand);
            PinDownRelayCommand = new RelayCommand(PinDownCommand);
            PinUpRelayCommand = new RelayCommand(PinUpCommand);
            ServoOnRelayCommand = new RelayCommand(ServoOnCommand);
            ServoOffRelayCommand = new RelayCommand(ServoOffCommand);
            ShutterOpenRelayCommand = new RelayCommand(ShutterOpenCommand);
            ShutterCloseRelayCommand = new RelayCommand(ShutterCloseCommand);
        }
        
        public List<ModuleBaseCls> ModuleList
        {
            get { return ModuleList_; }
            set { ModuleList_ = value; RaisePropertyChanged("ModuleList"); }
        }

        public ModuleBaseCls ModuleInfo
        {
            get { return ModuleInfo_; }
            set { ModuleInfo_ = value;
                MainTitle = string.Format("Data {0}-{1} {2}", ModuleInfo.BlockNo, ModuleInfo.ModuleNo, ModuleInfo.MachineName);
                RaisePropertyChanged("ModuleInfo"); }
        }

        public int SelectedIndex
        {
            get { return SelectedIndex_; }
            set { SelectedIndex_ = value;                
                RaisePropertyChanged("SelectedIndex"); }
        }

        public string MainTitle
        {
            get { return MainTitle_; }
            set { MainTitle_ = value; RaisePropertyChanged("MainTitle"); }
        }
        #region Event
        private void TempCommand()
        {

        }
        private void RunCommand()
        {

        }
        private void StopCommand()
        {

        }
        private void AutoTuningCommand()
        {

        }
        private void PinHomeCommand()
        {

        }
        private void PinReadyCommand()
        {

        }
        private void PinDownCommand()
        {

        }
        private void PinUpCommand()
        {

        }
        private void ServoOnCommand()
        {

        }
        private void ServoOffCommand()
        {

        }
        private void ShutterOpenCommand()
        {

        }
        private void ShutterCloseCommand()
        {

        }
        #endregion
    }
}
