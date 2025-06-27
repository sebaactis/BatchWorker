using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BatchProcessing.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessExecutionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessExecutions",
                columns: table => new
                {
                    ProcessId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProcessName = table.Column<string>(type: "text", nullable: false),
                    ProcessStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ProcessEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ProcessDuration = table.Column<string>(type: "text", nullable: false),
                    ProcessState = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    SuccessItems = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    FailedItems = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessExecutions", x => x.ProcessId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessExecutions");
        }
    }
}
