using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatchProcessing.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreTRANSACTION_INFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountLocalCurrency",
                table: "TRANSACTIONS_IN",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "AuthorizationCode",
                table: "TRANSACTIONS_IN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "AuthorizationDate",
                table: "TRANSACTIONS_IN",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CardHolderName",
                table: "TRANSACTIONS_IN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CardNumberMasked",
                table: "TRANSACTIONS_IN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CardType",
                table: "TRANSACTIONS_IN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "TRANSACTIONS_IN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EntryMode",
                table: "TRANSACTIONS_IN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "TRANSACTIONS_IN",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsFraudSuspected",
                table: "TRANSACTIONS_IN",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInternational",
                table: "TRANSACTIONS_IN",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOfflineTransaction",
                table: "TRANSACTIONS_IN",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LocalCurrency",
                table: "TRANSACTIONS_IN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MerchantCategoryCode",
                table: "TRANSACTIONS_IN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MerchantCountry",
                table: "TRANSACTIONS_IN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MerchantName",
                table: "TRANSACTIONS_IN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "TRANSACTIONS_IN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "POSCountryCode",
                table: "TRANSACTIONS_IN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "POSLocation",
                table: "TRANSACTIONS_IN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PostingDate",
                table: "TRANSACTIONS_IN",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                table: "TRANSACTIONS_IN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TRANSACTIONS_IN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TerminalId",
                table: "TRANSACTIONS_IN",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TransactionType",
                table: "TRANSACTIONS_IN",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountLocalCurrency",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "AuthorizationCode",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "AuthorizationDate",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "CardHolderName",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "CardNumberMasked",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "CardType",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "EntryMode",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "IsFraudSuspected",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "IsInternational",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "IsOfflineTransaction",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "LocalCurrency",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "MerchantCategoryCode",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "MerchantCountry",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "MerchantName",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "POSCountryCode",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "POSLocation",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "PostingDate",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "TerminalId",
                table: "TRANSACTIONS_IN");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "TRANSACTIONS_IN");
        }
    }
}
