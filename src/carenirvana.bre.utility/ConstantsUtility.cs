namespace carenirvana.bre.utility
{
    public class ConstantsUtility
    {
        public const string RuleExecutorTypeName = "RuleExecutor";
        public const string RunTimeNameSpace = "carenirvana.bre.engine.runtime";
        public const string RunTimeInputDataTypeName = "carenirvana.bre.engine.inputdata";
        public const string RunTimeRuleExecutorTypeName = "carenirvana.bre.engine.ruleexecutor";
        public const string RunTimeRuleFunctionTypeName = "carenirvana.bre.engine.rulefunction";
        public const string InputServer = "InputServer";
        public const string InputServerDatabase = "InputServerDatabase";
        public const string InputServerUserName = "InputServerUserName";
        public const string InputServerPassword = "InputServerPassword";
        public const string InputServerPortNum = "InputServerPortNum";
        public const string OutputServer = "OutputServer";
        public const string OutputServerDatabase = "OutputServerDatabase";
        public const string OutputServerUserName = "OutputServerUserName";
        public const string OutputServerPassword = "OutputServerPassword";
        public const string OutputServerPortNum = "OutputServerPortNum";
        public const string OutputBatchSize = "OutputBatchSize";
        public static string[] ExcludeFromMethodList = ["ToString", "Equals", "GetHashCode", "GetType"];
    }
}
