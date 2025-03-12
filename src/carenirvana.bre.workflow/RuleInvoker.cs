using carenirvana.bre.model;
using carenirvana.bre.model.Impl;
using carenirvana.bre.utility;
using System.Reflection;

namespace carenirvana.bre.workflow
{
    public class RuleInvoker(WorkflowAssemblyCacher workflowAssemblyCacher, RuleSetting ruleSetting, int runId)
    {
        private readonly WorkflowAssemblyCacher _workflowAssemblyCacher = workflowAssemblyCacher ?? throw new ArgumentNullException(nameof(workflowAssemblyCacher));
        private readonly RuleSetting _ruleSetting  = ruleSetting;
        private readonly int runId = runId;

        public void InvokeRuleMethods(IWorkflowItem workItem)
        {
            foreach (var method in _workflowAssemblyCacher.Methods)
            {
                try
                {
                    InvokeRuleMethod(method, workItem);
                }
                catch (Exception ex)
                {
                    HandleMethodInvocationError(workItem, method, ex);
                }
            }
        }

        public void InvokeRuleMethod(string methodName, IWorkflowItem workItem)
        {
            try
            {
                // build parameters for the rule from the work item...

                var value = _workflowAssemblyCacher.InvokeMethod(methodName, BuildParametersForRule(workItem, methodName));
                BuildOutput(value, workItem, methodName);
            }
            catch (Exception ex)
            {
                HandleMethodInvocationError(workItem, methodName, ex);
            }
        }

        private void BuildOutput(object value, IWorkflowItem workItem, string methodName)
        {
            var ruleOutput = new RuleOutput
            {
                RunId = runId,
                UniqueId = workItem.Id,
                Result = bool.Parse(value.ToString()),
                RunDtTm = DateTime.Now.Date,
                RuleId = int.Parse(GetRuleId(methodName)), // need to find the rule id...
                RuleName = methodName,
                OutputMessage = ""
            };
            workItem.AddOutput(ruleOutput);

            Console.WriteLine($"RuleOutput: UniqueId={ruleOutput.UniqueId}, Result={ruleOutput.Result}, RuleId={ruleOutput.RuleId}, RuleName={ruleOutput.RuleName}, OutputMessage={ruleOutput.OutputMessage}");

        }

        private void HandleMethodInvocationError(IWorkflowItem workItem, string methodName, Exception ex)
        {
            // Log or handle the error as needed
            Console.WriteLine($"Error invoking method {methodName} for work item {workItem.Id}: {ex.Message}");
        }

        private object[] BuildParametersForRule(IWorkflowItem workItem, string methodName)
        {
            var rule = _ruleSetting.RuleNode
                .SelectMany(node => node.Rules)
                .FirstOrDefault(r => r.RuleName == methodName);

            if (rule == null)
            {
                throw new InvalidOperationException($"Rule '{methodName}' not found in RuleSetting.");
            }

            var parameters = new List<object>();
            foreach (var param in rule.Parameters)
            {
                var parameterName = param.ParameterName;
                if (!IsParamAFunction(parameterName))
                { 
                    var inputObject = workItem.Input.GetReflector().GetValue(parameterName);
                    parameters.Add(inputObject);
                }
                else
                {
                    var ruleFunction = _ruleSetting.RuleFunction.FirstOrDefault(x => x.Name == parameterName);
                    if (ruleFunction != null)
                    {
                        foreach (var ruleParam in ruleFunction.Parameters)
                        {
                            var inputObject = workItem.Input.GetReflector().GetValue(ruleParam.ParameterName);
                            parameters.Add(inputObject);
                        }
                    }
                }
            }

            return parameters.ToArray();
        }

        private bool IsParamAFunction(string fieldName)
        {
            return _ruleSetting.RuleFunction.Any(x => x.Name == fieldName);
        }

        private string GetRuleId(string ruleName)
        {
            return _ruleSetting.RuleNode
                .SelectMany(node => node.Rules)
                .FirstOrDefault(r => r.RuleName == ruleName).RuleId;
        }


    }
}

