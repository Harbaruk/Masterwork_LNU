using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Starter.DAL.Migrations
{
    public partial class ConnectServerAndBlocksWithFR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blocks_TrustfullServers_Hash",
                table: "Blocks");

            migrationBuilder.DropIndex(
                name: "IX_Blocks_Hash",
                table: "Blocks");

            migrationBuilder.RenameColumn(
                name: "Hash",
                table: "Blocks",
                newName: "BlockHash");

            migrationBuilder.AlterColumn<string>(
                name: "BlockHash",
                table: "Blocks",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "MinerHash",
                table: "Blocks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_MinerHash",
                table: "Blocks",
                column: "MinerHash");

            migrationBuilder.AddForeignKey(
                name: "FK_Blocks_TrustfullServers_MinerHash",
                table: "Blocks",
                column: "MinerHash",
                principalTable: "TrustfullServers",
                principalColumn: "Hash",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blocks_TrustfullServers_MinerHash",
                table: "Blocks");

            migrationBuilder.DropIndex(
                name: "IX_Blocks_MinerHash",
                table: "Blocks");

            migrationBuilder.DropColumn(
                name: "MinerHash",
                table: "Blocks");

            migrationBuilder.RenameColumn(
                name: "BlockHash",
                table: "Blocks",
                newName: "Hash");

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
    }
}
