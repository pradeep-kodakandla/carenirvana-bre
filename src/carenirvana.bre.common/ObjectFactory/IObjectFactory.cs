using System.Data;
using carenirvana.bre.common.ObjectFactory.Impl;

namespace carenirvana.bre.common.ObjectFactory
{
    public interface IObjectFactory
    {
        object GetInstanceOfAnObject(string assemblyName, string nameSpace, string typeName);

        object GetInstanceOfAnObject(string assemblyName, string nameSpace, string typeName, object[] args);

        object CallGenericMethodFromAnAssemblyUsingReflection(
                        Type assembly,
                        object[] constructorArgs,
                        string methodName,
                        object[] methodArgs,
                        Type typeOfObjectThatIsPassedToGenericMethod);

        List<T> GetDataFromReader<T>(IDataReader reader);

        Type GetType(object type);

        CreateObject CreateInstance(Type type);
    }
}
