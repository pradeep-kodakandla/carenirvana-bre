using System.Collections.Concurrent;
using System.Reflection;

namespace carenirvana.bre.utility.Extensions
{
    public static class ObjectExtensions
    {
        public static ConcurrentDictionary<string, PropertyInfo> ToProperties(this Type type)
        {
            var properties = type.GetProperties().Where(
                (prop) => prop.MemberType == MemberTypes.Property).Aggregate(
                    new ConcurrentDictionary<string, PropertyInfo>(),
                    (dictionary, prop) =>
                    {
                        if (!dictionary.TryAdd(prop.Name, prop))
                            throw new InvalidOperationException($"Property {prop.Name} already exists in the dictionary.");
                        return dictionary;
                    });
            return properties;
        }
    }
}
