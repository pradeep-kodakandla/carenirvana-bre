using carenirvana.bre.utility;
using System;
using System.Configuration;

namespace carenirvana.bre.engine
{
    public static class ConfigReader
    {
        public static string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
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
