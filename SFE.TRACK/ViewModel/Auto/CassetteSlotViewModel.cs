using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using SFE.TRACK.Model;
using System.Collections.ObjectModel;

namespace SFE.TRACK.ViewModel.Auto
{
    public class CassetteSlotViewModel : ViewModelBase
    {
        
        List<WaferCls> waferList { get; set; }
        List<FoupCls> foupTempList { get; set; }
        public RelayCommand<string> SlotDetailRelayCommand { get; set; }
        public RelayCommand<string> SetSlotRelayCommand { get; set; }
        public RelayCommand<Window> OKRelayCommand { get; set; }
        public RelayCommand<Window> CancelRelayCommand { get; set; }
        public ObservableCollection<CstInfoCls> cstList { get; set; }
        List<int> BackUpList = new List<int>();
        CstInfoCls cstInfo { get; set; }
        int JobIndex = 0;
        public CassetteSlotViewModel()
        {
            WaferList = new List<WaferCls>();
            FoupTempList = new List<FoupCls>();
            SlotDetailRelayCommand = new RelayCommand<string>(SlotDetailCommand);
            SetSlotRelayCommand = new RelayCommand<string>(SetSlotCommand);
            OKRelayCommand = new RelayCommand<Window>(OKCommand);
            CancelRelayCommand = new RelayCommand<Window>(CancelCommand);
            cstList = new ObservableCollection<CstInfoCls>();
        }

        public ObservableCollection<CstInfoCls> CstList
        {
            get { return cstList; }
            set { cstList = value; RaisePropertyChanged("CstList"); }
        }

        public List<WaferCls> WaferList
        {
            get { return waferList; }
            set { waferList = value; RaisePropertyChanged("WaferList"); }
        }

        public List<FoupCls> FoupTempList
        {
            get { return foupTempList; }
            set { foupTempList = value; RaisePropertyChanged("FoupTempList"); }
        }

        public void SetValue(int jobIndex)
        {
            JobIndex = jobIndex;
            FoupTempList.Clear();
            WaferList.Clear();
            CstList.Clear();
            BackUpList.Clear();

            List<FoupCls> foupList = Global.STModuleList.FindAll(x => x.ModuleType == enModuleType.FOUP).Cast<FoupCls>().ToList();
            //메모리 문제
            foreach(WaferCls wafer in Global.STWaferList)
            {
                WaferList.Add(wafer.Clone());
            }

            foreach(FoupCls foup_ in foupList)
            {
                FoupCls cloneFoup = foup_.Clone();
                cloneFoup.FoupWaferList = WaferList.FindAll(x => x.ModuleNo == foup_.ModuleNo);
                FoupTempList.Add(cloneFoup);
            }

            foreach(int iValue in Global.STJobInfo.LotInfoList[jobIndex].StartModuleList)
            {
                cstInfo = new CstInfoCls(iValue);
                CstList.Add(cstInfo);
                BackUpList.Add(iValue);
            }
        }

        private void OKCommand(Window window)
        {
            SetStartDisplay();
            FoupTempList.Clear();
            WaferList.Clear();
            CstList.Clear();
            BackUpList.Clear();
            window.DialogResult = true;
        }

        private void CancelCommand(Window window)
        {
            Global.STJobInfo.LotInfoList[JobIndex].StartModuleList.Clear();
            foreach (int iVal in BackUpList) Global.STJobInfo.LotInfoList[JobIndex].StartModuleList.Add(iVal);
            SetStartDisplay();
            FoupTempList.Clear();
            WaferList.Clear();
            CstList.Clear();
            BackUpList.Clear();
            window.DialogResult = false;
        }


        private void SlotDetailCommand(string slot)
        {
            Console.WriteLine(slot);
            FoupCls foup = FoupTempList.Find(x => x.ModuleNo == Convert.ToInt32(slot));

            View.Auto.SlotDetail slotDetail = new View.Auto.SlotDetail();
            slotDetail.Owner = Application.Current.MainWindow;

            CommonServiceLocator.ServiceLocator.Current.GetInstance<SlotDetailViewModel>().SetValue(foup);
            slotDetail.ShowDialog();
            List<WaferCls> list = WaferList.FindAll(x => x.ModuleNo == Convert.ToInt32(slot));
        }

        private void SetSlotCommand(string slot)
        {
            FoupCls foup = FoupTempList.Find(x => x.ModuleNo == Convert.ToInt32(slot));            

            bool isFind = false;
            int slotIndex = 0;


            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < Global.STJobInfo.LotInfoList[i].StartModuleList.Count; j++)
                {
                    slotIndex = Global.STJobInfo.LotInfoList[i].StartModuleList[j];
                    if (slotIndex == Convert.ToInt32(slot))
                    {
                        isFind = true; //다른곳에서 사용중이다.
                        break;
                    }
                }
                
                if(isFind)
                {
                    if(i == JobIndex)
                    {
                        Global.STJobInfo.LotInfoList[i].StartModuleList.Remove(slotIndex);
                        foreach (CstInfoCls cst in CstList)
                        {
                            if (cst.CstNo == slotIndex)
                            {
                                CstList.Remove(cst);
                                break;
                            }
                        }

                        foreach (FoupCls foupCls in FoupTempList)
                        {
                            if (foupCls.ModuleNo != Convert.ToInt32(slot)) continue;
                            foreach (WaferCls waferCls in foupCls.FoupWaferList)
                            {
                                if (waferCls.WaferState == enWaferState.WAFER_EXIST)
                                {
                                    waferCls.Recipe.Name = string.Empty;
                                }
                            }
                        }
                    }
                }
            }

            if (!foup.IsScan) return;

            if (!isFind)
            {
                Global.STJobInfo.LotInfoList[JobIndex].StartModuleList.Add(Convert.ToInt32(slot));
                CstList.Add(new CstInfoCls(Convert.ToInt32(slot)));
                foreach(FoupCls foupCls in FoupTempList)
                {
                    if (foupCls.ModuleNo != Convert.ToInt32(slot)) continue;
                    foreach(WaferCls waferCls in foupCls.FoupWaferList)
                    {
                        if(waferCls.WaferState == enWaferState.WAFER_EXIST)
                        {
                            waferCls.Recipe.Name = Global.STJobInfo.LotInfoList[JobIndex].RecipeName;
                        }
                    }
                }
            }
        }

        private void SetStartDisplay()
        {
            Global.STJobInfo.LotInfoList[JobIndex].StartDisplay = string.Empty;
            for (int i = 0; i < Global.STJobInfo.LotInfoList[JobIndex].StartModuleList.Count; i++)
            {                
                Global.STJobInfo.LotInfoList[JobIndex].StartDisplay += Global.STJobInfo.LotInfoList[JobIndex].StartModuleList[i].ToString();
                if (i != Global.STJobInfo.LotInfoList[JobIndex].StartModuleList.Count - 1) Global.STJobInfo.LotInfoList[JobIndex].StartDisplay += ",";
                
            }
        }
    }

    public class CstInfoCls : ViewModelBase
    {
        int cstNo = 0;

        public CstInfoCls(int iValue)
        {
            CstNo = iValue;
        }
        public int CstNo
        {
            get { return cstNo; }
            set { cstNo = value; RaisePropertyChanged("CstNo"); }
        }
    }
}
