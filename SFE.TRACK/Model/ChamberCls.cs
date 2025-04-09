using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DefaultBase;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SFE.TRACK.Model
{
    public class ChamberCls : ModuleBaseCls        
    {
        List<HeatTempCls> _heatTempList = new List<HeatTempCls>();
        public ChamberCls()
        {
            
            for (int i = 0; i < 4; i++)
            {
                HeatTempCls temp = new HeatTempCls();
                temp.ZoneIndex = i + 1;
                _heatTempList.Add(temp);
            }
        }

        public List<HeatTempCls> HeatTempList
        {
            get { return _heatTempList; }
            set { _heatTempList = value; RaisePropertyChanged("HeatTempList"); }
        }
    }

    public class HeatTempCls : ViewModelBase
    {
        int zoneIndex = 0;
        float processValue = 0;
        float setValue = 0;
        string controllerStatus = "STOP";

        public int ZoneIndex
        {
            get { return zoneIndex; }
            set { zoneIndex = value; RaisePropertyChanged("ZoneIndex"); }
        }
        public float SetValue
        {
            get { return setValue; }
            set { setValue = value; RaisePropertyChanged("SetValue"); }
        }

        public float ProcessValue
        {
            get { return processValue; }
            set { processValue = value; RaisePropertyChanged("ProcessValue"); }
        }

        public string ControllerStatus
        {
            get { return controllerStatus; }
            set { controllerStatus = value; RaisePropertyChanged("ControllerStatus"); }
        }
    }
}
