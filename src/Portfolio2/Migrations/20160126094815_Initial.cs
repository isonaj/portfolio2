using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace Portfolio2.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_PriceHistory_Stock_StockId", table: "PriceHistory");
            migrationBuilder.DropForeignKey(name: "FK_Txn_Stock_StockId", table: "Txn");
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
