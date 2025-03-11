using carenirvana.bre.codegenerator;
using carenirvana.bre.codegenerator.Impl;
using carenirvana.bre.utility;
using carenirvana.bre.model.Impl;
using static carenirvana.bre.codegenerator.EnumCollection;

namespace carenirvana.bre.codebuilder
{
    public class RuleClassBuilder(RuleSetting ruleSetting, string ruleSetName) : CodeModelBuilder
    {
        private readonly RuleSetting _ruleSetting = ruleSetting ?? throw new ArgumentNullException(nameof(ruleSetting));
        private readonly string _ruleSetName = ruleSetName ?? throw new ArgumentNullException(nameof(ruleSetName));

        public override ICodeModel ToModel()
        {
            BuildRuleClass();
            return CodeModel;
        }

        private void BuildRuleClass()
        {
            AddClassWithName("RuleExecutor", false)
                .AddNamespace("carenirvana.bre.engine.ruleexecutor")
                .AddNamespaceImports(["carenirvana.bre.engine.rulefunction", "System"]);

            AddRuleVariables();
            AddRuleMethods();
        }

        private void AddRuleVariables()
        {
            foreach (var ruleVar in _ruleSetting.RuleVariable)
            {
                var originalType = HelperFunctions.GetOriginalType(ruleVar.VariableValue).ToString();
                var variableValue = GetVariableValue(ruleVar.VariableValue, originalType);

                var codeMember = new CodeMember(
                    ruleVar.VariableName,
                    originalType,
                    [CodeModelAttribute.Private, CodeModelAttribute.Static, CodeModelAttribute.Final],
                    null,
                    variableValue);

                AddField(codeMember);
            }
        }

        private static string GetVariableValue(string variableValue, string originalType)
        {
            if (originalType.Contains("string", StringComparison.CurrentCultureIgnoreCase))
            {
                return $"\"{variableValue}\"";
            }
            if (originalType.Contains("date", StringComparison.CurrentCultureIgnoreCase))
            {
                return $"Convert.ToDateTime(\"{variableValue}\")";
            }
            return variableValue;
        }

        private void AddRuleMethods()
        {
            var ruleSet = _ruleSetting.RuleNode.FirstOrDefault(x => x.RulesetName == _ruleSetName) ?? throw new InvalidOperationException($"Rule set '{_ruleSetName}' not found.");
            foreach (var rule in ruleSet.Rules)
            {
                var methodParams = GetMethodParameters(rule);
                var methodBody = RuleMethodBody.BuildMethodBody(rule);
                var codeMember = new CodeMemberMethod(
                    rule.RuleName,
                    typeof(bool).ToString(),
                    [CodeModelAttribute.Public, CodeModelAttribute.Static],
                    null,
                    ReplaceFunctionNameInMethodBody(methodBody),
                    methodParams);

                AddMethod(codeMember);
            }
        }

        private Dictionary<string, Type> GetMethodParameters(Rule rule)
        {
            var methodParams = new Dictionary<string, Type>();
            foreach (var param in rule.Parameters)
            {
                if (!IsParamAFunction(param.ParameterName))
                {
                    methodParams.Add(param.ParameterName, HelperFunctions.GetTypeFromString(param.Type));
                }
                else
                {
                    var ruleFunction = _ruleSetting.RuleFunction.FirstOrDefault(x => x.Name == param.ParameterName);
                    if (ruleFunction != null)
                    {
                        foreach (var ruleParam in ruleFunction.Parameters)
                        {
                            methodParams.Add(ruleParam.ParameterName, HelperFunctions.GetTypeFromString(ruleParam.Type));
                        }
                    }
                }
            }
            return methodParams;
        }

        private bool IsParamAFunction(string fieldName)
        {
            return _ruleSetting.RuleFunction.Any(x => x.Name == fieldName);
        }

        private string ReplaceFunctionNameInMethodBody(string methodBody)
        {
            foreach (var function in _ruleSetting.RuleFunction)
            {
                if (methodBody.Contains(function.Name))
                {
                    var parameters = string.Join(",", function.Parameters.Select(x => x.ParameterName));
                    methodBody = methodBody.Replace(function.Name, $"RuleFunctionEx.{function.Name}({parameters})");
                }
            }
            return methodBody;
        }
    }
}