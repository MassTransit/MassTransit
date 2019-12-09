// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Collections.Concurrent;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    public class MsSqlLockStatements : IRawSqlLockStatements
    {
        const string DefaultRowLockStatement = "select * from {0}.{1} WITH (UPDLOCK, ROWLOCK) WHERE CorrelationId = @p0";

        protected static readonly ConcurrentDictionary<Type, SchemaTablePair> TableNames = new ConcurrentDictionary<Type, SchemaTablePair>();

        public MsSqlLockStatements(
            string defaultSchema = "dbo",
            string rowLockStatement = DefaultRowLockStatement
            )
        {
            DefaultSchema = defaultSchema;
            RowLockStatement = rowLockStatement ?? throw new ArgumentNullException(nameof(rowLockStatement));
        }

        protected string DefaultSchema { get; }
        protected string RowLockStatement { get; }

        public virtual string GetRowLockStatement<TSaga>(DbContext context)
            where TSaga : class, ISaga
        {
            var schemaTablePair = GetSchemaAndTableName<TSaga>(context);

            return string.Format(RowLockStatement, schemaTablePair.Schema, schemaTablePair.Table);
        }

        private SchemaTablePair GetSchemaAndTableName<T>(DbContext context)
            where T : class
        {
            var t = typeof(T);

            if (!TableNames.TryGetValue(t, out var result))
            {
                var sql = context.Model.FindEntityType(t);

                result = new SchemaTablePair
                {
                    Schema = sql.GetSchema() ?? DefaultSchema,
                    Table = sql.GetTableName()
                };

                if (result != null
                    && !string.IsNullOrWhiteSpace(result.Schema)
                    && !string.IsNullOrWhiteSpace(result.Table))
                    TableNames.TryAdd(t, result);
                else
                    throw new MassTransitException("Couldn't determine table and schema name (using model metadata).");
            }

            return result;
        }

        public class SchemaTablePair
        {
            public string Schema { get; set; }
            public string Table { get; set; }
        }
    }
}
