using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Globalization;
using CoreCSBase;

namespace SFE.TRACK.Log
{
    public class LogCls
    {
        private object LockObject = new object();
        private object SocketLockObject = new object();
        private object AlarmLockObject = new object();
        Logger socketLogger;
        Logger alarmLogger;
        public LogCls()
        {
            socketLogger = new Logger();
            DirectoryInfo directoryInfo = new DirectoryInfo(@"D:\MARK_LOG\UI\SOCKET_LOG\");
            if (!directoryInfo.Exists) directoryInfo.Create();
            socketLogger.Initialize(@"D:\MARK_LOG\UI\SOCKET_LOG\", "log", 1, "UISocket", "");
            socketLogger.SetAutoDelete(30);

            alarmLogger = new Logger();
            directoryInfo = new DirectoryInfo(@"D:\MARK_LOG\UI\ALARM_LOG\");
            if (!directoryInfo.Exists) directoryInfo.Create();
            alarmLogger.Initialize(@"D:\MARK_LOG\UI\ALARM_LOG\", "log", 1, "Alarm", "");
            alarmLogger.SetAutoDelete(30);
        }
        private void SetDirectory(ref string dir)
        {
            DateTime dt = DateTime.Now;
            dir = dir + dt.ToString("yyyy") + @"\" + dt.ToString("MM") + @"\" + dt.ToString("dd") + @"\";
            DirectoryInfo logDir = new DirectoryInfo(dir);
            if (!logDir.Exists) logDir.Create();
        }

        public void AddLog(string msg)
        {
            string directoryPath = @"D:\MARK_LOG\UI\LOG\";
            string filePath = string.Empty;
            StreamWriter sw = null;

            try
            {
                Monitor.Enter(LockObject);
                SetDirectory(ref directoryPath);
                filePath = directoryPath + DateTime.Now.ToString("HH") + ".csv";

                FileInfo fileInfo = new FileInfo(filePath);

                if(!fileInfo.Exists) sw = new StreamWriter(fileInfo.Create(), Encoding.Default);
                else sw = new StreamWriter(filePath, true, Encoding.Default);

                sw.Write("{0},", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff", CultureInfo.InvariantCulture));
                sw.WriteLine(msg);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                sw.Flush();
                sw.Close();
                sw.Dispose();
                Monitor.Exit(LockObject);
            }
            Thread.Sleep(1);
        }

        public void AddSocketLog<TEnumGroup>(int id, string type, string msgType, TEnumGroup cmdGroup, string cmdItem, string msg) where TEnumGroup : struct, Enum
        {
            Monitor.Enter(SocketLockObject);
            string strPacket = string.Format("{0}({1}),{2},{3},{4},{5}", type, id, msgType, cmdGroup, cmdItem, msg);
            socketLogger.WriteLine(strPacket);
            Monitor.Exit(SocketLockObject);
            Thread.Sleep(1);
        }

        public void AddAlarmLog(string packet)
        {
            Monitor.Enter(AlarmLockObject);
            alarmLogger.WriteLine(packet);
            Monitor.Exit(AlarmLockObject);
        }
    }
}
