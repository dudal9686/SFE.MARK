using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.Reflection;
using System.Globalization;

namespace SFE.TRACK.Language
{
    public class Localization
    {
        public static ResourceManager STResouceManager = null;

        public static void Initialize()
        {
            STResouceManager = new ResourceManager("SFE.TRACK.Properties.Resources", Assembly.GetExecutingAssembly());
        }

        public static String ResString(String resName)
        {
            String reResouce = String.Empty;
            try
            {
                reResouce = STResouceManager.GetString(resName, CultureInfo.CurrentCulture);
            }
            catch { }

            return reResouce;
        }
    }
}
