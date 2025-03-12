using System;
using System.Collections.Concurrent;
using System.Data;
using carenirvana.bre.common.ObjectFactory;
using carenirvana.bre.dataaccess;
using carenirvana.bre.model;
using carenirvana.bre.utility;

namespace carenirvana.bre.repository.Impl
{
    public class RuleDataRepository(IAbstractDataLayer dataLayer, IObjectFactory objectFactory) : IRuleDataRepository
    {
        private readonly IAbstractDataLayer dataLayer = dataLayer;
        private readonly IObjectFactory objectFactory = objectFactory;

        public ConcurrentDictionary<int, IInputObject> GetInputDataAsync(
                    string query,
                    string assemblyName,
                    string nameSapceWhereTheClassObjectExists,
                    string typeName)
        {
            return GetInputDataInternal(query, assemblyName, nameSapceWhereTheClassObjectExists, typeName);
        }

        public IList<int> GetUniqueIds(string tableName, string uniqueIdColumnName)
        {
            List<int> result = new List<int>();
            var query = $"select {uniqueIdColumnName} from {tableName}";
            using IDataReader reader = dataLayer.ExecuteDataReader(query);
            while (reader.Read())
            {
                if (!reader.IsDBNull(0))
                {
                    result.Add(reader.GetInt32(0));
                }
            }
            return result;
        }

        public void BulkInsert(IWorkflowItem workItem, string destTableName)
        {
            dataLayer.BulkInsert(workItem, destTableName);
        }

        private ConcurrentDictionary<int, IInputObject> GetInputDataInternal(
                    string query,
                    string assemblyName,
                    string nameSapceWhereTheClassObjectExists,
                    string typeName)
        {
            using IDataReader dataReader = dataLayer.ExecuteDataReader(query);
            return GetDataFromDataReader(
                dataReader, assemblyName, nameSapceWhereTheClassObjectExists, typeName);
        }

        private ConcurrentDictionary<int, IInputObject> GetDataFromDataReader(
                    IDataReader reader,
                    string assemblyName,
                    string nameSapceWhereTheClassObjectExists,
                    string typeName)
        {
            var items = new ConcurrentDictionary<int, IInputObject>();
            var attributes = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();

            while (reader.Read())
            {
                var values = new object[attributes.Count];
                reader.GetValues(values);

                var domainObject = CreateInstanceWithData(attributes, values, assemblyName, nameSapceWhereTheClassObjectExists, typeName);
                items.TryAdd(domainObject.id, domainObject);
            }

            if (!reader.IsClosed)
            {
                reader.Close();
            }

            return items;
        }

        private IInputObject CreateInstanceWithData(
                List<string> attributes,
                object[] values,
                string assemblyName,
                string nameSapceWhereTheClassObjectExists,
                string typeName)
        {
            var item = (IInputObject)
                            objectFactory.CreateInstance(
                                HelperFunctions.GetTypeFromAssembly(assemblyName, nameSapceWhereTheClassObjectExists, typeName))();
            int i = 0;
            var reflector = item.GetReflector();
            foreach (var attribute in attributes)
            {
                if (!Equals(values[i], DBNull.Value))
                    reflector.SetValue(attribute, values[i]);
                i++;
            }
            // this is hack for uniqueid column assuming this is a first column...
            reflector.SetValue("id", values[0]);
            return item;
        }
    }
}
