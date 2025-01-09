using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using CoreCSRunSim;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SFE.TRACK.Model
{
    public class TeachingDataCls : ViewModelBase
    {
        UnitMotor motor { get; set; }
        string mainTitle = string.Empty;
        int moduleNo = -1;
        int blockNo = 2;
        double acc = 0;
        double dec = 0;
        double vel = 0;
        double pos = 0;
        string teachingName = string.Empty;
        bool isArray = false;
        bool isOwn = false;
        int timeOut = 10000;
        public string MainTitle
        {
            get { return mainTitle; }
            set { mainTitle = value; RaisePropertyChanged("MainTitle"); }
        }
        public bool IsOwn
        {
            get { return isOwn; }
            set { isOwn = value; RaisePropertyChanged("IsOwn"); }
        }
        public string TeachingName
        {
            get
            {
                if (IsArray) return string.Format("{0}_{1}", teachingName, moduleNo);
                return teachingName;
            }
            set { teachingName = value; RaisePropertyChanged("TeachingName"); }
        }
        public int BlockNo
        {
            get { return blockNo; }
            set { blockNo = value; RaisePropertyChanged("BlockNo"); }
        }
        public int ModuleNo
        {
            get { return moduleNo; }
            set { moduleNo = value; RaisePropertyChanged("ModuleNo"); }
        }
        public UnitMotor Motor
        {
            get { return motor; }
            set
            {
                motor = value;
                Acc = GetData().speedPack.acc;
                Dec = GetData().speedPack.dec;
                Vel = GetData().speedPack.speed;
                Pos = GetData().position;
                TimeOut = GetData().speedPack.timeout;
                RaisePropertyChanged("Motor");
            }
        }
        public bool IsArray
        {
            get { return isArray; }
            set { isArray = value; RaisePropertyChanged("IsArray"); }
        }

        public double Acc
        {
            get { return acc; }
            set { acc = value; RaisePropertyChanged("Acc"); }
        }
        public double Dec
        {
            get { return dec; }
            set { dec = value; RaisePropertyChanged("Dec"); }
        }
        public double Vel
        {
            get { return vel; }
            set { vel = value; RaisePropertyChanged("Vel"); }
        }
        public double Pos
        {
            get { return pos; }
            set { pos = value; RaisePropertyChanged("Pos"); }
        }
        public int TimeOut
        {
            get { return timeOut; }
            set { timeOut = value; RaisePropertyChanged("TimeOut"); }
        }
        public string ModuleName
        {
            get
            {
                ModuleBaseCls module = Global.GetModule(BlockNo, ModuleNo);
                return string.Format("{0}[{1}-{2}]", module.MachineName, BlockNo, ModuleNo);
            }
        }
        public Teaching GetData()
        {
            return Motor.GetTeachingPosition(TeachingName);
        }
        public void SetData()
        {
            Motor.SetTeachingPosition(TeachingName, Pos, Vel, Acc, Dec, TimeOut);
        }
    }
}
