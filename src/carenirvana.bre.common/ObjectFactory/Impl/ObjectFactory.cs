using System.Collections.Concurrent;
using System.Data;
using System.Reflection.Emit;
using carenirvana.bre.utility;

namespace carenirvana.bre.common.ObjectFactory.Impl
{
    public delegate object CreateObject();

    public class ObjectFactory : IObjectFactory
    {
        private readonly ConcurrentDictionary<Type, Lazy<CreateObject>> _objectActivators = new();
        private readonly Type createObjectDelegateType = typeof(CreateObject);

        public CreateObject CreateInstance(Type type)
        {
            var activator = _objectActivators.GetOrAdd(type, t =>
            {
                return new Lazy<CreateObject>(() =>
                {
                    var dynamicMethod = new DynamicMethod(
                        "DM$OBJ_FACTORY_" + t.Name,
                        typeof(object),
                        null,
                        t);
                    var msil = dynamicMethod.GetILGenerator();
                    msil.Emit(OpCodes.Newobj, t.GetConstructor(Type.EmptyTypes));
                    msil.Emit(OpCodes.Ret);
                    return (CreateObject)dynamicMethod.CreateDelegate(createObjectDelegateType);
                });
            });
            return activator.Value;
        }

        public object GetInstanceOfAnObject(string assemblyName, string nameSpace, string typeName)
        {
            return Activator.CreateInstance(
                    HelperFunctions.GetTypeFromAssembly(assemblyName, nameSpace, typeName));
        }

        public object GetInstanceOfAnObject(string assemblyName, string nameSpace, string typeName, object[] args)
        {
            return Activator.CreateInstance(
                    HelperFunctions.GetTypeFromAssembly(assemblyName, nameSpace, typeName), args);
        }

        public object CallGenericMethodFromAnAssemblyUsingReflection(
            Type assembly,
            object[] constructorArgs,
            string methodName,
            object[] methodArgs,
            Type typeOfObjectThatIsPassedToGenericMethod)
        {
            var instance = Activator.CreateInstance(assembly, constructorArgs);
            var assemblyInternal = instance.GetType().Assembly;

            var genericMethod = assemblyInternal.GetType(instance.GetType().FullName)
                .GetMethod(methodName)
                .MakeGenericMethod(typeOfObjectThatIsPassedToGenericMethod);
            return genericMethod.Invoke(instance, methodArgs);
        }

        public List<T> GetDataFromReader<T>(IDataReader reader)
        {
            var result = new List<T>();
            var inputObjectType = typeof(T);
            var attributes = (from attrib in reader.GetSchemaTable().AsEnumerable()
                              select attrib.Field<string>("ColumnName")).ToList();
            while (reader.Read())
            {
                var values = new object[attributes.Count];
                reader.GetValues(values);
                result.Add(CreateInstanceWithData<T>(attributes, values));
            }

            if (!reader.IsClosed)
                reader.Close();

            return result;
        }

        public Type GetType(object type)
        {
            return type.GetType();
        }

        private T CreateInstanceWithData<T>(List<string> attributes, object[] values)
        {
            var instance = CreateInstance(typeof(T))();
            var type = instance.GetType();

            for (var i = 0; i < attributes.Count; i++)
            {
                var attribute = attributes[i];
                if (!Equals(values[i], DBNull.Value))
                {
                    type.GetProperty(attribute)?.SetValue(instance, values[i], null);
                }
            }
            return (T)instance;
        }
    }
}
