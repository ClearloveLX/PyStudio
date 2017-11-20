using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PyStudio.Model.Migrations
{
    public partial class No2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoggerUserId",
                table: "SysLogger");

            migrationBuilder.AddColumn<int>(
                name: "LoggerUser",
                table: "SysLogger",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InfoMessageBoard",
                columns: table => new
                {
                    MessageBoardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MessageBoardContent = table.Column<string>(type: "ntext", nullable: true),
                    MessageBoardCreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    MessageBoardIp = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MessageBoardUser = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfoMessageBoard", x => x.MessageBoardId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SysLogger_LoggerUser",
                table: "SysLogger",
                column: "LoggerUser");

            migrationBuilder.AddForeignKey(
                name: "FK_SysLogger_InfoUser",
                table: "SysLogger",
                column: "LoggerUser",
                principalTable: "InfoUser",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SysLogger_InfoUser",
                table: "SysLogger");

            migrationBuilder.DropTable(
                name: "InfoMessageBoard");

            migrationBuilder.DropIndex(
                name: "IX_SysLogger_LoggerUser",
                table: "SysLogger");

            migrationBuilder.DropColumn(
                name: "LoggerUser",
                table: "SysLogger");

            migrationBuilder.AddColumn<int>(
                name: "LoggerUserId",
                table: "SysLogger",
                nullable: true);
        }
    }
}
