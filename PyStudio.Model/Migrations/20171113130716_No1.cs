using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PyStudio.Model.Migrations
{
    public partial class No1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InfoArea",
                columns: table => new
                {
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    AreaCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    AreaCoord = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    AreaLevel = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((0))"),
                    AreaName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AreaNote = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    AreaPathId = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    AreaPid = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    AreaZipCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfoArea", x => x.AreaId);
                });

            migrationBuilder.CreateTable(
                name: "InfoEi",
                columns: table => new
                {
                    Eicol1 = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Eicol2 = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Eicol3 = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Eicol4 = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfoEi", x => x.Eicol1);
                });

            migrationBuilder.CreateTable(
                name: "InfoUser",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserBirthday = table.Column<DateTime>(type: "date", nullable: true),
                    UserBlog = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserCreateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    UserEmail = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    UserHeadPhoto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserIntroduce = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserIps = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserLoginTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserNickName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserPwd = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    UserSex = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((2))"),
                    UserStatus = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((0))"),
                    UserTel = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfoUser", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "SysLogger",
                columns: table => new
                {
                    LoggerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LoggerCreateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    LoggerDescription = table.Column<string>(type: "ntext", nullable: true),
                    LoggerIps = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    LoggerOperation = table.Column<int>(type: "int", nullable: true),
                    LoggerUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysLogger", x => x.LoggerId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InfoArea");

            migrationBuilder.DropTable(
                name: "InfoEi");

            migrationBuilder.DropTable(
                name: "InfoUser");

            migrationBuilder.DropTable(
                name: "SysLogger");
        }
    }
}
