using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SFE.TRACK.Model
{
    public class PRARobotCls : ModuleBaseCls
    {
        List<WaferCls> armWaferList = new List<WaferCls>();
        
        public PRARobotCls()
        {
            for(int i = 0; i < 3; i++) armWaferList.Add(new WaferCls());
        }

        public List<WaferCls> ArmWaferList
        {
            get { return armWaferList; }
            set { armWaferList = value; RaisePropertyChanged("ArmWaferList"); }
        }
    }
}
