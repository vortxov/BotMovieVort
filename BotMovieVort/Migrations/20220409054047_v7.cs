using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotMovieVort.Migrations
{
    public partial class v7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seasons_ItemSerials_ItemSerialsId",
                table: "Seasons");

            migrationBuilder.DropForeignKey(
                name: "FK_Series_Seasons_SeasonId",
                table: "Series");

            migrationBuilder.DropIndex(
                name: "IX_Seasons_ItemSerialsId",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "ItemSerialsId",
                table: "Seasons");

            migrationBuilder.AlterColumn<Guid>(
                name: "SeasonId",
                table: "Series",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SerialsId",
                table: "Seasons",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_SerialsId",
                table: "Seasons",
                column: "SerialsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Seasons_ItemSerials_SerialsId",
                table: "Seasons",
                column: "SerialsId",
                principalTable: "ItemSerials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Series_Seasons_SeasonId",
                table: "Series",
                column: "SeasonId",
                principalTable: "Seasons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seasons_ItemSerials_SerialsId",
                table: "Seasons");

            migrationBuilder.DropForeignKey(
                name: "FK_Series_Seasons_SeasonId",
                table: "Series");

            migrationBuilder.DropIndex(
                name: "IX_Seasons_SerialsId",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "SerialsId",
                table: "Seasons");

            migrationBuilder.AlterColumn<Guid>(
                name: "SeasonId",
                table: "Series",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ItemSerialsId",
                table: "Seasons",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_ItemSerialsId",
                table: "Seasons",
                column: "ItemSerialsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Seasons_ItemSerials_ItemSerialsId",
                table: "Seasons",
                column: "ItemSerialsId",
                principalTable: "ItemSerials",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Series_Seasons_SeasonId",
                table: "Series",
                column: "SeasonId",
                principalTable: "Seasons",
                principalColumn: "Id");
        }
    }
}
