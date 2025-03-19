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
    /// IOControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class IOControl : UserControl
    {
        public IOControl()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                Global.SendCommand(Global.MCS_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Status, EnumCommand_Status.UnitStatus___SendStart, "IO_IN");
                Global.SendCommand(Global.MCS_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Status, EnumCommand_Status.UnitStatus___SendStart, "IO_OUT");
                Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.StatusChange___IODoRequest, string.Format("IO:TRUE"));
            }
            else
            {
                Global.SendCommand(Global.MCS_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Status, EnumCommand_Status.UnitStatus___SendStop, "IO_IN");
                Global.SendCommand(Global.MCS_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Status, EnumCommand_Status.UnitStatus___SendStop, "IO_OUT");
                Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.StatusChange___IODoRequest, string.Format("IO:FALSE"));
            }
        }
    }
}
