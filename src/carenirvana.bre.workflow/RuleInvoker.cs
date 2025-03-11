using carenirvana.bre.model;
using carenirvana.bre.model.Impl;

namespace carenirvana.bre.workflow
{
    public class RuleInvoker(WorkflowAssemblyCacher workflowAssemblyCacher, RuleSetting ruleSetting)
    {
        private readonly WorkflowAssemblyCacher _workflowAssemblyCacher = workflowAssemblyCacher ?? throw new ArgumentNullException(nameof(workflowAssemblyCacher));
        private readonly RuleSetting _ruleSetting  = ruleSetting;

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
                UniqueId = workItem.Id,
                Result = bool.Parse(value.ToString()),
                RuleId = 1,
                RuleName = methodName,
                OutputMessage = ""
            };
            workItem.AddOutput(ruleOutput);

            Console.WriteLine($"{ruleOutput}");
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
                var inputObject = workItem.Input(param.ParameterName);
                parameters.Add(inputObject);
            }

            return parameters.ToArray();
        }
    }
}

