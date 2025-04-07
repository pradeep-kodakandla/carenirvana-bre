using System.Collections.Concurrent;
using System.Data;
using System.Text;
using carenirvana.bre.common;
using carenirvana.bre.model;
using carenirvana.bre.model.Impl;
using Npgsql;
using Npgsql.Internal;

namespace carenirvana.bre.dataaccess.Impl.Postgres
{
    public class PostgresDataLayer : IAbstractDataLayer
    {
        private readonly string connectionString;
        private readonly int timeOut;
        private readonly int retryCount;

        public PostgresDataLayer(
                string serverName,
                string databaseName,
                string userName,
                string password,
                int port,
                int timeOut = 180,
                int retryCount = 3)
        {
            connectionString = BuildConnectionString(serverName, databaseName, userName, password, port);
            this.timeOut = timeOut;
            this.retryCount = retryCount;
        }

        public PostgresDataLayer(
                string connectionString,
                int timeOut = 180,
                int retryCount = 3)
        {
            this.connectionString = connectionString;
            this.timeOut = timeOut;
            this.retryCount = retryCount;
        }

        public string ConnectionString => connectionString;

        public int ExecuteNonQuery(string query)
        {
            using var command = BuildCommandWithParameters(CommandType.Text, query, []);
            var returnValue = new ComputationRetryer(retryCount, query).Run<Exception, int>(() =>
                                        command.ExecuteNonQuery());
            command.Connection?.Close();
            return returnValue;
        }

        public int ExecuteNonQuery(string query, Dictionary<string, object> parmeterWithValues)
        {
            using var command = BuildCommandWithParameters(CommandType.Text, query, parmeterWithValues);
            var returnValue = new ComputationRetryer(retryCount, query).Run<Exception, int>(() =>
                                    command.ExecuteNonQuery());
            command.Connection?.Close();
            return returnValue;
        }

        public IDataReader ExecuteDataReader(string query)
        {
            var command = BuildCommandWithParameters(CommandType.Text, query, []);
            var returnValue = new ComputationRetryer(retryCount, query).Run<Exception, IDataReader>(() =>
                                    command.ExecuteReader(CommandBehavior.CloseConnection));
            return returnValue;
        }

        public IDataReader ExecuteDataReader(string query, Dictionary<string, object> parmeterWithValues)
        {
            var command = BuildCommandWithParameters(CommandType.Text, query, parmeterWithValues);
            var returnValue = new ComputationRetryer(retryCount, query).Run<Exception, IDataReader>(() =>
                                    command.ExecuteReader(CommandBehavior.CloseConnection));
            return returnValue;
        }

        public object ExecuteScalar(string query)
        {
            using var command = BuildCommandWithParameters(CommandType.Text, query, []);
            var returnValue = new ComputationRetryer(retryCount, query).Run<Exception, object>(() =>
                                        command.ExecuteScalar());
            command.Connection?.Close();
            return returnValue;
        }

        public object ExecuteScalar(string query, Dictionary<string, object> parmeterWithValues)
        {
            using var command = BuildCommandWithParameters(CommandType.Text, query, parmeterWithValues);
            var returnValue = new ComputationRetryer(retryCount, query).Run<Exception, object>(() =>
                                            command.ExecuteScalar());
            command.Connection?.Close();
            return returnValue;
        }

        public void BulkInsert(ConcurrentQueue<IWorkflowItem> workflowItems, string destTableName)
        {
            using var conn = Connection();


            using var writer = conn.BeginBinaryImport($"COPY {destTableName} (RunId, UniqueId, RuleId, RuleName, RunDtTm, Result, OutputMessage) FROM STDIN (FORMAT BINARY)");

            while (workflowItems.TryDequeue(out var workflowItem))
            {
                foreach (var output in workflowItem.Outputs)
                {
                    writer.WriteRow(GetOutputsAsArray(output));
                }
            }
            writer.Complete();
        }

        private string GetOutputsAsCsv(IWorkflowItem workflowItem)
        {
            var csvBuilder = new StringBuilder();
            foreach (var output in workflowItem.Outputs)
            {
                var outputArray = GetOutputsAsArray(output);
                var quotedValues = outputArray.Select(value => $"'{value}'");
                csvBuilder.AppendLine(string.Join(",", quotedValues));
            }
            return csvBuilder.ToString();
        }

        private object[] GetOutputsAsArray(RuleOutput output)
        {
            return
            [
                output.RunId,
                output.UniqueId,
                output.RuleId,
                output.RuleName,
                output.RunDtTm,
                output.Result,
                output.OutputMessage
            ];
        }

        private string BuildConnectionString(string serverName, string databaseName, string userName, string password, int port)
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                ApplicationName = nameof(iCare4H),
                Host = serverName,
                Database = databaseName,
                Port = port,
                Username = userName,
                Password = password,
                Timeout = timeOut,
                SslMode = SslMode.Require
            };

            return connectionStringBuilder.ToString();
        }

        private NpgsqlConnection Connection()
        {
            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        private IDbCommand BuildCommandWithParameters(
                CommandType commandType,
                string query,
                Dictionary<string, object> parameterWithValues)
        {
            var command = new NpgsqlCommand
            {
                CommandText = query,
                CommandType = commandType,
                CommandTimeout = timeOut,
                Connection = Connection()
            };

            foreach (var parameter in parameterWithValues)
            {
                NpgsqlParameter npgsqlParameter;
                if (IsCommaSeparated(parameter.Value))
                {
                    var values = Convert.ToString(parameter.Value).Split(',');
                    var intermediateParameters = new string[values.Length];

                    for (var i = 0; i < values.Length; i++)
                    {
                        if (values[i] != null)
                        {
                            intermediateParameters[i] = parameter.Key + i.ToString();
                            npgsqlParameter = new NpgsqlParameter(
                                        intermediateParameters[i],
                                        values[i]);
                            command.Parameters.Add(npgsqlParameter);
                        }
                    }
                    query = query.Replace(parameter.Key,
                                    string.Join(",", intermediateParameters));
                }
                else
                {
                    npgsqlParameter = new NpgsqlParameter(parameter.Key, parameter.Value);
                    command.Parameters.Add(npgsqlParameter);
                }
            }

            return command;
        }

        private static bool IsCommaSeparated(object parameterWithValues)
        {
            return parameterWithValues.GetType() == typeof(string)
                && Convert.ToString(parameterWithValues).Contains(',');
        }
    }
}
