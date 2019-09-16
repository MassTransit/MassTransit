// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.DapperIntegration
{
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Context;
    using Dapper;
    using Metadata;
    using Saga;
    using Util;


    public class DapperSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextProxyScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly SqlConnection _sqlConnection;
        readonly string _tableName;
        readonly bool _existing;

        public DapperSagaConsumeContext(SqlConnection sqlConnection, ConsumeContext<TMessage> context, TSaga instance, string tableName,
            bool existing = true)
            : base(context)
        {
            _sqlConnection = sqlConnection;
            _tableName = tableName;
            _existing = existing;
            Saga = instance;
        }

        public async Task SetCompleted()
        {
            IsCompleted = true;
            if (_existing)
            {
                var correlationId = Saga.CorrelationId;
                await _sqlConnection
                    .QueryAsync($"DELETE FROM {_tableName} WHERE CorrelationId = @correlationId", new {correlationId})
                    .ConfigureAwait(false);

                LogContext.Debug?.Log("SAGA:{SagaType}:{CorrelationId} Removed {MessageType}", TypeMetadataCache<TSaga>.ShortName, Saga.CorrelationId,
                    TypeMetadataCache<TMessage>.ShortName);
            }
        }

        public TSaga Saga { get; }
        public bool IsCompleted { get; private set; }
    }
}
