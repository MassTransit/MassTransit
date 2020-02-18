using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Migrations.Saga
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EfCoreSimpleSagas",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(nullable: false),
                    Completed = table.Column<bool>(nullable: false),
                    Initiated = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 40, nullable: true),
                    Observed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EfCoreSimpleSagas", x => x.CorrelationId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EfCoreSimpleSagas");
        }
    }
}
