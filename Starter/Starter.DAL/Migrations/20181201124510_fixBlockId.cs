using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Starter.DAL.Migrations
{
    public partial class fixBlockId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlockVerifications_Blocks_BlockHash",
                table: "BlockVerifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Blocks_BlockHash",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_BlockHash",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_BlockVerifications_BlockHash",
                table: "BlockVerifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Blocks",
                table: "Blocks");

            migrationBuilder.DropColumn(
                name: "BlockHash",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "BlockHash",
                table: "BlockVerifications");

            migrationBuilder.AddColumn<int>(
                name: "BlockId",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BlockId",
                table: "BlockVerifications",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "Blocks",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Blocks",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Blocks",
                table: "Blocks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BlockId",
                table: "Transactions",
                column: "BlockId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockVerifications_BlockId",
                table: "BlockVerifications",
                column: "BlockId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlockVerifications_Blocks_BlockId",
                table: "BlockVerifications",
                column: "BlockId",
                principalTable: "Blocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Blocks_BlockId",
                table: "Transactions",
                column: "BlockId",
                principalTable: "Blocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlockVerifications_Blocks_BlockId",
                table: "BlockVerifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Blocks_BlockId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_BlockId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_BlockVerifications_BlockId",
                table: "BlockVerifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Blocks",
                table: "Blocks");

            migrationBuilder.DropColumn(
                name: "BlockId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "BlockId",
                table: "BlockVerifications");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Blocks");

            migrationBuilder.AddColumn<string>(
                name: "BlockHash",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BlockHash",
                table: "BlockVerifications",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "Blocks",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Blocks",
                table: "Blocks",
                column: "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BlockHash",
                table: "Transactions",
                column: "BlockHash");

            migrationBuilder.CreateIndex(
                name: "IX_BlockVerifications_BlockHash",
                table: "BlockVerifications",
                column: "BlockHash");

            migrationBuilder.AddForeignKey(
                name: "FK_BlockVerifications_Blocks_BlockHash",
                table: "BlockVerifications",
                column: "BlockHash",
                principalTable: "Blocks",
                principalColumn: "Hash",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Blocks_BlockHash",
                table: "Transactions",
                column: "BlockHash",
                principalTable: "Blocks",
                principalColumn: "Hash",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
