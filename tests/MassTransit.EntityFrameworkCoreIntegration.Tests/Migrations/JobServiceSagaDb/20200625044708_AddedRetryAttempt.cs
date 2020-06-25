using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Migrations.JobServiceSagaDb
{
    public partial class AddedRetryAttempt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "JobRetryDelayToken",
                table: "JobSaga",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RetryAttempt",
                table: "JobSaga",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobRetryDelayToken",
                table: "JobSaga");

            migrationBuilder.DropColumn(
                name: "RetryAttempt",
                table: "JobSaga");
        }
    }
}
