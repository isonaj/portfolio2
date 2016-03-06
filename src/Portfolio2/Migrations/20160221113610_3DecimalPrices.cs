using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace Portfolio2.Migrations
{
    public partial class _3DecimalPrices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_PriceHistory_Stock_StockId", table: "PriceHistory");
            migrationBuilder.DropForeignKey(name: "FK_Txn_Stock_StockId", table: "Txn");
            migrationBuilder.AlterColumn<decimal>(
                name: "Open",
                table: "PriceHistory",
                type: "decimal(18,3)",
                nullable: false);
            migrationBuilder.AlterColumn<decimal>(
                name: "Low",
                table: "PriceHistory",
                type: "decimal(18,3)",
                nullable: false);
            migrationBuilder.AlterColumn<decimal>(
                name: "High",
                table: "PriceHistory",
                type: "decimal(18,3)",
                nullable: false);
            migrationBuilder.AlterColumn<decimal>(
                name: "Close",
                table: "PriceHistory",
                type: "decimal(18,3)",
                nullable: false);
            migrationBuilder.AddForeignKey(
                name: "FK_PriceHistory_Stock_StockId",
                table: "PriceHistory",
                column: "StockId",
                principalTable: "Stock",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_Txn_Stock_StockId",
                table: "Txn",
                column: "StockId",
                principalTable: "Stock",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_PriceHistory_Stock_StockId", table: "PriceHistory");
            migrationBuilder.DropForeignKey(name: "FK_Txn_Stock_StockId", table: "Txn");
            migrationBuilder.AlterColumn<decimal>(
                name: "Open",
                table: "PriceHistory",
                nullable: false);
            migrationBuilder.AlterColumn<decimal>(
                name: "Low",
                table: "PriceHistory",
                nullable: false);
            migrationBuilder.AlterColumn<decimal>(
                name: "High",
                table: "PriceHistory",
                nullable: false);
            migrationBuilder.AlterColumn<decimal>(
                name: "Close",
                table: "PriceHistory",
                nullable: false);
            migrationBuilder.AddForeignKey(
                name: "FK_PriceHistory_Stock_StockId",
                table: "PriceHistory",
                column: "StockId",
                principalTable: "Stock",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Txn_Stock_StockId",
                table: "Txn",
                column: "StockId",
                principalTable: "Stock",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
