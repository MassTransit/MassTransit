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
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;


    public class EntityFrameworkMetadataHelper :
        IRelationalEntityMetadataHelper
    {
        protected static readonly ConcurrentDictionary<Type, string> TableNames = new ConcurrentDictionary<Type, string>();

        public EntityFrameworkMetadataHelper(string schemaTableFormat = "{0}.{1}", string defaultSchema = "dbo",
            Func<IEntityType, IRelationalEntityTypeAnnotations> relationalEntityTypeAnnotations = null)
        {
            SchemaTableFormat = schemaTableFormat;
            DefaultSchema = defaultSchema;
            RelationalEntityTypeAnnotations = relationalEntityTypeAnnotations ?? GetRelationalEntityTypeAnnotations;
        }

        protected string SchemaTableFormat { get; }
        protected string DefaultSchema { get; }
        protected Func<IEntityType, IRelationalEntityTypeAnnotations> RelationalEntityTypeAnnotations { get; }

        public virtual string GetTableName<T>(DbContext context)
            where T : class
        {
            var t = typeof(T);

            string result;

            if (!TableNames.TryGetValue(t, out result))
            {
                var sql = GetRelationalEntityTypeAnnotations(context.Model.FindEntityType(t));
                var sqlSchema = sql.Schema;
                if (string.IsNullOrEmpty(sqlSchema))
                    sqlSchema = DefaultSchema;

                result = string.Format(SchemaTableFormat, sqlSchema, sql.TableName);

                if (!string.IsNullOrWhiteSpace(result))
                    TableNames.TryAdd(t, result);
                else
                    throw new MassTransitException("Couldn't determine table and schema name (using model metadata).");
            }

            return result;
        }

        protected virtual IRelationalEntityTypeAnnotations GetRelationalEntityTypeAnnotations(IEntityType entityType)
        {
            return entityType.SqlServer();
        }
    }
}