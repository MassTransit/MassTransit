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
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public static class AuditMappingExtensions
    {
        public static void ConfigureAuditMapping(this ModelBuilder modelBuilder, string tableName)
        {
            EntityTypeBuilder<AuditRecord> typeBuilder = modelBuilder.Entity<AuditRecord>();
            typeBuilder.ToTable(tableName);

            typeBuilder.HasKey(x => x.AuditRecordId);
                typeBuilder.Property(x => x.AuditRecordId)
                .ValueGeneratedOnAdd();

            typeBuilder.Property(x => x.ContextType);
            typeBuilder.Property(x => x.MessageId);
            typeBuilder.Property(x => x.InitiatorId);
            typeBuilder.Property(x => x.ConversationId);
            typeBuilder.Property(x => x.CorrelationId);
            typeBuilder.Property(x => x.RequestId);
            typeBuilder.Property(x => x.SourceAddress);
            typeBuilder.Property(x => x.DestinationAddress);
            typeBuilder.Property(x => x.ResponseAddress);
            typeBuilder.Property(x => x.FaultAddress);
            typeBuilder.Property(x => x.InputAddress);
            typeBuilder.Property(x => x.MessageType);
            typeBuilder.Property(x => x._headers).HasColumnName("Headers");
            typeBuilder.Property(x => x._custom).HasColumnName("Custom");
            typeBuilder.Property(x => x._message).HasColumnName("Message");
        }
    }
}