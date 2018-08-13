using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Ready.UI
{
    public class Configuration
    {
        private Dictionary<string, string> dict;

        private Configuration()
        {
            dict = new Dictionary<string, string>();
        }

        private static Configuration instance;

        public static void FromAppSettings()
        {
            Configuration config = new Configuration();
            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
                config.dict[key] = ConfigurationManager.AppSettings.Get(key);

            instance = config;
        }

        public static void FromCommandLine(string[] args)
        {
            Configuration config = new Configuration();
            
            instance = config;
        }

        public static string GetValue(string key)
        {
            return GetValue<string>(key);
        }
        public static T GetValue<T>(string key)
        {
            T value;
            if (instance.dict.ContainsKey(key) && CanChangeType(instance.dict[key], typeof(T)))
                value = (T)Convert.ChangeType(instance.dict[key], typeof(T));
            else
                value = default(T);

            return value;
        }

        private static bool CanChangeType(object value, Type conversionType)
        {
            if (conversionType == null)
            {
                return false;
            }

            if (value == null)
            {
                return false;
            }

            IConvertible convertible = value as IConvertible;

            if (convertible == null)
            {
                return false;
            }

            return true;
        }
    }
}
