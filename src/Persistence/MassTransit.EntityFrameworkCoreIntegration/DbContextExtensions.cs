namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Collections.Concurrent;

    using Microsoft.EntityFrameworkCore;


    public static class DbContextExtensions
    {
        private static readonly ConcurrentDictionary<Type, string> TableNames = new ConcurrentDictionary<Type, string>();

        public static string GetTableName<T>(this DbContext context) where T : class
        {
            return GetTableName(context, typeof(T));
        }

        public static string GetTableName(this DbContext context, Type t)
        {
            string result;

            if (!TableNames.TryGetValue(t, out result))
            {
                var sql = context.Model.FindEntityType(t).SqlServer();
                var sqlSchema = sql.Schema;
                if (string.IsNullOrEmpty(sqlSchema))
                {
                    sqlSchema = "dbo";
                }

                result = $"[{sqlSchema}].[{sql.TableName}]";

                if(!string.IsNullOrWhiteSpace(result))
                {
                    TableNames.TryAdd(t, result);
                }
                else
                {
                    throw new MassTransitException("Couldn't determine table and schema name (using model metadata).");
                }
            }

            return result;

        }
    }
}