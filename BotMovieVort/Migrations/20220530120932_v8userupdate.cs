using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotMovieVort.Migrations
{
    public partial class v8userupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserTelegramId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemSerialsUser",
                columns: table => new
                {
                    ItemSerialsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSerialsUser", x => new { x.ItemSerialsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ItemSerialsUser_ItemSerials_ItemSerialsId",
                        column: x => x.ItemSerialsId,
                        principalTable: "ItemSerials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemSerialsUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemSerialsUser_UsersId",
                table: "ItemSerialsUser",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemSerialsUser");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
