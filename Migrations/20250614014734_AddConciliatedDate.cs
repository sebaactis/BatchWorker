﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatchProcessing.Migrations
{
    /// <inheritdoc />
    public partial class AddConciliatedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ConciliatedDate",
                table: "TRANSACTIONS_PROCESSED",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConciliatedDate",
                table: "TRANSACTIONS_PROCESSED");
        }
    }
}
