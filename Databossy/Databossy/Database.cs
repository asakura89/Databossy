﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Databossy
{
    public class Database : IDisposable
    {
        private const String SqlServerProvider = "System.Data.SqlClient";
        private DbConnection connection;
        private DbProviderFactory factory;
        private Byte openedConnectionCount = 0;

        public enum ConnectionStringType
        {
            ConnectionString,
            ConnectionStringName
        }

        public Database() : this(ConfigurationManager.ConnectionStrings[0].Name) { }

        public Database(String connectionString, String provider = SqlServerProvider) : this(connectionString, ConnectionStringType.ConnectionStringName, provider) { }

        public Database(String connectionString, ConnectionStringType connectionStringType, String provider = SqlServerProvider)
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
                    case ConnectionStringType.ConnectionString:
                        connection.ConnectionString = connectionString;
                        break;
                    case ConnectionStringType.ConnectionStringName:
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

        private DbCommand BuildSqlCommand(String queryString, Object[] queryParams)
        {
            DbCommand builtSqlCommand = connection.CreateCommand();
            builtSqlCommand.CommandText = queryString;
            builtSqlCommand.CommandType = CommandType.Text;
            BuildSqlCommandParameter(ref builtSqlCommand, queryParams);

            return builtSqlCommand;
        }

        /*private DbCommand BuildSqlCommand<TParam>(String queryString, TParam paramObj)
        {
            DbCommand builtSqlCommand = connection.CreateCommand();
            builtSqlCommand.CommandText = queryString;
            builtSqlCommand.CommandType = CommandType.Text;
            BuildSqlCommandParameter(ref builtSqlCommand, paramObj);

            return builtSqlCommand;
        }*/

        private void BuildSqlCommandParameter(ref DbCommand builtSqlCommand, Object[] queryParams)
        {
            builtSqlCommand.Parameters.Clear();
            for (Int32 paramIdx = 0; paramIdx < queryParams.Length; paramIdx++)
            {
                Object currentqueryParams = queryParams[paramIdx] ?? DBNull.Value;
                DbParameter param = builtSqlCommand.CreateParameter();
                param.ParameterName = paramIdx.ToString();
                param.Value = currentqueryParams;
                builtSqlCommand.Parameters.Add(param);
            }
        }

        /*private void BuildSqlCommandParameter<TParam>(ref DbCommand builtSqlCommand, TParam paramObj)
        {
            builtSqlCommand.Parameters.Clear();
            PropertyInfo[] properties = paramObj.GetType().GetProperties();
            foreach (PropertyInfo currentProperty in properties)
            {
                Object currentParam = currentProperty.GetValue(paramObj, null) ?? DBNull.Value;
                DbParameter param = builtSqlCommand.CreateParameter();
                param.ParameterName = currentProperty.Name;
                param.Value = currentParam;
                builtSqlCommand.Parameters.Add(param);
            }
        }*/

        private DbDataAdapter BuildSelectDataAdapter(DbCommand builtSqlCommand)
        {
            DbDataAdapter builtDataAdapter = factory.CreateDataAdapter();
            if (builtDataAdapter == null)
                throw new Exception("Data Adapter creation from factory failed.");

            builtDataAdapter.SelectCommand = builtSqlCommand;
            return builtDataAdapter;
        }

        public DataTable QueryDataTable(String queryString)
        {
            var dt = new DataTable();
            using (DbCommand cmd = BuildSqlCommand(queryString))
            {
                using (DbDataAdapter dataAdapter = BuildSelectDataAdapter(cmd))
                    dataAdapter.Fill(dt);
            }

            return dt;
        }

        public DataTable QueryDataTable(String queryString, params Object[] queryParams)
        {
            var dt = new DataTable();
            using (DbCommand cmd = BuildSqlCommand(queryString, queryParams))
            {
                using (DbDataAdapter dataAdapter = BuildSelectDataAdapter(cmd))
                    dataAdapter.Fill(dt);
            }

            return dt;
        }

        /*public DataTable QueryDataTable<TParam>(String queryString, TParam paramObj)
        {
            var dt = new DataTable();
            using (DbCommand cmd = BuildSqlCommand(queryString, paramObj))
            {
                using (DbDataAdapter dataAdapter = BuildSelectDataAdapter(cmd))
                    dataAdapter.Fill(dt);
            }

            return dt;
        }*/

        public DataSet QueryDataSet(String queryString)
        {
            var ds = new DataSet();
            using (DbCommand cmd = BuildSqlCommand(queryString))
            {
                using (DbDataAdapter dataAdapter = BuildSelectDataAdapter(cmd))
                    dataAdapter.Fill(ds);
            }

            return ds;
        }

        public DataSet QueryDataSet(String queryString, params Object[] queryParams)
        {
            var ds = new DataSet();
            using (DbCommand cmd = BuildSqlCommand(queryString, queryParams))
            {
                using (DbDataAdapter dataAdapter = BuildSelectDataAdapter(cmd))
                    dataAdapter.Fill(ds);
            }

            return ds;
        }
        /*public DataSet QueryDataSet<TParam>(String queryString, TParam paramObj)
        {
            var ds = new DataSet();
            using (DbCommand cmd = BuildSqlCommand(queryString, paramObj))
            {
                using (DbDataAdapter dataAdapter = BuildSelectDataAdapter(cmd))
                    dataAdapter.Fill(ds);
            }

            return ds;
        }*/

        public IEnumerable<T> Query<T>(String queryString)
        {
            DataTable dt = QueryDataTable(queryString);
            IEnumerable<T> result = ToIEnumerable<T>(dt);

            return result;
        }

        public IEnumerable<T> Query<T>(String queryString, params Object[] queryParams)
        {
            DataTable dt = QueryDataTable(queryString, queryParams);
            IEnumerable<T> result = ToIEnumerable<T>(dt);

            return result;
        }

        /*public IEnumerable<T> Query<T, TParam>(String queryString, TParam paramObj)
        {
            DataTable dt = QueryDataTable(queryString, paramObj);
            IEnumerable<T> result = dt.ToIEnumerable<T>();

            return result;
        }*/

        public T QuerySingle<T>(String queryString)
        {
            T result = Query<T>(queryString).FirstOrDefault();

            return result;
        }

        public T QuerySingle<T>(String queryString, params Object[] queryParams)
        {
            T result = Query<T>(queryString, queryParams).FirstOrDefault();

            return result;
        }

        /*public T QuerySingle<T, TParam>(String queryString, TParam paramObj)
        {
            T result = Query<T>(queryString, paramObj).FirstOrDefault();

            return result;
        }*/

        public T QueryScalar<T>(String queryString)
        {
            using (DbCommand cmd = BuildSqlCommand(queryString))
                return (T)cmd.ExecuteScalar();
        }

        public T QueryScalar<T>(String queryString, params Object[] queryParams)
        {
            using (DbCommand cmd = BuildSqlCommand(queryString, queryParams))
                return (T)cmd.ExecuteScalar();
        }

        /*public T QueryScalar<T, TParam>(String queryString, TParam paramObj)
        {
            using (DbCommand cmd = BuildSqlCommand(queryString, paramObj))
                return (T)cmd.ExecuteScalar();
        }*/

        public Int32 Execute(String queryString)
        {
            Int32 result = -1;
            using (DbCommand cmd = BuildSqlCommand(queryString))
                result = cmd.ExecuteNonQuery();

            return result;
        }

        public Int32 Execute(String queryString, params Object[] queryParams)
        {
            Int32 result = -1;
            using (DbCommand cmd = BuildSqlCommand(queryString, queryParams))
                result = cmd.ExecuteNonQuery();

            return result;
        }

        /*public Int32 Execute<TParam>(String queryString, TParam paramObj)
        {
            Int32 result = -1;
            using (DbCommand cmd = BuildSqlCommand(queryString, paramObj))
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
        }*/

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }

        private IEnumerable<TResult> ToIEnumerable<TResult>(DataTable dt)
        {
            foreach (DataRow dataRow in dt.Rows)
            {
                var t = Activator.CreateInstance<TResult>();
                Type tType = typeof(TResult);
                PropertyInfo[] tProperties = tType.GetProperties();
                FieldInfo[] tFields = tType.GetFields();

                if (tProperties.Length != 0)
                {
                    foreach (PropertyInfo property in tProperties)
                    {
                        Type propertyType = property.PropertyType;

                        if (dataRow[property.Name] != DBNull.Value)
                            property.SetValue(t, Convert.ChangeType(dataRow[property.Name], propertyType), null);
                    }
                }
                else
                {
                    foreach (FieldInfo field in tFields)
                    {
                        Type fieldType = field.FieldType;

                        if (dataRow[field.Name] != DBNull.Value)
                            field.SetValue(t, Convert.ChangeType(dataRow[field.Name], fieldType));
                    }
                }

                yield return t;
            }
        }
    }
}
