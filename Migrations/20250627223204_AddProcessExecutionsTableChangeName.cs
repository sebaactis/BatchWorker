using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatchProcessing.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessExecutionsTableChangeName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcessExecutions",
                table: "ProcessExecutions");

            migrationBuilder.RenameTable(
                name: "ProcessExecutions",
                newName: "PROCESS_EXECUTIONS");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PROCESS_EXECUTIONS",
                table: "PROCESS_EXECUTIONS",
                column: "ProcessId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PROCESS_EXECUTIONS",
                table: "PROCESS_EXECUTIONS");

            migrationBuilder.RenameTable(
                name: "PROCESS_EXECUTIONS",
                newName: "ProcessExecutions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcessExecutions",
                table: "ProcessExecutions",
                column: "ProcessId");
        }
    }
}
