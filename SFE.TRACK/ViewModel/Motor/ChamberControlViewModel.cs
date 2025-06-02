using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SFE.TRACK.Model;
using MachineDefine;

namespace SFE.TRACK.ViewModel.Motor
{
    public class ChamberControlViewModel : ViewModelBase
    {
        string axisInfo = string.Empty;
        AxisInfoCls axis { get; set; }
        ChamberCls chamber { get; set; }
        HeatTempCls heatTemp { get; set; }
        List<AxisInfoCls> list { get; set; }
        public RelayCommand HomeRelayCommand { get; set; }
        public RelayCommand ServoOnRelayCommand { get; set; }
        public RelayCommand ServoOffRelayCommand { get; set; }
        public RelayCommand StopRelayCommand { get; set; }
        public RelayCommand TempRunRelayCommand { get; set; }
        public RelayCommand TempStopRelayCommand { get; set; }
        public RelayCommand TempSetRelayCommand { get; set; }
        public RelayCommand AutoTuningRelayCommand { get; set; }
        public RelayCommand PinHomeRelayCommand { get; set; }
        public RelayCommand PinReadyRelayCommand { get; set; }
        public RelayCommand PinDownRelayCommand { get; set; }
        public RelayCommand PinUpRelayCommand { get; set; }
        public RelayCommand ShutterOpenRelayCommand { get; set; }
        public RelayCommand ShutterCloseRelayCommand { get; set; }
        public RelayCommand MouseDownPlusRelayCommand { get; set; }
        public RelayCommand MouseUpPlusRelayCommand { get; set; }
        public RelayCommand MouseDownMinusRelayCommand { get; set; }
        public RelayCommand MouseUpMinusRelayCommand { get; set; }
        public RelayCommand PitchClickRelayCommand { get; set; }
        public RelayCommand<object> GridlDoubleClickRelayCommand { get; set; }
        public RelayCommand<object> GridTempDoubleClickRelayCommand { get; set; }
        bool[] isVelocity = new bool[2] { false, true };
        float pitch = 0.1f;
        string command = string.Empty;
        float heatSetTemp = 0;
        public ChamberControlViewModel()
        {
            HomeRelayCommand = new RelayCommand(HomeCommand);
            ServoOnRelayCommand = new RelayCommand(ServoOnCommand);
            ServoOffRelayCommand = new RelayCommand(ServoOffCommand);
            StopRelayCommand = new RelayCommand(StopCommand);
            TempRunRelayCommand = new RelayCommand(TempRunCommand);
            TempStopRelayCommand = new RelayCommand(TempStopCommand);
            TempSetRelayCommand = new RelayCommand(TempSetCommand);
            AutoTuningRelayCommand = new RelayCommand(AutoTuningCommand);
            PinHomeRelayCommand = new RelayCommand(PinHomeCommand);
            PinReadyRelayCommand = new RelayCommand(PinReadyCommand);
            PinDownRelayCommand = new RelayCommand(PinDownCommand);
            PinUpRelayCommand = new RelayCommand(PinUpCommand);
            ShutterOpenRelayCommand = new RelayCommand(ShutterOpenCommand);
            ShutterCloseRelayCommand = new RelayCommand(ShutterCloseCommand);
            MouseDownPlusRelayCommand = new RelayCommand(MouseDownPlusCommand);
            MouseUpPlusRelayCommand = new RelayCommand(MouseUpPlusCommand);
            MouseDownMinusRelayCommand = new RelayCommand(MouseDownMinusCommand);
            MouseUpMinusRelayCommand = new RelayCommand(MouseUpMinusCommand);
            PitchClickRelayCommand = new RelayCommand(PitchClickCommand);
            GridlDoubleClickRelayCommand = new RelayCommand<object>(GridlDoubleClickCommand);
            GridTempDoubleClickRelayCommand = new RelayCommand<object>(GridTempDoubleClickCommand);
        }
        private void MouseDownPlusCommand()
        {
            if (Axis == null) return;

            if(IsVelocity[0]) //Jog
            {
                command = string.Format("Chamber:{0}:{1}:{2}", Chamber.BlockNo, Chamber.ModuleNo, (int)enDirection.CW);
                Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___JogMove, command);
            }
            else if(IsVelocity[1])
            {
                command = string.Format("Chamber:{0}:{1}:{2}", Chamber.BlockNo, Chamber.ModuleNo, Pitch * 1000);
                Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___PitchMove, command);
            }
        }
        private void MouseUpPlusCommand()
        {
            if (Axis == null) return;

            if(IsVelocity[0])
            {
                command = string.Format("Chamber:{0}:{1}", Chamber.BlockNo, Chamber.ModuleNo);
                Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___MoveStop, command);
            }
        }
        private void MouseDownMinusCommand()
        {
            if (Axis == null) return;

            if (IsVelocity[0]) //Jog
            {
                command = string.Format("Chamber:{0}:{1}:{2}", Chamber.BlockNo, Chamber.ModuleNo, (int)enDirection.CCW);
                Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___JogMove, command);
            }
            else if (IsVelocity[1])
            {
                command = string.Format("Chamber:{0}:{1}:{2}", Chamber.BlockNo, Chamber.ModuleNo, (-1000)*Pitch);
                Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___PitchMove, command);
            }
        }
        private void MouseUpMinusCommand()
        {
            MouseUpPlusCommand();
        }

        private void HomeCommand()
        {
            if (Chamber == null) return;
            command = string.Format("Module:{0},{1},{2}", Chamber.MachineName, Chamber.BlockNo, Chamber.ModuleNo);
            Chamber.HomeSituation = enHomeState.HOMMING;
            Chamber.ModuleState = enModuleState.NOTINITIAL;
            Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___ModuleOriginMove, command, true);
        }
        private void ServoOnCommand()
        {
            if (Chamber == null) return;
            command = string.Format("Chamber:{0}:{1}", Chamber.BlockNo, Chamber.ModuleNo);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___ServoOn, command);
            Axis.Servo = true;
        }
        private void ServoOffCommand()
        {
            if (Chamber == null) return;
            command = string.Format("Chamber:{0}:{1}", Chamber.BlockNo, Chamber.ModuleNo);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___ServoOff, command);
            Axis.Servo = false;
        }
        private void StopCommand()
        {
            if (Chamber == null) return;
            command = string.Format("Chamber:{0}:{1}", Chamber.BlockNo, Chamber.ModuleNo);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___MoveStop, command);
        }
        private void TempRunCommand()
        {
            if (Chamber == null) return;
            if (HeatTemp == null) return;
            command = string.Format("Chamber:{0}:{1}:{2}", Chamber.BlockNo, Chamber.ModuleNo, HeatTemp.ZoneIndex);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___Run, command);
        }
        private void TempStopCommand()
        {
            if (Chamber == null) return;
            if (HeatTemp == null) return;
            command = string.Format("Chamber:{0}:{1}:{2}", Chamber.BlockNo, Chamber.ModuleNo, HeatTemp.ZoneIndex);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___Stop, command);
        }
        private void TempSetCommand()
        {
            if (Chamber == null) return;
            if (HeatTemp == null) return;
            command = string.Format("Chamber:{0}:{1}:{2}:{3}", Chamber.BlockNo, Chamber.ModuleNo, HeatTemp.ZoneIndex, HeatSetTemp);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___SetTemperature, command);
        }
        private void AutoTuningCommand()
        {
            if (Chamber == null) return;
            if (HeatTemp == null) return;
            command = string.Format("Chamber:{0}:{1}:{2}", Chamber.BlockNo, Chamber.ModuleNo, HeatTemp.ZoneIndex);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___AutoTuning, command);
        }
        private void PinHomeCommand()
        {
            if (Chamber == null) return;
            command = string.Format("Chamber:{0}:{1}", Chamber.BlockNo, Chamber.ModuleNo);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___PinHome, command);
        }
        private void PinReadyCommand()
        {
            if (Chamber == null) return;
            command = string.Format("Chamber:{0}:{1}", Chamber.BlockNo, Chamber.ModuleNo);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___PinReady, command);
        }
        private void PinDownCommand()
        {
            if (Chamber == null) return;
            command = string.Format("Chamber:{0}:{1}", Chamber.BlockNo, Chamber.ModuleNo);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___PinDown, command);
        }
        private void PinUpCommand()
        {
            if (Chamber == null) return;
            command = string.Format("Chamber:{0}:{1}", Chamber.BlockNo, Chamber.ModuleNo);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___PinUp, command);
        }
        private void ShutterOpenCommand()
        {
            if (Chamber == null) return;
            command = string.Format("Chamber:{0}:{1}", Chamber.BlockNo, Chamber.ModuleNo);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___ShutterOpen, command);
        }
        private void ShutterCloseCommand()
        {
            if (Chamber == null) return;
            command = string.Format("Chamber:{0}:{1}", Chamber.BlockNo, Chamber.ModuleNo);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___ShutterClose, command);
        }
        private void PitchClickCommand()
        {
            Pitch = Global.KeyPad(Pitch, 10, -10);
        }
        private void GridTempDoubleClickCommand(object o)
        {
            System.Windows.Controls.DataGrid grid = o as System.Windows.Controls.DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            if (Chamber == null) return;
            if (HeatTemp == null) return;

            Console.WriteLine(HeatTemp.ZoneIndex);

            switch (index)
            {
                case 2:
                    Chamber.HeatTempList[HeatTemp.ZoneIndex - 1].SetValue = Global.KeyPad(HeatTemp.SetValue);
                    break;
            }

        }
        private void GridlDoubleClickCommand(object o)
        {
            System.Windows.Controls.DataGrid grid = o as System.Windows.Controls.DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            switch(index)
            {
                case 3:
                    float max = 0;
                    float min = 0;

                    if (Chamber.MachineName.ToUpper().IndexOf("ADH") != -1) { max = 180; min = 50; }
                    else if (Chamber.MachineName.ToUpper().IndexOf("CPL") != -1) { max = 28; min = 15; }
                    else if (Chamber.MachineName.ToUpper().IndexOf("LHP") != -1) { max = 180; min = 50; }
                    else if (Chamber.MachineName.ToUpper().IndexOf("HHP") != -1) { max = 300; min = 50; }

                    //Chamber.SetValue = Global.KeyPad(Chamber.SetValue, max, min);
                    //Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___SetTemperature, string.Format("Chamber:{0}:{1}:{2}", Chamber.BlockNo, Chamber.ModuleNo, Chamber.SetValue));
                    break;
            }
        }
        public void SetDisplay()
        {
            AxisInfo = "Axis Status (" + Global.STMotorIODataMessage.Title + ")";
            AxisList = Global.STAxis.FindAll(x => x.AxisID == Global.STMotorIODataMessage.Title.Replace("-", "_")).ToList();
            if(AxisList.Count != 0)
            {
                Axis = AxisList[0];
                Chamber = (ChamberCls)Global.GetModule(Axis.BlockNo, Axis.ModuleNo);
                AxisInfo += " " + Chamber.MachineTitle;
                Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___ChamberState, string.Format("Chamber:{0}:{1}", Chamber.BlockNo, Chamber.ModuleNo));
            }
        }
        public float Pitch
        {
            get { return pitch; }
            set { pitch = value; RaisePropertyChanged("Pitch"); }
        }
        public bool[] IsVelocity
        {
            get { return isVelocity; }
            set { isVelocity = value; RaisePropertyChanged("IsVelocity"); }
        }
        public string AxisInfo
        {
            get { return axisInfo; }
            set { axisInfo = value; RaisePropertyChanged("AxisInfo"); }
        }

        public List<AxisInfoCls> AxisList
        {
            get { return list; }
            set { list = value; RaisePropertyChanged("AxisList"); }
        }

        public AxisInfoCls Axis
        {
            get { return axis; }
            set { axis = value; RaisePropertyChanged("Axis"); }
        }
        public ChamberCls Chamber
        {
            get { return chamber; }
            set { chamber = value; RaisePropertyChanged("Chamber"); }
        }
        public HeatTempCls HeatTemp
        {
            get { return heatTemp; }
            set { heatTemp = value; RaisePropertyChanged("HeatTemp"); }
        }
        public float HeatSetTemp
        {
            get { return heatSetTemp; }
            set { heatSetTemp = value; RaisePropertyChanged("HeatSetTemp"); }
        }
    }
}
