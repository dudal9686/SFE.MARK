using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SFE.TRACK.Model;
using CoreCSMac;
using MachineDefine;

namespace SFE.TRACK.ViewModel.Util
{
    public class LampConfigViewModel : ViewModelBase
    {
        int selectedIndex = 0;
        public RelayCommand<object> LampDoubleClickRelayCommand { get; set; }
        public RelayCommand SaveLampRelayCommand { get; set; }
        public LampCls lampInfo { get; set; }
        public LampConfigViewModel()
        {
            LampDoubleClickRelayCommand = new RelayCommand<object>(LampDoubleClickCommand);
            SaveLampRelayCommand = new RelayCommand(SaveLampCommand);
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged("SelectedIndex"); }
        }

        private void SaveLampCommand()
        {
            if (Global.STLampList.Count == 0) return;

            List<string> valueList = new List<string>();
            
            int buzzerTimeOut = Global.STLampList[0].BuzzerTimeOut;
            valueList.Add(buzzerTimeOut.ToString());

            foreach(LampCls lamp in Global.STLampList)
            {
                string data = string.Empty;

                if (lamp.RedString == enLamp.ON.ToString()) data += "O";
                else if (lamp.RedString == enLamp.OFF.ToString()) data += "X";
                else  data += "T";

                if (lamp.YellowString == enLamp.ON.ToString()) data += "O";
                else if (lamp.YellowString == enLamp.OFF.ToString()) data += "X";
                else data += "T";

                if (lamp.GreenString == enLamp.ON.ToString()) data += "O";
                else if (lamp.GreenString == enLamp.OFF.ToString()) data += "X";
                else data += "T";

                if (lamp.BuzzerString == enBuzzer.ON.ToString()) data += "O";
                else  data += "X";

                valueList.Add(data);
            }

            PrgCfgItem prgItem = Global.MachineWorker.Reader.GetConfigItem(EnumConfigGroup.Environment, EnumConfig_Environment.TowerLamp);
            prgItem.SetValueAll(valueList);

            Global.MessageOpen(enMessageType.OK, "Saved.");
        }

        private void LampDoubleClickCommand(object o)
        {
            if (lampInfo == null) return;

            DataGrid grid = o as DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            switch(index)
            {
                case 1:
                    if (lampInfo.RedString == enLamp.ON.ToString()) lampInfo.RedString = enLamp.TOGGLE.ToString();
                    else if (lampInfo.RedString == enLamp.TOGGLE.ToString()) lampInfo.RedString = enLamp.OFF.ToString();
                    else if (lampInfo.RedString == enLamp.OFF.ToString()) lampInfo.RedString = enLamp.ON.ToString();
                    break;
                case 2:
                    if (lampInfo.YellowString == enLamp.ON.ToString()) lampInfo.YellowString = enLamp.TOGGLE.ToString();
                    else if (lampInfo.YellowString == enLamp.TOGGLE.ToString()) lampInfo.YellowString = enLamp.OFF.ToString();
                    else if (lampInfo.YellowString == enLamp.OFF.ToString()) lampInfo.YellowString = enLamp.ON.ToString();
                    break;
                case 3:
                    if (lampInfo.GreenString == enLamp.ON.ToString()) lampInfo.GreenString = enLamp.TOGGLE.ToString();
                    else if (lampInfo.GreenString == enLamp.TOGGLE.ToString()) lampInfo.GreenString = enLamp.OFF.ToString();
                    else if (lampInfo.GreenString == enLamp.OFF.ToString()) lampInfo.GreenString = enLamp.ON.ToString();
                    break;
                case 4:
                    if (lampInfo.BuzzerString == enLamp.ON.ToString()) lampInfo.BuzzerString = enBuzzer.OFF.ToString();
                    else lampInfo.BuzzerString = enBuzzer.ON.ToString();
                    break;
            }
        }
    }
}
