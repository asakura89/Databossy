using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace Databossy.NET20
{
    public class Database : IDisposable
    {
        private const String SQL_SERVER_PROVIDER = "System.Data.SqlClient";
        private DbConnection connection;
        private DbProviderFactory factory;
        private Byte openedConnectionCount = 0;

        public enum ConnectionStringType
        {
            CONNECTION_STRING,
            CONNECTION_STRING_NAME
        }

        public Database() : this(ConfigurationManager.ConnectionStrings[0].Name) { }

        public Database(String connectionString, String provider = SQL_SERVER_PROVIDER) : this(connectionString, ConnectionStringType.CONNECTION_STRING_NAME, provider) { }

        public Database(String connectionString, ConnectionStringType connectionStringType, String provider = SQL_SERVER_PROVIDER)
        {
            Open(connectionString, connectionStringType, provider);
        }

        private void Open(String connectionString, ConnectionStringType connectionStringType, String provider)
        {
            if (openedConnectionCount == 0)
            {
                factory = DbProviderFactories.GetFactory(provider);
                connection = factory.CreateConnection();
                if (connection == null)
                    throw new Exception("Connection creation from factory failed.");

                switch (connectionStringType)
                {
                    case ConnectionStringType.CONNECTION_STRING:
                        connection.ConnectionString = connectionString;
                        break;
                    case ConnectionStringType.CONNECTION_STRING_NAME:
                        String connString = ConfigurationManager.ConnectionStrings[connectionString].ConnectionString;
                        connection.ConnectionString = connString;
                        break;
                }

                connection.Open();
            }

            openedConnectionCount++;
        }

        private void Close()
        {
            if (openedConnectionCount > 0)
            {
                openedConnectionCount--;
                if (openedConnectionCount == 0)
                {
                    connection.Close();
                    connection.Dispose();
                    connection = null;
                }
            }
        }

        private DbCommand BuildSqlCommand(String queryString)
        {
            DbCommand builtSqlCommand = connection.CreateCommand();
            builtSqlCommand.CommandText = queryString;
            builtSqlCommand.CommandType = CommandType.Text;

            return builtSqlCommand;
        }

        private DbCommand BuildSqlCommand(String queryString, Object[] args)
        {
            DbCommand builtSqlCommand = connection.CreateCommand();
            builtSqlCommand.CommandText = queryString;
            builtSqlCommand.CommandType = CommandType.Text;
            BuildSqlCommandParameter(ref builtSqlCommand, args);

            return builtSqlCommand;
        }

        private void BuildSqlCommandParameter(ref DbCommand builtSqlCommand, Object[] queryParameters)
        {
            builtSqlCommand.Parameters.Clear();
            for (Int32 paramIdx = 0; paramIdx < queryParameters.Length; paramIdx++)
            {
                Object currentArgs = queryParameters[paramIdx] ?? DBNull.Value;
                DbParameter param = builtSqlCommand.CreateParameter();
                param.ParameterName = paramIdx.ToString();
                param.Value = currentArgs;
                builtSqlCommand.Parameters.Add(param);
            }
        }

        private DbDataAdapter BuildSelectDataAdapter(DbCommand builtSqlCommand)
        {
            DbDataAdapter builtDataAdapter = factory.CreateDataAdapter();
            if (builtDataAdapter == null)
                throw new Exception("Data Adapter creation from factory failed.");

            builtDataAdapter.SelectCommand = builtSqlCommand;
            return builtDataAdapter;
        }

        public DataTable Query(String queryString)
        {
            var dt = new DataTable();
            using (DbCommand cmd = BuildSqlCommand(queryString))
            {
                using (DbDataAdapter dataAdapter = BuildSelectDataAdapter(cmd))
                    dataAdapter.Fill(dt);
            }

            return dt;
        }

        public DataTable Query(String queryString, params Object[] args)
        {
            var dt = new DataTable();
            using (DbCommand cmd = BuildSqlCommand(queryString, args))
            {
                using (DbDataAdapter dataAdapter = BuildSelectDataAdapter(cmd))
                    dataAdapter.Fill(dt);
            }

            return dt;
        }

        public IEnumerable<T> Query<T>(String queryString)
        {
            DataTable dt = Query(queryString);
            IEnumerable<T> result = dt.ToIEnumerable<T>();

            return result;
        }

        public IEnumerable<T> Query<T>(String queryString, params Object[] args)
        {
            DataTable dt = Query(queryString, args);
            IEnumerable<T> result = dt.ToIEnumerable<T>();

            return result;
        }

        public T QuerySingle<T>(String queryString)
        {
            T result = Query<T>(queryString).FirstOrDefault();

            return result;
        }

        public T QuerySingle<T>(String queryString, params Object[] args)
        {
            T result = Query<T>(queryString, args).FirstOrDefault();

            return result;
        }

        public T QueryScalar<T>(String queryString)
        {
            using (DbCommand cmd = BuildSqlCommand(queryString))
                return (T)cmd.ExecuteScalar();
        }

        public T QueryScalar<T>(String queryString, params Object[] args)
        {
            using (DbCommand cmd = BuildSqlCommand(queryString, args))
                return (T)cmd.ExecuteScalar();
        }

        public Int32 Execute(String queryString, params Object[] args)
        {
            Int32 result = -1;
            using (DbCommand cmd = BuildSqlCommand(queryString, args))
            {
                cmd.Transaction = connection.BeginTransaction();
                try
                {
                    result = cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                }
                catch
                {
                    cmd.Transaction.Rollback();
                    throw;
                }
            }

            return result;
        }

        public Int32 Execute(String queryString)
        {
            Int32 result = -1;
            using (DbCommand cmd = BuildSqlCommand(queryString))
            {
                cmd.Transaction = connection.BeginTransaction();
                try
                {
                    result = cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                }
                catch
                {
                    cmd.Transaction.Rollback();
                    throw;
                }
            }

            return result;
        }

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }
    }
}
