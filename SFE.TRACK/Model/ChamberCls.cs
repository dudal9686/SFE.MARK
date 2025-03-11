using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DefaultBase;

namespace SFE.TRACK.Model
{
    public class ChamberCls : ModuleBaseCls        
    {
        float setValue = 0;
        float processValue = 0;
        public ChamberCls()
        {
            
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
    }
}
