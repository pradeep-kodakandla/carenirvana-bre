using System.Collections.Concurrent;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using carenirvana.bre.utility;

namespace carenirvana.bre.common.ObjectFactory.Impl
{
    public delegate object CreateObject();

    public class ObjectFactory : IObjectFactory
    {
        private readonly ConcurrentDictionary<Type, Lazy<CreateObject>> _objectActivators = new();
        private readonly Type _createObjectDelegateType = typeof(CreateObject);

        public CreateObject CreateInstance(Type type)
        {
            var activator = _objectActivators.GetOrAdd(type, t => new Lazy<CreateObject>(() => CreateActivator(t)));
            return activator.Value;
        }

        public object GetInstanceOfAnObject(string assemblyName, string nameSpace, string typeName)
        {
            var type = HelperFunctions.GetTypeFromAssembly(assemblyName, nameSpace, typeName);
            return Activator.CreateInstance(type);
        }

        public object GetInstanceOfAnObject(string assemblyName, string nameSpace, string typeName, object[] args)
        {
            var type = HelperFunctions.GetTypeFromAssembly(assemblyName, nameSpace, typeName);
            return Activator.CreateInstance(type, args);
        }

        public object CallGenericMethodFromAnAssemblyUsingReflection(
            Type assembly,
            object[] constructorArgs,
            string methodName,
            object[] methodArgs,
            Type typeOfObjectThatIsPassedToGenericMethod)
        {
            var instance = Activator.CreateInstance(assembly, constructorArgs);
            var genericMethod = GetGenericMethod(instance, methodName, typeOfObjectThatIsPassedToGenericMethod);
            return genericMethod.Invoke(instance, methodArgs);
        }

        public List<T> GetDataFromReader<T>(IDataReader reader)
        {
            var result = new List<T>();
            var attributes = GetColumnNames(reader);
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

        private CreateObject CreateActivator(Type type)
        {
            var dynamicMethod = new DynamicMethod(
                "DM$OBJ_FACTORY_" + type.Name,
                typeof(object),
                null,
                type);
            var msil = dynamicMethod.GetILGenerator();
            msil.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            msil.Emit(OpCodes.Ret);
            return (CreateObject)dynamicMethod.CreateDelegate(_createObjectDelegateType);
        }

        private MethodInfo GetGenericMethod(object instance, string methodName, Type genericType)
        {
            var type = instance.GetType();
            var method = type.GetMethod(methodName);
            return method.MakeGenericMethod(genericType);
        }

        private List<string> GetColumnNames(IDataReader reader)
        {
            return (from attrib in reader.GetSchemaTable().AsEnumerable()
                    select attrib.Field<string>("ColumnName")).ToList();
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