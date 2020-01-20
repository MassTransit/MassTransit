using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Migrations.SagaWithDependency
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SagaInnerDependency",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SagaInnerDependency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SagaDependency",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SagaInnerDependencyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SagaDependency", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SagaDependency_SagaInnerDependency_SagaInnerDependencyId",
                        column: x => x.SagaInnerDependencyId,
                        principalTable: "SagaInnerDependency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EfCoreSagasWithDepencies",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(nullable: false),
                    Completed = table.Column<bool>(nullable: false),
                    DependencyId = table.Column<Guid>(nullable: false),
                    Initiated = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EfCoreSagasWithDepencies", x => x.CorrelationId);
                    table.ForeignKey(
                        name: "FK_EfCoreSagasWithDepencies_SagaDependency_DependencyId",
                        column: x => x.DependencyId,
                        principalTable: "SagaDependency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EfCoreSagasWithDepencies_DependencyId",
                table: "EfCoreSagasWithDepencies",
                column: "DependencyId");

            migrationBuilder.CreateIndex(
                name: "IX_SagaDependency_SagaInnerDependencyId",
                table: "SagaDependency",
                column: "SagaInnerDependencyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EfCoreSagasWithDepencies");

            migrationBuilder.DropTable(
                name: "SagaDependency");

            migrationBuilder.DropTable(
                name: "SagaInnerDependency");
        }
    }
}
