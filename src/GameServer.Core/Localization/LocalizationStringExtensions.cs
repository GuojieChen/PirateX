using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Core.Localization
{
    public static class LocalizationStringExtensions
    {
        private static ResourceManager rm = null;

        public static string CultureCode { get; set; }
        

        static LocalizationStringExtensions()
        {
            
        }

        public static string GetString(this string name)
        {
            if(!Equals(Thread.CurrentThread.CurrentCulture.Name,CultureCode))
                Thread.CurrentThread.CurrentCulture = new CultureInfo(CultureCode);

            return rm.GetString(name,Thread.CurrentThread.CurrentCulture); 
        }
    }
}
