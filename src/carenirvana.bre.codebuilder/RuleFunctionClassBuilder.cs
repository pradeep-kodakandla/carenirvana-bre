using carenirvana.bre.codegenerator.Impl;
using carenirvana.bre.codegenerator;
using carenirvana.bre.utility;
using carenirvana.bre.model.Impl;
using static carenirvana.bre.codegenerator.EnumCollection;

namespace carenirvana.bre.codebuilder
{
    public class RuleFunctionClassBuilder(List<RuleFunction> ruleFunctions) : CodeModelBuilder
    {
        private readonly List<RuleFunction> _ruleFunctions = ruleFunctions ?? throw new ArgumentNullException(nameof(ruleFunctions));

        public override ICodeModel ToModel()
        {
            BuildRuleFunctionClass();
            return CodeModel;
        }

        private void BuildRuleFunctionClass()
        {
            AddClassWithName("RuleFunctionEx", false)
                .AddNamespace("carenirvana.bre.engine.rulefunction");

            foreach (var ruleFunction in _ruleFunctions)
            {
                var methodParams = ruleFunction.Parameters.ToDictionary(
                    param => param.ParameterName,
                    param => HelperFunctions.GetTypeFromString(param.Type)
                );

                var codeMember = new CodeMemberMethod(
                    ruleFunction.Name,
                    HelperFunctions.GetTypeFromString(ruleFunction.ReturnType).ToString(),
                    [CodeModelAttribute.Public, CodeModelAttribute.Static],
                    null,
                    $"return {HelperFunctions.AppendRuleFuncToMethodBodyIfExists(ruleFunction.MethodBody)};",
                    methodParams
                );

                AddMethod(codeMember);
            }
        }
    }
}