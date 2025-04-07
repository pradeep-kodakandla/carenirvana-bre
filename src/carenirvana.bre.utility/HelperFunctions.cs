using System.Collections.Concurrent;

namespace carenirvana.bre.utility
{
    public class HelperFunctions
    {
        private static readonly ConcurrentDictionary<string, Type> Assemblies = new();

        public static Type GetOriginalType(string stringValue)
        {
            if (int.TryParse(stringValue, out _))
            {
                return typeof(int);
            }
            else if (bool.TryParse(stringValue, out _))
            {
                return typeof(bool);
            }
            else if (double.TryParse(stringValue, out _))
            {
                return typeof(double);
            }
            else if (DateTime.TryParse(stringValue, out _))
            {
                return typeof(DateTime);
            }
            else
            {
                return typeof(string);
            }
        }

        public static object ConvertValue(object value, Type type)
        {
            if (value == null) return null;
            var valueToString = value.ToString();
            if (type.Equals(typeof(int)))
            {
                return int.Parse(valueToString);
            }
            else if (type.Equals(typeof(bool)))
            {
                return bool.Parse(valueToString);
            }
            else if (type.Equals(typeof(double)))
            {
                return double.Parse(valueToString);
            }
            else if (type.Equals(typeof(DateTime)))
            {
                return DateTime.Parse(valueToString);
            }
            else
            {
                return valueToString;
            }
        }

        public static Type GetTypeFromString(string type)
        {
            if (type == "int")
            {
                return typeof(int);
            }
            else if (type == "bool")
            {
                return typeof(bool);
            }
            else if (type == "double")
            {
                return typeof(double);
            }
            else if (type == "datetime")
            {
                return typeof(DateTime);
            }
            else
            {
                return typeof(string);
            }
        }

        public static string AppendRuleFuncToMethodBodyIfExists(string methodBody)
        {
            var values = Enum.GetValues(typeof(RuleFunctionName)).Cast<RuleFunctionName>();
            foreach (var value in values)
            {
                if (methodBody.Contains(value.ToString()))
                {
                    methodBody = methodBody.Replace(value.ToString(), $"RuleFunctionInternal.{value}");
                }
            }

            return methodBody;
        }

        public static string ParseLoginExpression(string logicCondition)
        {
            if (logicCondition.Equals("and", StringComparison.CurrentCultureIgnoreCase))
            {
                return "&&";
            }
            else if (logicCondition.Equals("or", StringComparison.CurrentCultureIgnoreCase))
            {
                return "||";
            }
            else
            {
                return string.Empty;
            }
        }

        public static Type GetTypeFromAssembly(string assemblyName, string nameSpace, string typeName)
        {
            return Assemblies.GetOrAdd(
                $"{nameSpace}.{typeName}",
               type =>
               {
                   var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                   return assemblies
                        .First(a => a.GetName().Name.Equals(assemblyName))
                        .DefinedTypes.FirstOrDefault(d => d.FullName.Equals($"{nameSpace}.{typeName}"));
               });
        }
    }
}
