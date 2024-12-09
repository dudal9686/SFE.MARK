using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SFE.TRACK.Model;

namespace SFE.TRACK.ViewModel.Motor
{
    public class MotorControlViewModel : ViewModelBase
    {
        List<AxisInfoCls> list { get; set; }
        string AxisInfo_ = "Axis Status";
        public AxisInfoCls Axis { get; set; }
        bool[] isVelocity = new bool[3] { true, false, false };
        public RelayCommand<object> VelocityGridDoubleClickRelayCommand { get; set; }
        
        public RelayCommand MouseDownPlusRelayCommand { get; set; }
        public RelayCommand MouseUpPlusRelayCommand { get; set; }
        public RelayCommand MouseDownMinusRelayCommand { get; set; }
        public RelayCommand MouseUpMinusRelayCommand { get; set; }

        double firstPosition = 0;
        double secondPosition = 0;
        public RelayCommand FirstMoveRelayCommand { get; set; }
        public RelayCommand SecondMoveRelayCommand { get; set; }
        public RelayCommand FirstTeachingRelayCommand { get; set; }
        public RelayCommand SecondTeachingRelayCommand { get; set; }
        public RelayCommand RepeatRelayCommand { get; set; }

        public MotorControlViewModel()
        {
            VelocityGridDoubleClickRelayCommand = new RelayCommand<object>(VelocityGridDoubleClickCommand);
            
            MouseDownPlusRelayCommand = new RelayCommand(MouseDownPlusCommand);
            MouseUpPlusRelayCommand = new RelayCommand(MouseUpPlusCommand);
            MouseDownMinusRelayCommand = new RelayCommand(MouseDownMinusCommand);
            MouseUpMinusRelayCommand = new RelayCommand(MouseUpMinusCommand);
            FirstMoveRelayCommand = new RelayCommand(FirstMoveCommand);
            SecondMoveRelayCommand = new RelayCommand(SecondMoveCommand);
            FirstTeachingRelayCommand = new RelayCommand(FirstTeachingCommand);
            SecondTeachingRelayCommand = new RelayCommand(SecondTeachingCommand);
        }        

        public bool[] IsVelocity
        {
            get { return isVelocity; }
            set { isVelocity = value; RaisePropertyChanged("IsVelocity"); }
        }

        private void MouseDownPlusCommand()
        {
            Console.WriteLine("AAAAA");
        }

        private void MouseUpPlusCommand()
        {
            Console.WriteLine("BBBBB");
        }

        private void MouseDownMinusCommand()
        {
            Console.WriteLine("CCCCC");
        }

        private void MouseUpMinusCommand()
        {
            Console.WriteLine("DDDDD");
        }

        private void FirstMoveCommand()
        {

        }
        private void SecondMoveCommand()
        {

        }
        private void FirstTeachingCommand()
        {

        }
        private void SecondTeachingCommand()
        {

        }

        public List<AxisInfoCls> AxisList
        {
            get { return list; }
            set { list = value; RaisePropertyChanged("AxisList"); }
        }

        public string AxisInfo
        {
            get { return AxisInfo_; }
            set { AxisInfo_ = value; RaisePropertyChanged("AxisInfo"); }
        }

        public void SetDisplay()
        {
            AxisList = Global.STAxis.FindAll(x => x.AxisNo == (int)Global.STMotorIODataMessage.AxisType).ToList();

            if (AxisList.Count != 0)
            {
                AxisInfo = string.Format("Axis Status ({0})", AxisList[0].AxisID);
                Axis = AxisList[0];
                Console.WriteLine(Axis.ServoState);
            }
            else AxisInfo = "Axis Status";
        }

        public double FirstPosition
        {
            get { return firstPosition; }
            set { firstPosition = value; RaisePropertyChanged("FirstPosition"); }
        }
        public double SecondPosition
        {
            get { return secondPosition; }
            set { secondPosition = value; RaisePropertyChanged("SecondPosition"); }
        }
        private void VelocityGridDoubleClickCommand(object o)
        {
            System.Windows.Controls.DataGrid grid = o as System.Windows.Controls.DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            switch(index)
            {
                case 0:
                    Axis.ACC = Convert.ToInt32(Global.KeyPad(Axis.ACC));
                    break;
                case 1:
                    Axis.DEC = Convert.ToInt32(Global.KeyPad(Axis.DEC));
                    break;
                case 2:
                    Axis.VEL = Convert.ToInt32(Global.KeyPad(Axis.VEL));
                    break;
            }
        }
    }
}
