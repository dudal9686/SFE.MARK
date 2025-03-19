using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SFE.TRACK.Model;

namespace SFE.TRACK.ViewModel.Auto
{
    public class JobStartViewModel : ViewModelBase
    {
        public RelayCommand NewJobRelayCommand { get; set; }
        public RelayCommand DeleteJobRelayCommand { get; set; }
        public RelayCommand PreJobRelayCommand { get; set; }
        public RelayCommand HostJobRelayCommand { get; set; }
        public RelayCommand<Window> OKRelayCommand { get; set; }
        public RelayCommand<Window> CancelRelayCommand { get; set; }
        public RelayCommand<object> JobGridDoubleClickRelayCommand { get; set; }
        LotInfoCls lotInfo { get; set; }
        int selectedIndex = -1;
        
        public JobStartViewModel()
        {
            NewJobRelayCommand = new RelayCommand(NewJobCommand);
            DeleteJobRelayCommand = new RelayCommand(DeleteJobCommand);
            PreJobRelayCommand = new RelayCommand(PreJobCommand);
            HostJobRelayCommand = new RelayCommand(HostJobCommand);
            OKRelayCommand = new RelayCommand<Window>(OKCommand);
            CancelRelayCommand = new RelayCommand<Window>(CancelCommand);
            JobGridDoubleClickRelayCommand = new RelayCommand<object>(JobGridDoubleClickCommand);
        }

        public LotInfoCls LotInfo
        {
            get { return lotInfo; }
            set { lotInfo = value; RaisePropertyChanged("LotInfo"); }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged("SelectedIndex"); }
        }

        private void NewJobCommand()
        {
            string jobName = Global.STJobInfo.JobName;
            if (Global.KeyBoard(ref jobName))
            {
                Global.STJobInfo.JobName = jobName;
            }
        }

        private void DeleteJobCommand()
        {
            Global.STJobInfo.Clear();
        }

        private void PreJobCommand()
        {
            if(Global.RecipeOpen(enRecipeMenu.JOBINFO))
            {
                Global.STJobInfo.Clear();
                Global.STDataAccess.ReadJobInfoData(string.Format(@"C:\MachineSet\SFETrack\Recipe\JobInfo\{0}.sfe", Global.STRecipePopUp.SelectRecipeName));
            }
        }

        private void HostJobCommand()
        {
            
        }

        private void OKCommand(Window window)
        {
            if(Global.STJobInfo.JobName == string.Empty)
            {
                Global.MessageOpen(enMessageType.OK, "Please enter the JobName.");
                return;
            }

            if (Global.STJobInfo.LotInfoList[0].LotID == string.Empty)
            {
                Global.MessageOpen(enMessageType.OK, "Please enter the LotID.");
                return;
            }

            if (Global.STJobInfo.LotInfoList[0].RecipeName == string.Empty)
            {
                Global.MessageOpen(enMessageType.OK, "Please select a recipe file.");
                return;
            }

            if (Global.STJobInfo.LotInfoList[0].StartModuleList.Count == 0)
            {
                Global.MessageOpen(enMessageType.OK, "Please select a cassette.");
                return;
            }

            if (!SetWafer()) return;
            string filename = string.Format(@"C:\MachineSet\SFETrack\Recipe\JobInfo\JOB{0}.sfe", DateTime.Now.ToString("yyyyMMddHHmmss"));
            Global.STDataAccess.SaveJobInfo(filename);
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\JobInfo\", ref Global.JobInfoFileList, ".sfe");

            window.DialogResult = true;
        }

        private bool SetWafer()
        {
            bool isMatch = true;
            foreach(LotInfoCls lot in Global.STJobInfo.LotInfoList)
            {
                if (lot.LotID == string.Empty || lot.RecipeName == string.Empty) continue;

                for(int i = 0; i < lot.StartModuleList.Count; i++)
                {
                    FoupCls foup = Global.STModuleList.Find(x => x.ModuleNo == lot.StartModuleList[i] && x.ModuleType == enModuleType.FOUP) as FoupCls;
                    if (!foup.IsScan || !foup.Use)
                    {
                        Global.MessageOpen(enMessageType.OK, string.Format("The cassette ({0}) is unusable.", foup.MachineName));
                        isMatch = false;
                        break;
                    }
                    foreach(WaferCls wafer in foup.FoupWaferList)
                    {
                        if (!wafer.Use) continue;
                        if(wafer.Recipe.Name == string.Empty && wafer.WaferState == enWaferState.WAFER_EXIST)
                        {   
                            wafer.Recipe.Name = lot.RecipeName;
                        }

                        if(wafer.Recipe.Name != string.Empty && wafer.WaferState == enWaferState.WAFER_EXIST)
                        {
                            string[] arWaferList = Global.WaferFlowRecipeFileList.Select(x => x.FileName).ToArray();
                            if (!arWaferList.Contains(wafer.Recipe.Name))
                            {
                                Global.MessageOpen(enMessageType.OK, string.Format("This recipe is unavailable. ({0})", wafer.Recipe.Name));
                                isMatch = false;
                                break;
                            }
                        }

                        if (!isMatch) break;
                    }

                    if (!isMatch) break;
                }

                if (!isMatch) break;
            }

            return isMatch;
        }

        private void CancelCommand(Window window)
        {
            window.DialogResult = false;
        }

        private void JobGridDoubleClickCommand(object o)
        {
            if (SelectedIndex == -1) return;
            if (Global.STJobInfo.JobName == string.Empty)
            {
                Global.MessageOpen(enMessageType.OK, "Please enter the JobName.");
                return;
            }
            
            string value = string.Empty;

            System.Windows.Controls.DataGrid grid = o as System.Windows.Controls.DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            if(SelectedIndex != 0)
            {
                if (Global.STJobInfo.LotInfoList[SelectedIndex - 1].LotID == string.Empty) return;
            }

            switch(index)
            {
                case 0: //LotID
                    value = Global.STJobInfo.LotInfoList[SelectedIndex].LotID;
                    if (Global.KeyBoard(ref value)) Global.STJobInfo.LotInfoList[SelectedIndex].LotID = value;
                    break;
                case 1: //Recipe
                    if (Global.STJobInfo.LotInfoList[SelectedIndex].LotID == string.Empty)
                    {
                        Global.MessageOpen(enMessageType.OK, "Please enter the LotID.");
                        return;
                    }                    
                    value = Global.STJobInfo.LotInfoList[SelectedIndex].RecipeName;
                    if (Global.RecipeOpen(enRecipeMenu.WAFER_FLOW, value)) Global.STJobInfo.LotInfoList[SelectedIndex].RecipeName = Global.STRecipePopUp.SelectRecipeName;
                    break;
                case 2: //Module
                    if (Global.STJobInfo.LotInfoList[SelectedIndex].RecipeName == string.Empty)
                    {
                        Global.MessageOpen(enMessageType.OK, "Please select a recipe file.");
                        return;
                    }
                    View.Auto.CassetteSlot slot = new View.Auto.CassetteSlot();
                    slot.Owner = Application.Current.MainWindow;
                    CommonServiceLocator.ServiceLocator.Current.GetInstance<CassetteSlotViewModel>().SetValue(SelectedIndex);
                    slot.ShowDialog();
                    break;
            }
        }
    }
    
}
