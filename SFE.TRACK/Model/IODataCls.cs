using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MachineCSBaseSim;

namespace SFE.TRACK.Model
{
    public class IODataCls : ViewModelBase
    {
        UnitIO io { get; set; }
        enIOType ioType = enIOType.DO;
        int blockNo = 0;
        int moduleNo = 0;
        int ioNum = 0;
        bool state = false;
        bool enable = false;
        string name = string.Empty;
        string alias = string.Empty;
        SolidColorBrush stateColor = new SolidColorBrush();
        public RelayCommand IORelayCommand { get; set; }

        public IODataCls()
        {
            IORelayCommand = new RelayCommand(IOCommand);
            stateColor = Brushes.Red;
        }

        public UnitIO IO
        {
            get { return io; }
            set { io = value; RaisePropertyChanged("IO"); }
        }

        public enIOType IOType
        {
            get { return ioType; }
            set { ioType = value; RaisePropertyChanged("IOType"); }
        }

        public int IONum
        {
            get { return ioNum; }
            set { ioNum = value; RaisePropertyChanged("IONum"); }
        }

        public int BlockNo
        {
            get { return blockNo; }
            set { blockNo = value; }
        }

        public int ModuleNo
        {
            get { return moduleNo; }
            set { moduleNo = value; }
        }

        public SolidColorBrush StateColor
        {
            get { return stateColor; }
            set { stateColor = value; RaisePropertyChanged("StateColor"); }
        }

        public string Name
        {
            get { return IO.MyNameInfo.Name; }
        }

        public string Alias
        {
            get { return alias; }
            set { alias = value; RaisePropertyChanged("Alias"); }
        }

        public bool State
        {
            get { return state; }
            set { state = value;
                if (state) StateColor = Brushes.YellowGreen;
                else StateColor = Brushes.Red;
                RaisePropertyChanged("State"); }
        }

        public bool Enable
        {
            get { return enable; }
            set { enable = value; RaisePropertyChanged("Enable"); }
        }

        private void IOCommand()
        {
            Console.WriteLine("IO{0} = {1} ({2})", IOType.ToString(), Name, IONum);
            IO.SetValue(!IO.ReadIO());
        }
    }

    public class AIODataCls : ViewModelBase
    {
        UnitAIO io { get; set; }
        enIOType ioType = enIOType.DO;
        int blockNo = 0;
        int moduleNo = 0;
        int ioNum = 0;
        bool enable = false;
        string name = string.Empty;
        string alias = string.Empty;
        double values = 0;

        public UnitAIO IO
        {
            get { return io; }
            set { io = value; RaisePropertyChanged("IO"); }
        }

        public enIOType IOType
        {
            get { return ioType; }
            set { ioType = value; RaisePropertyChanged("IOType"); }
        }

        public int IONum
        {
            get { return ioNum; }
            set { ioNum = value; RaisePropertyChanged("IONum"); }
        }

        public int BlockNo
        {
            get { return blockNo; }
            set { blockNo = value; }
        }

        public int ModuleNo
        {
            get { return moduleNo; }
            set { moduleNo = value; }
        }

        public string Name
        {
            get { return IO.MyNameInfo.Name; }
        }

        public string Alias
        {
            get { return alias; }
            set { alias = value; RaisePropertyChanged("Alias"); }
        }
        public bool Enable
        {
            get { return enable; }
            set { enable = value; RaisePropertyChanged("Enable"); }
        }

        public double Value
        {
            get { return values; }
            set { values = value; RaisePropertyChanged("Value"); }
        }
    }
}
