using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Starter.DAL.Migrations
{
    public partial class userRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                nullable: false,
                defaultValue: "User");

            migrationBuilder.CreateTable(
                name: "TrustfullServers",
                columns: table => new
                {
                    Hash = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    PublicKey = table.Column<string>(nullable: false),
                    Salt = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrustfullServers", x => x.Hash);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrustfullServers");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");
        }
    }
}