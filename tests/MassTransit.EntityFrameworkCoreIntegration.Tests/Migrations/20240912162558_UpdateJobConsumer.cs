using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Migrations
{
    /// <inheritdoc />
    public partial class UpdateJobConsumer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobAttemptSaga",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentState = table.Column<int>(type: "int", nullable: false),
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RetryAttempt = table.Column<int>(type: "int", nullable: false),
                    ServiceAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstanceAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Started = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Faulted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusCheckTokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobAttemptSaga", x => x.CorrelationId);
                });

            migrationBuilder.CreateTable(
                name: "JobSaga",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentState = table.Column<int>(type: "int", nullable: false),
                    Submitted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ServiceAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobTimeout = table.Column<TimeSpan>(type: "time", nullable: true),
                    Job = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttemptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RetryAttempt = table.Column<int>(type: "int", nullable: false),
                    Started = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Completed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    Faulted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobSlotWaitToken = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    JobRetryDelayToken = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CronExpression = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeZoneId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    EndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NextStartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSaga", x => x.CorrelationId);
                });

            migrationBuilder.CreateTable(
                name: "JobTypeSaga",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentState = table.Column<int>(type: "int", nullable: false),
                    ActiveJobCount = table.Column<int>(type: "int", nullable: false),
                    ConcurrentJobLimit = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverrideJobLimit = table.Column<int>(type: "int", nullable: true),
                    OverrideLimitExpiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActiveJobs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Instances = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

        /// <inheritdoc />
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
