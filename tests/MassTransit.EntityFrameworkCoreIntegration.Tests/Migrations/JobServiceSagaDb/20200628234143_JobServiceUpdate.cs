using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Migrations.JobServiceSagaDb
{
    public partial class JobServiceUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobAttemptSaga",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(nullable: false),
                    CurrentState = table.Column<int>(nullable: false),
                    JobId = table.Column<Guid>(nullable: false),
                    RetryAttempt = table.Column<int>(nullable: false),
                    ServiceAddress = table.Column<string>(nullable: true),
                    InstanceAddress = table.Column<string>(nullable: true),
                    Started = table.Column<DateTime>(nullable: true),
                    Faulted = table.Column<DateTime>(nullable: true),
                    StatusCheckTokenId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobAttemptSaga", x => x.CorrelationId);
                });

            migrationBuilder.CreateTable(
                name: "JobSaga",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(nullable: false),
                    CurrentState = table.Column<int>(nullable: false),
                    Submitted = table.Column<DateTime>(nullable: true),
                    ServiceAddress = table.Column<string>(nullable: true),
                    JobTimeout = table.Column<TimeSpan>(nullable: true),
                    Job = table.Column<string>(nullable: true),
                    JobTypeId = table.Column<Guid>(nullable: false),
                    AttemptId = table.Column<Guid>(nullable: false),
                    RetryAttempt = table.Column<int>(nullable: false),
                    Started = table.Column<DateTime>(nullable: true),
                    Completed = table.Column<DateTime>(nullable: true),
                    Duration = table.Column<TimeSpan>(nullable: true),
                    Faulted = table.Column<DateTime>(nullable: true),
                    Reason = table.Column<string>(nullable: true),
                    JobSlotWaitToken = table.Column<Guid>(nullable: true),
                    JobRetryDelayToken = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSaga", x => x.CorrelationId);
                });

            migrationBuilder.CreateTable(
                name: "JobTypeSaga",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(nullable: false),
                    CurrentState = table.Column<int>(nullable: false),
                    ActiveJobCount = table.Column<int>(nullable: false),
                    ConcurrentJobLimit = table.Column<int>(nullable: false),
                    OverrideJobLimit = table.Column<int>(nullable: true),
                    OverrideLimitExpiration = table.Column<DateTime>(nullable: true),
                    ActiveJobs = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTypeSaga", x => x.CorrelationId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobAttemptSaga_JobId_RetryAttempt",
                table: "JobAttemptSaga",
                columns: new[] { "JobId", "RetryAttempt" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobAttemptSaga");

            migrationBuilder.DropTable(
                name: "JobSaga");

            migrationBuilder.DropTable(
                name: "JobTypeSaga");
        }
    }
}
