using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatchProcessing.Migrations
{
    /// <inheritdoc />
    public partial class AddIsConciliatedOnTrxProcessed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConciliated",
                table: "TRANSACTIONS_PROCESSED",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConciliated",
                table: "TRANSACTIONS_PROCESSED");
        }
    }
}
