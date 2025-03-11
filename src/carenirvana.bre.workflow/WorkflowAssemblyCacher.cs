using System.Collections.Concurrent;
using System.Reflection;
using carenirvana.bre.common.ObjectFactory;
using carenirvana.bre.common.ObjectFactory.Impl;
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
                            "carenirvana.bre.engine.runtime",
                            "carenirvana.bre.engine.ruleexecutor",
                            "RuleExecutor");

            ruleExecutorInstance = _objectFactory.CreateInstance(
                                HelperFunctions.GetTypeFromAssembly(
                                "carenirvana.bre.engine.runtime",
                                "carenirvana.bre.engine.ruleexecutor",
                                "RuleExecutor"))();

            var excludedMethods = new[] { "ToString", "Equals", "GetHashCode", "GetType" };
            ruleExecutorType.GetMethods()
                .Where(m => !excludedMethods.Contains(m.Name))
                .ToList()
                .ForEach(x => GetMethod(x.Name));
        }

        private MethodInfo GetMethod(string methodName)
        {
            var key = $"{methodName}";
            if (_methodCache.TryGetValue(key, out var method))
            {
                return method;
            }

            method = ruleExecutorType?.GetMethod(methodName) ?? throw new InvalidOperationException($"Method '{methodName}' not found.");
            _methodCache[key] = method;
            return method;
        }

        public object? InvokeMethod(string methodName, params object[] parameters)
        {
            MethodInfo method = GetMethod(methodName);
            return method.Invoke(ruleExecutorInstance, parameters);
        }
    }
}
