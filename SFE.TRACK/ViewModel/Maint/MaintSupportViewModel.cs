using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace SFE.TRACK.ViewModel.Maint
{
    public class MaintSupportViewModel : ViewModelBase
    {
        public RelayCommand LoadRelayCommand { get; set; }
        public RelayCommand SaveSupportRelayCommand { get; set; }
        public RelayCommand<object> SupportGridDoubleClickRelayCommand { get; set; }
        public MaintSupportCls MaintInfo { get; set; }
        int SelectedIndex_ = 0;
        public MaintSupportViewModel()
        {
            LoadRelayCommand = new RelayCommand(LoadCommand);
            SaveSupportRelayCommand = new RelayCommand(SaveSupportCommand);
            SupportGridDoubleClickRelayCommand = new RelayCommand<object>(SupportGridDoubleClickCommand);
        }

        private void LoadCommand()
        {
            GetCalulatorData();
        }

        public int SelectedIndex
        {
            get { return SelectedIndex_; }
            set { SelectedIndex_ = value; RaisePropertyChanged("SelectedIndex"); }
        }

        private void GetCalulatorData()
        {
            DateTime curTime = DateTime.Now;
            
            foreach(MaintSupportCls maint in Global.STMaintSupportList)
            {
                if (maint.Type == 0)
                {
                    DateTime time = Convert.ToDateTime(maint.Time);
                    TimeSpan ts = time - curTime;

                    if (maint.Unit == 0) maint.Amount = ts.Days;
                    else if (maint.Unit == 1) maint.Amount = ts.Days / 7;
                    else if (maint.Unit == 2) maint.Amount = ts.Days / 31;
                    else if (maint.Unit == 3) maint.Amount = ts.Days / 365;

                    maint.AmountDisplay = string.Format("{0} {1}", maint.Amount, (enMaintDate)maint.Unit);
                }
                else
                {
                    maint.AmountDisplay = string.Format("{0} {1}", maint.Amount, (enMaintUnit)maint.Unit);
                }
            }
        }

        private void SaveSupportCommand()
        {
            if (Global.STDataAccess.SaveMaintSupportData()) Global.MessageOpen(enMessageType.OK, "Saved.");
            else Global.MessageOpen(enMessageType.OK, "Not Saved.");
        }

        private void SupportGridDoubleClickCommand(object o)
        {
            System.Windows.Controls.DataGrid grid = o as System.Windows.Controls.DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            switch(index)
            {
                case 3: 
                    if(Global.MessageOpen(enMessageType.OKCANCEL, "Change?")) MaintInfo.IsWatch = !MaintInfo.IsWatch;
                    break;
                case 4:
                    MaintInfo.WarnLevel = Convert.ToInt32(Global.KeyPad(MaintInfo.WarnLevel));
                    break;
                case 5:
                    MaintInfo.LimitValue = Convert.ToInt32(Global.KeyPad(MaintInfo.LimitValue));
                    break;
                case 6:
                    if (Global.MessageOpen(enMessageType.OKCANCEL, "Change?")) MaintInfo.IsAlarm = !MaintInfo.IsAlarm;
                    break;
                case 7:
                    if(MaintInfo.Type == 0)
                    {
                        if (MaintInfo.Unit == (int)enMaintDate.YEARS) MaintInfo.Unit = 0;
                        else MaintInfo.Unit++;

                        GetCalulatorData();
                    }
                    else
                    {
                        if (MaintInfo.Unit == (int)enMaintUnit.VOLUME) MaintInfo.Unit = 0;
                        else MaintInfo.Unit++;

                        GetCalulatorData();
                    }
                    break;
                case 8:
                    
                    if(Global.GetDateOpen(MaintInfo.Time))
                    {
                        MaintInfo.Time = Global.STDateMessage.Date.ToString("yyyy-MM-dd");
                        GetCalulatorData();
                    }
                    break;
            }
        }
    }
}
