﻿using System;
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

namespace SFE.TRACK.View.Param
{
    /// <summary>
    /// UCParameterMain.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UCParameterMain : UserControl
    {
        public UCParameterMain()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                Global.SendCommand(Global.MCS_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Status, EnumCommand_Status.UnitStatus__SendStart, "MOTOR");
                Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.StatusChange__MotorDoRequest, string.Format("Chamber:TRUE"));
                Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.StatusChange__ChamberDoRequest, string.Format("Chamber:TRUE"));
            }
            else
            {
                Global.SendCommand(Global.MCS_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Status, EnumCommand_Status.UnitStatus__SendStop, "MOTOR");
                Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.StatusChange__MotorDoRequest, string.Format("Chamber:FALSE"));
                Global.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.StatusChange__ChamberDoRequest, string.Format("Chamber:FALSE"));
            }
        }
    }
}
