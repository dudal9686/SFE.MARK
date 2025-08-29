using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MachineDefine;

namespace SFE.TRACK.View.Motor
{
    /// <summary>
    /// MotorControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MotorControl : UserControl
    {
        public MotorControl()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                Global.SendCommand(Global.MCS_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Status, EnumCommand_Status.UnitStatus__SendStart, "SFETrack:MOTOR");
                Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.StatusChange__MotorDoRequest, string.Format("Motor:TRUE"));
            }                
            else
            {
                Global.SendCommand(Global.MCS_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Status, EnumCommand_Status.UnitStatus__SendStop, "SFETrack:MOTOR");
                Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.StatusChange__MotorDoRequest, string.Format("Motor:FALSE"));
            }   
        }
    }
}
