using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BatchProcessing.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIdLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TRANSACTIONS_PROCESSED",
                table: "TRANSACTIONS_PROCESSED");

            migrationBuilder.AlterColumn<long>(
                name: "TransactionId",
                table: "TRANSACTIONS_PROCESSED",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "TRANSACTIONS_PROCESSED",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TRANSACTIONS_PROCESSED",
                table: "TRANSACTIONS_PROCESSED",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TRANSACTIONS_PROCESSED",
                table: "TRANSACTIONS_PROCESSED");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TRANSACTIONS_PROCESSED");

            migrationBuilder.AlterColumn<long>(
                name: "TransactionId",
                table: "TRANSACTIONS_PROCESSED",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TRANSACTIONS_PROCESSED",
                table: "TRANSACTIONS_PROCESSED",
                column: "TransactionId");
        }
    }
}
