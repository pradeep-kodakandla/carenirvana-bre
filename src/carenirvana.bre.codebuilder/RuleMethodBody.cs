using carenirvana.bre.model.Impl;
using carenirvana.bre.utility;
using System.Text;

namespace carenirvana.bre.codebuilder
{
    public class RuleMethodBody
    {
        public static string BuildMethodBody(Rule rule)
        {
            if (rule.Expressions == null || rule.Expressions.Count == 0)
            {
                throw new ArgumentException("Rule must contain at least one expression.");
            }

            var methodBody = new StringBuilder();

            if (rule.Expressions.Count == 1)
            {
                var ruleExpression = rule.Expressions[0];
                AppendExpression(methodBody, ruleExpression.Expression, ruleExpression.LogicCondition);
            }
            else
            {
                for (var i = 0; i < rule.Expressions.Count; i++)
                {
                    var ruleExpression = rule.Expressions[i];
                    if (i == 0)
                    {
                        methodBody.Append($"if(({ruleExpression.Expression}) {HelperFunctions.ParseLoginExpression(ruleExpression.LogicCondition)} ");
                    }
                    else
                    {
                        methodBody.Append($"({ruleExpression.Expression}) {HelperFunctions.ParseLoginExpression(ruleExpression.LogicCondition)} ");
                    }
                }
            }

            return $"{HelperFunctions.AppendRuleFuncToMethodBodyIfExists(methodBody.ToString().Trim())}) return true; else return false;";
        }

        private static void AppendExpression(StringBuilder methodBody, string expression, string logicCondition)
        {
            if (string.IsNullOrEmpty(logicCondition))
            {
                methodBody.Append($"if({expression}");
            }
            else
            {
                methodBody.Append($"if(({expression}) {HelperFunctions.ParseLoginExpression(logicCondition)} ");
            }
        }
    }
}