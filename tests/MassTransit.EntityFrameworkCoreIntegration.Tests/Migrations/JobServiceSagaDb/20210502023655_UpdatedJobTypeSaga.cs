using Microsoft.EntityFrameworkCore.Migrations;

namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Migrations.JobServiceSagaDb
{
    public partial class UpdatedJobTypeSaga : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Instances",
                table: "JobTypeSaga",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Instances",
                table: "JobTypeSaga");
        }
    }
}
