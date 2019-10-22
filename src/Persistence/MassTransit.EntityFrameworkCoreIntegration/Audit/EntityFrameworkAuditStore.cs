// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.EntityFrameworkCoreIntegration.Audit
{
    using System.Threading.Tasks;

    using MassTransit.Audit;
    using MassTransit.Util;
    using Metadata;
    using Microsoft.EntityFrameworkCore;

    public class EntityFrameworkAuditStore : IMessageAuditStore
    {
        readonly DbContextOptions _contextOptions;

        readonly string _auditTableName;

        public EntityFrameworkAuditStore(DbContextOptions contextOptions, string auditTableName)
        {
            _contextOptions = contextOptions;
            _auditTableName = auditTableName;
        }

        public DbContext AuditContext =>
            new AuditDbContext(_contextOptions, _auditTableName);

        async Task IMessageAuditStore.StoreMessage<T>(T message, MessageAuditMetadata metadata)
        {
            using (var dbContext = AuditContext)
            {
                var auditRecord = AuditRecord.Create(message, TypeMetadataCache<T>.ShortName, metadata);

                dbContext.Set<AuditRecord>().Add(auditRecord);

                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}