using carenirvana.bre.model;
using System.Collections.Concurrent;

namespace carenirvana.bre.repository
{
    public interface IRuleDataRepository
    {
        ConcurrentDictionary<int, IInputObject> GetInputDataAsync(
                    string query,
                    string assemblyName,
                    string nameSapceWhereTheClassObjectExists,
                    string typeName);

        IList<int> GetUniqueIds(string tableName, string uniqueIdColumnName);
    }
}