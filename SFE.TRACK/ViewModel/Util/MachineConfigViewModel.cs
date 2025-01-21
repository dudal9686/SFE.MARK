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
using CoreCSMac;
using MachineDefine;

namespace SFE.TRACK.ViewModel.Util
{
    public class MachineConfigViewModel : ViewModelBase
    {
        Model.ModuleBaseCls ModuleInfo_ { get; set; }
        public RelayCommand SaveRelayCommand { get; set; }
        List<string> arChamberInfo = new List<string>();
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
            List<ModuleBaseCls> arList = null;
            arChamberInfo.Clear();
            if (Global.STDataAccess.SaveModuleData())
            {
                arList = Global.STModuleList.FindAll(x => x.MachineName.IndexOf("COT") != -1).OrderBy(x => x.ModuleNo).ToList();

                for(int i = 0; i < arList.Count; i++)
                {
                    ModuleBaseCls module = arList[i];
                    arChamberInfo.Add(string.Format("{0}, {1}, {2}, {3}", i, module.BlockNo, module.ModuleNo, module.Use));
                }

                PrgCfgItem item = Global.MachineWorker.Reader.GetConfigItem(EnumConfigGroup.Environment, EnumConfig_Environment.CoaterInfo);
                item.SetValue(arChamberInfo);
                arChamberInfo.Clear();

                arList = Global.STModuleList.FindAll(x => x.MachineName.IndexOf("DEV") != -1).OrderBy(x => x.ModuleNo).ToList();

                for (int i = 0; i < arList.Count; i++)
                {
                    ModuleBaseCls module = arList[i];
                    arChamberInfo.Add(string.Format("{0}, {1}, {2}, {3}", i, module.BlockNo, module.ModuleNo, module.Use));
                }

                item = Global.MachineWorker.Reader.GetConfigItem(EnumConfigGroup.Environment, EnumConfig_Environment.DeveloperInfo);
                item.SetValue(arChamberInfo);
                arChamberInfo.Clear();

                arList = Global.STModuleList.FindAll(x => x.ModuleType == enModuleType.CHAMBER).OrderBy(x => x.ModuleNo).ToList();

                for (int i = 0; i < arList.Count; i++)
                {
                    ModuleBaseCls module = arList[i];
                    arChamberInfo.Add(string.Format("{0}, {1}, {2}, {3}, {4}", i, module.MachineName, module.BlockNo, module.ModuleNo, module.Use));
                }

                item = Global.MachineWorker.Reader.GetConfigItem(EnumConfigGroup.Environment, EnumConfig_Environment.ChamberInfo);
                item.SetValue(arChamberInfo);
                arChamberInfo.Clear();

                Global.MessageOpen(enMessageType.OK, "It has been saved.");
            }
            else Global.MessageOpen(enMessageType.OK, "Not saved.");
        }
    }
}
