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
namespace MassTransit.EntityFrameworkIntegration.Audit
{
    using System.Data.Entity;
    using System.Threading.Tasks;
    using MassTransit.Audit;
    using Metadata;
    using Util;


    public class EntityFrameworkAuditStore : IMessageAuditStore
    {
        readonly string _auditTableName;
        readonly string _nameOrConnectionString;

        public EntityFrameworkAuditStore(string nameOrConnectionString, string auditTableName)
        {
            _nameOrConnectionString = nameOrConnectionString;
            _auditTableName = auditTableName;
        }

        public DbContext AuditContext =>
            new AuditDbContext(_nameOrConnectionString, _auditTableName);

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