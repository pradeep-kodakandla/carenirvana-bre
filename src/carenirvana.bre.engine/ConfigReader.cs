using System.Configuration;
using carenirvana.bre.utility;
using Microsoft.Extensions.Configuration;

namespace carenirvana.bre.engine
{
    public static class ConfigReader
    {
        public static IConfigurationRoot Configuration { get; set; }

        internal static string GetAppSetting(string key)
        {
            // First, try to get the value from appsettings.json
            string value = Configuration.GetSection("AppSettings")[key];
            if (string.IsNullOrEmpty(value))
            {
                // If not found, try to get the value from App.config
                value = ConfigurationManager.AppSettings[key];
            }
            return value;
        }

        public static string InputServer => GetAppSetting(ConstantsUtility.InputServer);
        public static string InputServerDatabase => GetAppSetting(ConstantsUtility.InputServerDatabase);
        public static string InputServerUserName => GetAppSetting(ConstantsUtility.InputServerUserName);
        public static string InputServerPassword => GetAppSetting(ConstantsUtility.InputServerPassword);
        public static string InputServerPortNum => GetAppSetting(ConstantsUtility.InputServerPortNum);
        public static string OutputServer => GetAppSetting(ConstantsUtility.OutputServer);
        public static string OutputServerDatabase => GetAppSetting(ConstantsUtility.OutputServerDatabase);
        public static string OutputServerUserName => GetAppSetting(ConstantsUtility.OutputServerUserName);
        public static string OutputServerPassword => GetAppSetting(ConstantsUtility.OutputServerPassword);
        public static string OutputServerPortNum => GetAppSetting(ConstantsUtility.OutputServerPortNum);
        public static string OutputBatchSize => GetAppSetting(ConstantsUtility.OutputBatchSize);
    }
}
