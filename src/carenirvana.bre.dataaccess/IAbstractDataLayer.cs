using System.Data;

namespace carenirvana.bre.dataaccess
{
    public interface IAbstractDataLayer
    {
        /// <summary>
        /// Gets connection string
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// This method takes the input as the query and returns a data reader
        /// </summary>
        /// <param name="query">query to be executed</param>
        /// <returns></returns>
        IDataReader ExecuteDataReader(string query);

        /// <summary>
        /// This methid takes the input as the query and returns a data reader
        /// </summary>
        /// <param name="query">query to be executed</param>
        /// <param name="parmeterWithValues">collection of parameters with values</param>
        /// <returns></returns>
        IDataReader ExecuteDataReader(string query, Dictionary<string, object> parmeterWithValues);

        /// <summary>
        /// This method takes the input as the query and returns a data reader
        /// </summary>
        /// <param name="query">query to be executed</param>
        /// <returns></returns>
        object ExecuteScalar(string query);

        /// <summary>
        /// This methid takes the input as the query and returns a data reader
        /// </summary>
        /// <param name="query">query to be executed</param>
        /// <param name="parmeterWithValues">collection of parameters with values</param>
        /// <returns></returns>
        object ExecuteScalar(string query, Dictionary<string, object> parmeterWithValues);

        /// <summary>
        /// This method takes the input as the query and returns a data reader
        /// </summary>
        /// <param name="query">query to be executed</param>
        /// <returns></returns>
        int ExecuteNonQuery(string query);

        /// <summary>
        /// This methid takes the input as the query and returns a data reader
        /// </summary>
        /// <param name="query">query to be executed</param>
        /// <param name="parmeterWithValues">collection of parameters with values</param>
        /// <returns></returns>
        int ExecuteNonQuery(string query, Dictionary<string, object> parmeterWithValues);
    }
}
