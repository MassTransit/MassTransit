using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MassTransit.EntityFrameworkCoreIntegration.Audit;

namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Migrations.Audit
{
    [DbContext(typeof(AuditDbContext))]
    [Migration("20170710143716_audit_init")]
    partial class audit_init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.Audit.AuditRecord", b =>
                {
                    b.Property<int>("AuditRecordId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContextType");

                    b.Property<Guid?>("ConversationId");

                    b.Property<Guid?>("CorrelationId");

                    b.Property<string>("DestinationAddress");

                    b.Property<string>("FaultAddress");

                    b.Property<string>("InputAddress");

                    b.Property<Guid?>("InitiatorId");

                    b.Property<Guid?>("MessageId");

                    b.Property<string>("MessageType");

                    b.Property<Guid?>("RequestId");

                    b.Property<string>("ResponseAddress");

                    b.Property<string>("SourceAddress");

                    b.Property<string>("_custom")
                        .HasColumnName("Custom");

                    b.Property<string>("_headers")
                        .HasColumnName("Headers");

                    b.Property<string>("_message")
                        .HasColumnName("Message");

                    b.HasKey("AuditRecordId");

                    b.ToTable("EfCoreAudit");
                });
        }
    }
}
