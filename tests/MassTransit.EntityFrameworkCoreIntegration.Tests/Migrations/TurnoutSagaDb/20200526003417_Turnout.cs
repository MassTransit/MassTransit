using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Migrations.TurnoutSagaDb
{
    public partial class Turnout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TurnoutJobAttemptState",
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
                    table.PrimaryKey("PK_TurnoutJobAttemptState", x => x.CorrelationId);
                });

            migrationBuilder.CreateTable(
                name: "TurnoutJobState",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(nullable: false),
                    CurrentState = table.Column<int>(nullable: false),
                    Submitted = table.Column<DateTime>(nullable: true),
                    ServiceAddress = table.Column<string>(nullable: true),
                    JobTimeout = table.Column<TimeSpan>(nullable: true),
                    JobJson = table.Column<string>(nullable: true),
                    JobTypeId = table.Column<Guid>(nullable: false),
                    AttemptId = table.Column<Guid>(nullable: false),
                    Started = table.Column<DateTime>(nullable: true),
                    Completed = table.Column<DateTime>(nullable: true),
                    Duration = table.Column<TimeSpan>(nullable: true),
                    Faulted = table.Column<DateTime>(nullable: true),
                    Reason = table.Column<string>(nullable: true),
                    StartJobRequestId = table.Column<Guid>(nullable: true),
                    JobSlotRequestId = table.Column<Guid>(nullable: true),
                    JobSlotWaitToken = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurnoutJobState", x => x.CorrelationId);
                });

            migrationBuilder.CreateTable(
                name: "TurnoutJobTypeState",
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
                    table.PrimaryKey("PK_TurnoutJobTypeState", x => x.CorrelationId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TurnoutJobAttemptState_JobId_RetryAttempt",
                table: "TurnoutJobAttemptState",
                columns: new[] { "JobId", "RetryAttempt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TurnoutJobState_JobSlotRequestId",
                table: "TurnoutJobState",
                column: "JobSlotRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TurnoutJobState_StartJobRequestId",
                table: "TurnoutJobState",
                column: "StartJobRequestId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TurnoutJobAttemptState");

            migrationBuilder.DropTable(
                name: "TurnoutJobState");

            migrationBuilder.DropTable(
                name: "TurnoutJobTypeState");
        }
    }
}
