using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Starter.DAL.Migrations
{
    public partial class BlockMinerBecomeServer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blocks_Users_MinerId",
                table: "Blocks");

            migrationBuilder.DropIndex(
                name: "IX_Blocks_MinerId",
                table: "Blocks");

            migrationBuilder.DropColumn(
                name: "MinerId",
                table: "Blocks");

            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "Blocks",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_Hash",
                table: "Blocks",
                column: "Hash");

            migrationBuilder.AddForeignKey(
                name: "FK_Blocks_TrustfullServers_Hash",
                table: "Blocks",
                column: "Hash",
                principalTable: "TrustfullServers",
                principalColumn: "Hash",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blocks_TrustfullServers_Hash",
                table: "Blocks");

            migrationBuilder.DropIndex(
                name: "IX_Blocks_Hash",
                table: "Blocks");

            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "Blocks",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<Guid>(
                name: "MinerId",
                table: "Blocks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_MinerId",
                table: "Blocks",
                column: "MinerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blocks_Users_MinerId",
                table: "Blocks",
                column: "MinerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
