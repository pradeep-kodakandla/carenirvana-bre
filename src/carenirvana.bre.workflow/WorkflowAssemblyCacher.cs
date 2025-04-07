using System.Collections.Concurrent;
using System.Reflection;
using carenirvana.bre.common.ObjectFactory;
using carenirvana.bre.utility;

namespace carenirvana.bre.workflow
{
    public class WorkflowAssemblyCacher
    {
        private readonly ConcurrentDictionary<string, MethodInfo> _methodCache = new();
        private readonly IObjectFactory _objectFactory;
        private Type? ruleExecutorType;
        private object? ruleExecutorInstance;

        public WorkflowAssemblyCacher(IObjectFactory objectFactory)
        {
            _objectFactory = objectFactory;
            Init();
        }

        public string[] Methods => [.. _methodCache.Keys];

        private void Init()
        {
            ruleExecutorType = HelperFunctions.GetTypeFromAssembly(
                            ConstantsUtility.RunTimeNameSpace,
                            ConstantsUtility.RunTimeRuleExecutorTypeName,
                            ConstantsUtility.RuleExecutorTypeName);

            ruleExecutorInstance = _objectFactory.CreateInstance(ruleExecutorType)();
            CacheMethods();
        }

        private void CacheMethods()
        {
            foreach (var method in ruleExecutorType.GetMethods().Where(m => !ConstantsUtility.ExcludeFromMethodList.Contains(m.Name)))
            {
                _methodCache[method.Name] = method;
            }
        }

        private MethodInfo GetMethod(string methodName)
        {
            if (_methodCache.TryGetValue(methodName, out var method))
            {
                return method;
            }

            throw new InvalidOperationException($"Method '{methodName}' not found.");
        }

        public object? InvokeMethod(string methodName, params object[] parameters)
        {
            MethodInfo method = GetMethod(methodName);
            return method.Invoke(ruleExecutorInstance, parameters);
        }

        public object? InvokeMethodWithTypeConversion(string methodName, params object[] parameters)
        {
            MethodInfo method = GetMethod(methodName);

            var convertedParameters = method.GetParameters()
                .Select((param, index) => HelperFunctions.ConvertValue(parameters[index], param.ParameterType))
                .ToArray();

            return method.Invoke(ruleExecutorInstance, convertedParameters);
        }
    }
}
