using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Globalization;

namespace SFE.TRACK.Log
{
    public class LogCls
    {
        private object LockObject = new object();
        private void SetDirectory(ref string dir)
        {
            DateTime dt = DateTime.Now;
            dir = @"D:\TRACK_LOG\" + dt.ToString("yyyy") + @"\" + dt.ToString("MM") + @"\" + dt.ToString("dd") + @"\";
            DirectoryInfo logDir = new DirectoryInfo(dir);
            if (!logDir.Exists) logDir.Create();
        }

        public void AddLog(string msg)
        {
            string directoryPath = string.Empty;
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
                System.Windows.Forms.MessageBox.Show(ex.StackTrace);
            }
            finally
            {
                sw.Flush();
                sw.Close();
                sw.Dispose();
                Monitor.Exit(LockObject);
            }
        }
    }
}
