using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using DefaultBase;

namespace SFE.TRACK.Model
{
    public class SpinChamberCls : ModuleBaseCls
    {
        public List<WaferDataArray> _waferWorkData;
        public SpinChamberCls()
        {
            _waferWorkData = new List<WaferDataArray>();
        }
    }
}
