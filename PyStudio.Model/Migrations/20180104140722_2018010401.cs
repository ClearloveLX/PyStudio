using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PyStudio.Model.Migrations
{
    public partial class _2018010401 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InfoArea",
                columns: table => new
                {
                    AreaId = table.Column<int>(nullable: false),
                    AreaCode = table.Column<string>(maxLength: 50, nullable: true),
                    AreaCoord = table.Column<string>(maxLength: 50, nullable: true),
                    AreaLevel = table.Column<int>(nullable: false, defaultValueSql: "'0'"),
                    AreaName = table.Column<string>(maxLength: 50, nullable: true),
                    AreaNote = table.Column<string>(maxLength: 150, nullable: true),
                    AreaPathId = table.Column<string>(maxLength: 100, nullable: true),
                    AreaPid = table.Column<string>(maxLength: 50, nullable: true),
                    AreaZipCode = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfoArea", x => x.AreaId);
                });

            migrationBuilder.CreateTable(
                name: "InfoEi",
                columns: table => new
                {
                    Eicol1 = table.Column<string>(maxLength: 50, nullable: false),
                    Eicol2 = table.Column<string>(maxLength: 50, nullable: true),
                    Eicol3 = table.Column<string>(maxLength: 50, nullable: true),
                    Eicol4 = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfoEi", x => x.Eicol1);
                });

            migrationBuilder.CreateTable(
                name: "InfoMessageBoard",
                columns: table => new
                {
                    MessageBoardId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MessageBoardContent = table.Column<string>(type: "text", nullable: true),
                    MessageBoardCreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    MessageBoardIp = table.Column<string>(maxLength: 50, nullable: true),
                    MessageBoardUser = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfoMessageBoard", x => x.MessageBoardId);
                });

            migrationBuilder.CreateTable(
                name: "InfoUser",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserAddress = table.Column<string>(maxLength: 200, nullable: true),
                    UserBirthday = table.Column<DateTime>(type: "date", nullable: true),
                    UserBlog = table.Column<string>(maxLength: 200, nullable: true),
                    UserCreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    UserEmail = table.Column<string>(maxLength: 50, nullable: true),
                    UserHeadPhoto = table.Column<string>(maxLength: 50, nullable: true),
                    UserIntroduce = table.Column<string>(maxLength: 200, nullable: true),
                    UserIps = table.Column<string>(maxLength: 50, nullable: true),
                    UserLoginTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: false),
                    UserNickName = table.Column<string>(maxLength: 50, nullable: true),
                    UserPwd = table.Column<string>(maxLength: 50, nullable: false),
                    UserSex = table.Column<int>(nullable: false, defaultValueSql: "'2'"),
                    UserStatus = table.Column<int>(nullable: false, defaultValueSql: "'0'"),
                    UserTel = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfoUser", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "SysLogger",
                columns: table => new
                {
                    LoggerId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LoggerCreateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    LoggerDescription = table.Column<string>(type: "text", nullable: true),
                    LoggerIps = table.Column<string>(maxLength: 50, nullable: true),
                    LoggerOperation = table.Column<int>(nullable: true),
                    LoggerUser = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysLogger", x => x.LoggerId);
                    table.ForeignKey(
                        name: "FK_SysLogger_InfoUser",
                        column: x => x.LoggerUser,
                        principalTable: "InfoUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SysLogger_LoggerUser",
                table: "SysLogger",
                column: "LoggerUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InfoArea");

            migrationBuilder.DropTable(
                name: "InfoEi");

            migrationBuilder.DropTable(
                name: "InfoMessageBoard");

            migrationBuilder.DropTable(
                name: "SysLogger");

            migrationBuilder.DropTable(
                name: "InfoUser");
        }
    }
}
