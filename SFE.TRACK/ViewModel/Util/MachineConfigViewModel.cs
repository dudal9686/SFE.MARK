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
    public class MachineConfigViewModel : ViewModelBase
    {
        Model.ModuleBaseCls ModuleInfo_ { get; set; }
        public RelayCommand SaveRelayCommand { get; set; }
        public MachineConfigViewModel()
        {
            SaveRelayCommand = new RelayCommand(SaveCommand);
            Messenger.Default.Register<SelectModuleCls>(this, OnReceiveMessageAction);
        }

        ~MachineConfigViewModel()
        {
            Messenger.Default.Unregister<SelectModuleCls>(this, OnReceiveMessageAction);
        }

        public Model.ModuleBaseCls ModuleInfo
        {
            get { return ModuleInfo_; }
            set { ModuleInfo_ = value; RaisePropertyChanged("ModuleInfo"); }
        }

        private void OnReceiveMessageAction(SelectModuleCls o)
        {
            ModuleInfo = Global.GetModule(o.BlockNo, o.ModuleNo);
        }

        private void SaveCommand()
        {
            if (Global.STDataAccess.SaveModuleData()) Global.MessageOpen(enMessageType.OK, "It has been saved.");
            else Global.MessageOpen(enMessageType.OK, "Not saved.");
        }
    }
}
