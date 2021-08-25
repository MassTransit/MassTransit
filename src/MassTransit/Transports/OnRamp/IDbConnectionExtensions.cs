using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit
{
    public static class IDbConnectionExtensions
    {
        public static IDbTransaction BeginTransaction(this IDbConnection conn, Transports.Outbox.IOnRampDbTransactionContext accessor, IsolationLevel il = IsolationLevel.ReadCommitted)
        {
            var transaction = conn.BeginTransaction();
            accessor.SetTransaction(transaction);
            return transaction;
        }

        private static Task TryOpenAsync(this IDbConnection cnn, CancellationToken cancel = default)
        {
            if (cnn is DbConnection dbConn)
            {
                return dbConn.OpenAsync(cancel);
            }
            else
            {
                throw new InvalidOperationException("Async operations require use of a DbConnection or an already-open IDbConnection");
            }
        }

        public static int ExecuteNonQuery(this IDbConnection connection, string sql, IDbTransaction transaction = null,
            params object[] sqlParams)
        {
            bool wasClosed = connection.State == ConnectionState.Closed;

            try
            {
                if (wasClosed) connection.Open();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                foreach (var param in sqlParams)
                {
                    command.Parameters.Add(param);
                }

                if (transaction != null)
                {
                    command.Transaction = transaction;
                }

                return command.ExecuteNonQuery();
            }
            finally
            {
                if (wasClosed) connection.Close();
            }
        }

        public static async Task<int> ExecuteNonQueryAsync(this DbConnection connection, string sql, DbTransaction transaction = null,
            IEnumerable<object> sqlParams = null, CancellationToken cancellationToken = default)
        {
            bool wasClosed = connection.State == ConnectionState.Closed;

            try
            {
                if (wasClosed) await connection.TryOpenAsync(cancellationToken).ConfigureAwait(false);

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                foreach (var param in sqlParams ?? Array.Empty<object>())
                {
                    command.Parameters.Add(param);
                }

                if (transaction != null)
                {
                    command.Transaction = transaction;
                }

                return await command.ExecuteNonQueryAsync(cancellationToken);
            }
            finally
            {
                if (wasClosed) connection.Close();
            }
        }

        public static T ExecuteReader<T>(this IDbConnection connection, string sql, Func<IDataReader, T> readerFunc,
            IDbTransaction transaction = null, params object[] sqlParams)
        {
            bool wasClosed = connection.State == ConnectionState.Closed;

            try
            {
                if (wasClosed) connection.Open();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                foreach (var param in sqlParams)
                {
                    command.Parameters.Add(param);
                }

                if (transaction != null)
                {
                    command.Transaction = transaction;
                }

                IDataReader reader = command.ExecuteReader();

                T result = default;
                if (readerFunc != null)
                {
                    result = readerFunc(reader);
                }

                reader.Close();

                return result;
            }
            finally
            {
                if (wasClosed) connection.Close();
            }
            
        }

        public static async Task<T> ExecuteReaderAsync<T>(this DbConnection connection, string sql, Func<DbDataReader, Task<T>> readerFunc,
            DbTransaction transaction = null, IEnumerable<object> sqlParams = null, CancellationToken cancellationToken = default)
        {
            bool wasClosed = connection.State == ConnectionState.Closed;

            try
            {
                if (wasClosed) await connection.TryOpenAsync(cancellationToken).ConfigureAwait(false);

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                foreach (var param in sqlParams)
                {
                    command.Parameters.Add(param);
                }

                if (transaction != null)
                {
                    command.Transaction = transaction;
                }

                var reader = await command.ExecuteReaderAsync(cancellationToken);

                T result = default;
                if (readerFunc != null)
                {
                    result = await readerFunc(reader);
                }

                reader.Close();

                return result;
            }
            finally
            {
                if (wasClosed) connection.Close();
            }

        }

        public static T ExecuteScalar<T>(this IDbConnection connection, string sql, params object[] sqlParams)
        {
            bool wasClosed = connection.State == ConnectionState.Closed;

            try
            {
                if (wasClosed) connection.Open();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                foreach (var param in sqlParams)
                {
                    command.Parameters.Add(param);
                }

                object objValue = objValue = command.ExecuteScalar();

                T result = default;
                if (objValue != null)
                {
                    var returnType = typeof(T);
                    var converter = TypeDescriptor.GetConverter(returnType);
                    if (converter.CanConvertFrom(objValue.GetType()))
                    {
                        result = (T)converter.ConvertFrom(objValue);
                    }
                    else
                    {
                        result = (T)Convert.ChangeType(objValue, returnType);
                    }
                }

                return result;
            }
            finally
            {
                if (wasClosed) connection.Close();
            }
        }

        public static async Task<T> ExecuteScalarAsync<T>(this DbConnection connection, string sql, IEnumerable<object> sqlParams = null, CancellationToken cancellationToken = default)
        {
            bool wasClosed = connection.State == ConnectionState.Closed;

            try
            {
                if (wasClosed) await connection.TryOpenAsync(cancellationToken).ConfigureAwait(false);

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                foreach (var param in sqlParams)
                {
                    command.Parameters.Add(param);
                }

                object objValue = await command.ExecuteScalarAsync(cancellationToken);

                T result = default;
                if (objValue != null)
                {
                    var returnType = typeof(T);
                    var converter = TypeDescriptor.GetConverter(returnType);
                    if (converter.CanConvertFrom(objValue.GetType()))
                    {
                        result = (T)converter.ConvertFrom(objValue);
                    }
                    else
                    {
                        result = (T)Convert.ChangeType(objValue, returnType);
                    }
                }

                return result;
            }
            finally
            {
                if (wasClosed) connection.Close();
            }
        }

        public static IDbDataParameter CreateParameter(this IDbCommand cmd, string parameterName, object value)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = parameterName;
            param.Value = value;
            return param;
        }

        /// <summary>
        /// This will add an array of parameters to a SqlCommand. This is used for an IN statement.
        /// Use the returned value for the IN part of your SQL call. (i.e. SELECT * FROM table WHERE field IN ({paramNameRoot}))
        /// https://stackoverflow.com/a/2377651
        /// </summary>
        /// <param name="cmd">The SqlCommand object to allow the creation of parameters.</param>
        /// <param name="sql">The sql string for parameter replacement.</param>
        /// <param name="paramNameRoot">What the parameter should be named followed by a unique value for each value. This value surrounded by {} in the CommandText will be replaced.</param>
        /// <param name="values">The array of strings that need to be added as parameters.</param>
        /// <param name="dbType">One of the System.Data.DbType values. If null, determines type based on T.</param>
        /// <param name="size">The maximum size, in bytes, of the data within the column. The default value is inferred from the parameter value.</param>
        public static IDbDataParameter[] CreateArrayParameters<T>(this IDbCommand cmd, ref string sql, string paramNameRoot, IEnumerable<T> values, DbType? dbType = null, int? size = null)
        {
            /* An array cannot be simply added as a parameter to a SqlCommand so we need to loop through things and add it manually. 
             * Each item in the array will end up being it's own SqlParameter so the return value for this must be used as part of the
             * IN statement in the CommandText.
             */
            var parameters = new List<IDbDataParameter>();
            var parameterNames = new List<string>();
            var paramNbr = 1;
            foreach (var value in values)
            {
                var paramName = string.Format("@{0}{1}", paramNameRoot, paramNbr++);
                parameterNames.Add(paramName);
                var p = cmd.CreateParameter(paramName, value);
                if (dbType.HasValue)
                    p.DbType = dbType.Value;
                if (size.HasValue)
                    p.Size = size.Value;
                parameters.Add(p);
            }

            sql = sql.Replace("{" + paramNameRoot + "}", string.Join(",", parameterNames));

            return parameters.ToArray();
        }
    }
}
