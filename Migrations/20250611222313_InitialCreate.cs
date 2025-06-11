using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BatchProcessing.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TRANSACTIONS_IN",
                columns: table => new
                {
                    TransactionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MerchantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MerchantName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MerchantCountry = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    MerchantCategoryCode = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    CardHolderName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CardNumberMasked = table.Column<string>(type: "character varying(19)", maxLength: 19, nullable: false),
                    CardType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CustomerId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AmountLocalCurrency = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    LocalCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PostingDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AuthorizationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TerminalId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    POSLocation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    POSCountryCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    EntryMode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AuthorizationCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TransactionType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsInternational = table.Column<bool>(type: "boolean", nullable: false),
                    IsFraudSuspected = table.Column<bool>(type: "boolean", nullable: false),
                    IsOfflineTransaction = table.Column<bool>(type: "boolean", nullable: false),
                    IsProcessed = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRANSACTIONS_IN", x => x.TransactionId);
                });

            migrationBuilder.CreateTable(
                name: "TRANSACTIONS_PROCESSED",
                columns: table => new
                {
                    TransactionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MerchantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MerchantName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MerchantCountry = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    MerchantCategoryCode = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    CardHolderName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CardNumberMasked = table.Column<string>(type: "character varying(19)", maxLength: 19, nullable: false),
                    CardType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CustomerId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AmountLocalCurrency = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    LocalCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PostingDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AuthorizationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TerminalId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    POSLocation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    POSCountryCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    EntryMode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AuthorizationCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TransactionType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsInternational = table.Column<bool>(type: "boolean", nullable: false),
                    IsFraudSuspected = table.Column<bool>(type: "boolean", nullable: false),
                    IsOfflineTransaction = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProcessedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRANSACTIONS_PROCESSED", x => x.TransactionId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TRANSACTIONS_IN");

            migrationBuilder.DropTable(
                name: "TRANSACTIONS_PROCESSED");
        }
    }
}
