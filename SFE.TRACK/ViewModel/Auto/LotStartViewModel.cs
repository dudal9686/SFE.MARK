using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using SFE.TRACK.Model;
using System.Windows;

namespace SFE.TRACK.ViewModel.Auto
{
    public class LotStartViewModel : ViewModelBase
    {
        List<FoupCls> foupList { get; set; }
        List<FoupCls> foupTempList = new List<FoupCls>();
        FoupCls foup { get; set; }
        int selectedIndex = 0;
        public RelayCommand LoadRelayCommand { get; set; }
        public RelayCommand SetRecipeRelayCommand { get; set; }
        public RelayCommand SetLotNameRelayCommand { get; set; }
        public RelayCommand SetCommentRelayCommand { get; set; }
        public RelayCommand<Window> SetPilotProcessRelayCommand { get; set; }
        public RelayCommand<Window> OKRelayCommand { get; set; }
        public RelayCommand<Window> CanCelRelayCommand { get; set; }
        
        public LotStartViewModel()
        {
            FoupList = Global.STModuleList.FindAll(x => x.ModuleType == enModuleType.FOUP).Cast<FoupCls>().ToList();
            //FoupTempList = Global.STModuleList.FindAll(x => x.ModuleType == enModuleType.FOUP).Cast<FoupCls>().ToList();
            LoadRelayCommand = new RelayCommand(LoadCommand);
            SetRecipeRelayCommand = new RelayCommand(SetRecipeCommand);
            SetLotNameRelayCommand = new RelayCommand(SetLotNameCommand);
            SetCommentRelayCommand = new RelayCommand(SetCommentCommand);
            SetPilotProcessRelayCommand = new RelayCommand<Window>(SetPilotProcessCommand);
            OKRelayCommand = new RelayCommand<Window>(OKCommand);
            CanCelRelayCommand = new RelayCommand<Window>(CanCelCommand);
        }

        private void LoadCommand()
        {
            Foup = null;
            FoupTempList.Clear();

            foreach(FoupCls foupCls in FoupList)
            {
                foupCls.RecipeName = string.Empty;
                foupCls.Comment = string.Empty;
                foupCls.LotID = string.Empty;

                FoupCls foup_ = new FoupCls();
                foup_.BlockNo = foupCls.BlockNo;
                foup_.ModuleNo = foupCls.ModuleNo;
                foup_.FoupColor = foupCls.FoupColor;
                foup_.MachineDesc = foupCls.MachineDesc;
                foup_.MachineTitle = foupCls.MachineTitle;
                foup_.IsDetect = foupCls.IsDetect;
                foup_.IsScan = foupCls.IsScan;
                foup_.FoupWaferList = new List<WaferCls>();
                
                foreach (WaferCls wafer_ in foupCls.FoupWaferList)
                {
                    wafer_.Recipe.Name = "";
                    WaferCls wafer = new WaferCls();
                    wafer.WaferState = wafer_.WaferState;
                    wafer.Recipe.Name = wafer_.Recipe.Name;
                    wafer.BlockNo = wafer_.BlockNo;
                    wafer.ModuleNo = wafer_.ModuleNo;
                    wafer.Index = wafer_.Index;
                    foup_.FoupWaferList.Add(wafer);
                }

                FoupTempList.Add(foup_);
            }

            SelectedIndex = 0;
        }

        public FoupCls Foup
        {
            get { return foup; }
            set { foup = value; RaisePropertyChanged("Foup"); }
        }

        public List<FoupCls> FoupList
        {
            get { return foupList; }
            set { foupList = value; RaisePropertyChanged("FoupList"); }
        }

        public List<FoupCls> FoupTempList
        {
            get { return foupTempList; }
            set { foupTempList = value; RaisePropertyChanged("FoupTempList"); }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged("SelectedIndex"); }
        }

        private void SetRecipeCommand()
        {
            if (Foup == null) return;

            if (Foup.IsDetect == false)
            //if(Foup.IsScan == false)
            {
                Global.MessageOpen(enMessageType.OK, "Use after scanning the cassette!");
                return;
            }

            if(Global.RecipeOpen(enRecipeMenu.WAFER_FLOW))
            {
                Foup.RecipeName = Global.STRecipePopUp.SelectRecipeName;
                for (int i = 0; i < Foup.FoupWaferList.Count; i++)
                {
                    WaferCls wafer_ = Foup.FoupWaferList[i];
                    if (wafer_.WaferState != enWaferState.WAFER_EXIST) { wafer_.Recipe.Name = string.Empty; continue; }
                    wafer_.Recipe.Name = Foup.RecipeName;
                }
            }
        }
        private void SetLotNameCommand()
        {
            if (Foup == null) return;
            if (Foup.IsDetect == false)
            //if (Foup.IsScan == false)
            {
                Global.MessageOpen(enMessageType.OK, "Use after scanning the cassette!");
                return;
            }

            string lotID = Foup.LotID;
            if (Global.KeyBoard(ref lotID))
            {
                Foup.LotID = lotID;                
            }
        }
        private void SetCommentCommand()
        {
            if (Foup == null) return;
            if (Foup.IsDetect == false)
            //if (Foup.IsScan == false)
            {
                Global.MessageOpen(enMessageType.OK, "Use after scanning the cassette!");
                return;
            }

            string comment = Foup.Comment;
            if (Global.KeyBoard(ref comment))
            {
                Foup.Comment = comment;
            }
        }

        private void SetPilotProcessCommand(Window window)
        {
            if (Foup == null) return;
            if (Foup.IsDetect == false)
            //if (Foup.IsScan == false)
            {
                Global.MessageOpen(enMessageType.OK, "Use after scanning the cassette!");
                return;
            }

            if (Foup.RecipeName == string.Empty)
            {
                Global.MessageOpen(enMessageType.OK, "Please select a recipe.");
                return;
            }

            View.Auto.SlotDetail slot = new View.Auto.SlotDetail();
            CommonServiceLocator.ServiceLocator.Current.GetInstance<SlotDetailViewModel>().SetValue(Foup);
            slot.Owner = window;
            slot.ShowDialog();

            for(int i = 0; i < Foup.FoupWaferList.Count; i++)
            {
                Console.WriteLine(string.Format("Wafer index = {0}, JobName = {1}", Foup.FoupWaferList[i].Index, Foup.FoupWaferList[i].Recipe.Name));
            }

            
        }

        private void OKCommand(Window window)
        {
            for (int j = 0; j < FoupTempList.Count; j++)
            {
                if(FoupList[j].IsDetect && FoupList[j].IsScan)
                {
                    if(FoupTempList[j].RecipeName != string.Empty && FoupTempList[j].LotID == string.Empty)
                    {
                        Global.MessageOpen(enMessageType.OK, string.Format("Please enter the LotID of the cassette[{0}].", FoupList[j].ModuleNo));
                        return;
                    }
                }
            }

            Global.STJobInfo.Clear();
            Global.STJobInfo.JobName = string.Format("JOB{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string filename = string.Format(@"C:\MachineSet\SFETrack\Recipe\JobInfo\{0}.sfe", Global.STJobInfo.JobName);
            int index = 0;
            for (int i = 0; i < FoupList.Count; i++)
            {
                if (!FoupList[i].Use) continue;
                for (int j = 0; j < FoupTempList.Count; j++)
                {
                    if (FoupList[i].BlockNo == FoupTempList[j].BlockNo && FoupList[i].ModuleNo == FoupTempList[j].ModuleNo)
                    {
                        if (FoupList[i].IsDetect && FoupList[i].IsScan)
                        {
                            if (FoupTempList[j].RecipeName == string.Empty || FoupTempList[j].LotID == string.Empty)
                            {
                                continue;
                            }
                            FoupList[i].RecipeName = FoupTempList[j].RecipeName;
                            FoupList[i].Comment = FoupTempList[j].Comment;
                            FoupList[i].LotID = FoupTempList[j].LotID;

                            Global.STJobInfo.LotInfoList[index].RecipeName = FoupList[i].RecipeName;
                            Global.STJobInfo.LotInfoList[index].LotID = FoupList[i].LotID;
                            Global.STJobInfo.LotInfoList[index].Comment = FoupList[i].Comment;
                            Global.STJobInfo.LotInfoList[index].StartModuleList.Add(FoupList[i].ModuleNo);
                            Global.STJobInfo.LotInfoList[index].SModuleCount = 1;
                            Global.STJobInfo.LotInfoList[index].StartDisplay = FoupList[i].ModuleNo.ToString();

                            for (int k = 0; k < FoupList[i].FoupWaferList.Count; k++)
                            {
                                WaferCls tempWafer_ = FoupTempList[j].FoupWaferList.Find(x => x.ModuleNo == FoupList[i].FoupWaferList[k].ModuleNo && x.Index == FoupList[i].FoupWaferList[k].Index);
                                if (tempWafer_ != null)
                                {
                                    if (!FoupList[i].FoupWaferList[k].Use) continue;
                                    FoupList[i].FoupWaferList[k].WaferState = tempWafer_.WaferState;
                                    FoupList[i].FoupWaferList[k].Recipe.Name = tempWafer_.Recipe.Name;
                                    FoupList[i].FoupWaferList[k].LotNo = FoupList[i].LotID;
                                }
                            }

                            index++;
                        }
                    }
                }
            }

            if(index != 0)
            {
                Global.STDataAccess.SaveJobInfo(filename);
                Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\JobInfo\", ref Global.JobInfoFileList, ".sfe");
            }
            

            FoupTempList.Clear();
            window.DialogResult = true;
        }
        private void CanCelCommand(Window window)
        {
            FoupTempList.Clear();
            window.DialogResult = false;
        }

        public void SetIsDetect(int moduleNo)
        {
            FoupCls foup = Global.STModuleList.Find(x => x.ModuleNo == moduleNo && x.ModuleType == enModuleType.FOUP) as FoupCls;
            FoupCls foupTemp = FoupTempList.Find(x => x.ModuleNo == moduleNo) as FoupCls;

            if (foupTemp == null) return;
            foupTemp.IsDetect = foup.IsDetect;
            foupTemp.IsScan = foup.IsScan;
        }
    }
}
