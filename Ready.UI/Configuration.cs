using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Serilog;

namespace Ready.UI
{
    public class Configuration
    {
        private static ILogger log = Log.ForContext(typeof(Configuration));
        private Dictionary<string, string> dict;

        private Configuration()
        {
            dict = new Dictionary<string, string>();
        }

        private static Configuration instance;

        public static void FromAppSettings()
        {
            log.Debug("Configuring from AppSettings");
            Configuration config = new Configuration();
            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
                config.dict[key] = ConfigurationManager.AppSettings.Get(key);

            instance = config;
            log.Information("Configuration complete");
        }

        public static void FromCommandLine(string[] args)
        {
            log.Debug("Configuring from CommandLine");
            Configuration config = new Configuration();
            
            instance = config;
            log.Information("Configuration complete");
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
