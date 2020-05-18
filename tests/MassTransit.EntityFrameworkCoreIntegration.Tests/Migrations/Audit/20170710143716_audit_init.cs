using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Migrations.Audit
{
    using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;


    public partial class audit_init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EfCoreAudit",
                columns: table => new
                {
                    AuditRecordId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContextType = table.Column<string>(nullable: true),
                    ConversationId = table.Column<Guid>(nullable: true),
                    CorrelationId = table.Column<Guid>(nullable: true),
                    DestinationAddress = table.Column<string>(nullable: true),
                    FaultAddress = table.Column<string>(nullable: true),
                    InputAddress = table.Column<string>(nullable: true),
                    InitiatorId = table.Column<Guid>(nullable: true),
                    MessageId = table.Column<Guid>(nullable: true),
                    MessageType = table.Column<string>(nullable: true),
                    RequestId = table.Column<Guid>(nullable: true),
                    ResponseAddress = table.Column<string>(nullable: true),
                    SourceAddress = table.Column<string>(nullable: true),
                    Custom = table.Column<string>(nullable: true),
                    Headers = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EfCoreAudit", x => x.AuditRecordId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EfCoreAudit");
        }
    }
}
