using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SFE.TRACK.Model;

namespace SFE.TRACK.ViewModel.Util
{
    public class DispenseConfigViewModel : ViewModelBase
    {
        List<DispenseInfoCls> DispenseList_ { get; set; }
        public DispenseInfoCls DispenseInfo { get; set; }
        public List<ModuleBaseCls> ModuleList { get; set; }
        public ModuleBaseCls ModuleInfo { get; set; } = null;
        public RelayCommand SaveDispenseRelayCommand { get; set; }
        public RelayCommand<object> DispenseDetailDoubleClickRelayCommand { get; set; }
        int SelectedIndex_ = 0;
        int DispSelectedIndex_ = 0;

        public DispenseConfigViewModel()
        {
            SaveDispenseRelayCommand = new RelayCommand(SaveDispenseCommand);
            DispenseDetailDoubleClickRelayCommand = new RelayCommand<object>(DispenseDetailDoubleClickCommand);
            ModuleList = Global.STModuleList.FindAll(x => x.MachineName.IndexOf("DEV") != -1 || x.MachineName.IndexOf("COT") != -1 || x.MachineName.IndexOf("ADH") != -1);

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

        public int SelectedIndex
        {
            get { return SelectedIndex_; }
            set 
            { 
                SelectedIndex_ = value;
                if (SelectedIndex != -1)
                {
                    ModuleInfo = ModuleList[SelectedIndex];
                    DispenseList = ModuleInfo.DispenseList.ToList();
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
        private void SaveDispenseCommand()
        {
            if(ModuleInfo == null)
            {
                Global.MessageOpen(enMessageType.OK, "Please select a module.");
            }
            else
            {
                if (Global.STDataAccess.SaveDispenseInfo(ModuleInfo.BlockNo, ModuleInfo.ModuleNo)) Global.MessageOpen(enMessageType.OK, "It has been saved.");
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
