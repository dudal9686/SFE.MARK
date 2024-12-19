using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.Reflection;

namespace SFE.TRACK.Language
{
    public class Localization
    {
        public static ResourceManager g_ResouceManager = null;

        public static void Initialize()
        {
            g_ResouceManager = new ResourceManager("SFE.TRACK.Properties.Resources", Assembly.GetExecutingAssembly());
        }

        public static String GetString(String resName)
        {
            String reResouce = String.Empty;
            try
            {
                reResouce = g_ResouceManager.GetString(resName, System.Globalization.CultureInfo.CurrentCulture);
            }
            catch { }

            return reResouce;
        }
    }
}
