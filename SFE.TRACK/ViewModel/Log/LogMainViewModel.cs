using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace SFE.TRACK.ViewModel.Log
{
    public class LogMainViewModel : ViewModelBase 
    {
        bool isChamberSocket = true;
        bool isChamberProcess = false;

        bool isRobotSocket = false;
        bool isRobotProcess = false;

        bool isUISocket = false;
        bool isUIAlarm = false;
        ObservableCollection<LogDataCls> logList { get; set; } = new ObservableCollection<LogDataCls>();
        public RelayCommand DateRelayCommand { get; set; }
        string dateDisplay = string.Empty;
        int selectedIndex = 0;
        string directoryInfo = string.Empty;
        List<string> fileList = new List<string>();
        public LogMainViewModel()
        {
            dateDisplay = DateTime.Now.ToString("yyyy-MM-dd");
            DateRelayCommand = new RelayCommand(DateCommand);
            directoryInfo = @"D:\MARK_LOG\CHAMBER\SOCKET_LOG\";
        }

        public void SetDisplay()
        {
            LogList.Clear();
            string date = DateDisplay.Replace("-","");
            //directoryInfo = @"D:\MARK_LOG\CHAMBER\SOCKET_LOG\";

            DirectoryInfo di = new DirectoryInfo(directoryInfo);
            foreach (FileInfo file in di.GetFiles())
            {
                if(file.Name.IndexOf(date) != -1)
                {
                    fileList.Add(file.FullName);
                }
            }
            //Console.WriteLine(string.Format("fileList Count : {0}", fileList.Count));
            string line = string.Empty;
            foreach(string fileName in fileList)
            {
                StreamReader sr = new StreamReader(fileName);
                while(!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (line == string.Empty) continue;

                    string[] arr = line.Split('>');
                    LogDataCls logData = new LogDataCls();
                    logData.Time = arr[0].Replace("<", "").Trim();
                    logData.Message = arr[1].Trim();
                    LogList.Add(logData);
                }

                sr.Close();
                sr.Dispose();
            }
            //Console.WriteLine(string.Format("LogList Count {0}: ", LogList.Count));
            fileList.Clear();
        }
        public ObservableCollection<LogDataCls> LogList
        {
            get { return logList; }
            set { logList = value; RaisePropertyChanged("LogList"); }
        }

        public bool IsChamberSocket
        {
            get { return isChamberSocket; }
            set
            {
                isChamberSocket = value;
                if (isChamberSocket)
                {
                    directoryInfo = @"D:\MARK_LOG\CHAMBER\SOCKET_LOG\";
                    SetDisplay();
                }
                RaisePropertyChanged("IsChamberSocket");
            }
        }

        public bool IsChamberProcess
        {
            get { return isChamberProcess; }
            set
            {
                isChamberProcess = value;
                if (isChamberProcess)
                {
                    directoryInfo = @"D:\MARK_LOG\CHAMBER\PROCESS_LOG\";
                    SetDisplay();
                }
                RaisePropertyChanged("IsChamberProcess");
            }
        }

        public bool IsRobotSocket
        {
            get { return isRobotSocket; }
            set
            {
                isRobotSocket = value;
                if (isRobotSocket)
                {
                    directoryInfo = @"D:\MARK_LOG\ROBOT\SOCKET_LOG\";
                    SetDisplay();
                }
                RaisePropertyChanged("IsRobotSocket");
            }
        }

        public bool IsRobotProcess
        {
            get { return isRobotProcess; }
            set
            {
                isRobotProcess = value;
                if (isRobotProcess)
                {
                    directoryInfo = @"D:\MARK_LOG\ROBOT\PROCESS_LOG\";
                    SetDisplay();
                }
                RaisePropertyChanged("IsRobotProcess");
            }
        }

        public bool IsUISocket
        {
            get { return isUISocket; }
            set
            {
                isUISocket = value;
                if (isUISocket)
                {
                    directoryInfo = @"D:\MARK_LOG\UI\SOCKET_LOG\";
                    SetDisplay();
                }
                RaisePropertyChanged("IsUISocket");
            }
        }

        public bool IsUIAlarm
        {
            get { return isUIAlarm; }
            set
            {
                isUIAlarm = value;
                if (isUIAlarm)
                {
                    directoryInfo = @"D:\MARK_LOG\UI\ALARM_LOG\";
                    SetDisplay();
                }
                RaisePropertyChanged("IsUIAlarm");
            }
        }
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged("SelectedIndex"); }
        }
        public string DateDisplay
        {
            get { return dateDisplay; }
            set { dateDisplay = value; RaisePropertyChanged("DateDisplay"); }
        }

        private void DateCommand()
        {
            if(Global.GetDateOpen(DateDisplay))
            {
                DateDisplay = Global.STDateMessage.Date.ToString("yyyy-MM-dd");
                SetDisplay();
            }
        }
    }

    public class LogDataCls : ViewModelBase
    {
        string time = string.Empty;
        string message = string.Empty;
        public LogDataCls() { }

        public string Time
        {
            get { return time; }
            set { time = value; RaisePropertyChanged("Time"); }
        }

        public string Message
        {
            get { return message; }
            set { message = value; RaisePropertyChanged("Message"); }
        }
    }
}
