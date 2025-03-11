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
using System.Windows.Shapes;
using MachineDefine;

namespace SFE.TRACK.View.Auto
{
    /// <summary>
    /// DataMonitoring.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DataMonitoring : Window
    {
        public DataMonitoring()
        {
            InitializeComponent();
        }

        private void MonitoringDataView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.StatusChange___MonitoringDoRequest, string.Format("MONIT:TRUE"));
            else
                Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.StatusChange___MonitoringDoRequest, string.Format("MONIT:FALSE"));
        }
    }
}
