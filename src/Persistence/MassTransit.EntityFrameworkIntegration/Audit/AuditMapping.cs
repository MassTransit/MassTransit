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
namespace MassTransit.EntityFrameworkIntegration
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using Audit;


    public class AuditMapping : EntityTypeConfiguration<AuditRecord>
    {
        public AuditMapping(string tableName)
        {
            ToTable(tableName);

            HasKey(x => x.AuditRecordId)
                .Property(x => x.AuditRecordId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.ContextType);
            Property(x => x.MessageId);
            Property(x => x.InitiatorId);
            Property(x => x.ConversationId);
            Property(x => x.CorrelationId);
            Property(x => x.RequestId);
            Property(x => x.SourceAddress);
            Property(x => x.DestinationAddress);
            Property(x => x.ResponseAddress);
            Property(x => x.FaultAddress);
            Property(x => x.InputAddress);

            Property(x => x.MessageType);
            Property(x => x._headers).HasColumnName("Headers");
            Property(x => x._custom).HasColumnName("Custom");
            Property(x => x._message).HasColumnName("Message");
        }
    }
}