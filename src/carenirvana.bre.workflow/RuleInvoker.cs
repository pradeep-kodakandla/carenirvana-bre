using carenirvana.bre.model;
using carenirvana.bre.model.Impl;
using carenirvana.bre.utility;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace carenirvana.bre.workflow
{
    public class RuleInvoker(WorkflowAssemblyCacher workflowAssemblyCacher, RuleSetting ruleSetting, int runId)
    {
        private readonly WorkflowAssemblyCacher _workflowAssemblyCacher = workflowAssemblyCacher ?? throw new ArgumentNullException(nameof(workflowAssemblyCacher));
        private readonly RuleSetting _ruleSetting  = ruleSetting;
        private readonly int runId = runId;

        public void InvokeRuleMethod(IWorkflowItem workItem)
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

        public object InvokeRuleMethod(string methodName, IWorkflowItem workItem)
        {
            try
            {
                var value = _workflowAssemblyCacher.InvokeMethod(methodName, BuildParametersForRule(workItem, methodName));
                BuildOutput(value, workItem, methodName);
                return value;
            }
            catch (Exception ex)
            {
                HandleMethodInvocationError(workItem, methodName, ex);
            }
            return false;
        }

        public RuleOutput InvokeRuleMethod(string methodName, object[] parameters)
        {
            try
            {
                var output = _workflowAssemblyCacher.InvokeMethodWithTypeConversion(methodName, parameters);
                var ruleOutput = new RuleOutput
                {
                    RunId = runId,
                    UniqueId = 0,
                    Result = bool.Parse(output.ToString()),
                    RunDtTm = DateTime.Now.Date,
                    RuleId = int.Parse(GetRuleId(methodName)), // need to find the rule id...
                    RuleName = methodName,
                    OutputMessage = ""
                };
                return ruleOutput;
            }
            catch (Exception ex)
            {
                HandleMethodInvocationError(methodName, ex);
            }
            return null;
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
        }

        private void HandleMethodInvocationError(IWorkflowItem workItem, string methodName, Exception ex)
        {
            // Log or handle the error as needed
            Console.WriteLine($"Error invoking method {methodName} for work item {workItem.Id}: {ex.Message}");
        }

        private void HandleMethodInvocationError(string methodName, Exception ex)
        {
            // Log or handle the error as needed
            Console.WriteLine($"Error invoking method {methodName}: {ex.Message}");
        }

        private object[] BuildParametersForRule(IWorkflowItem workItem, string methodName)
        {
            var rule = _ruleSetting.RuleNode
                .SelectMany(node => node.Rules)
                .FirstOrDefault(r => r.RuleName == methodName) ?? throw new InvalidOperationException($"Rule '{methodName}' not found in RuleSetting.");
            
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

            return [.. parameters];
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

