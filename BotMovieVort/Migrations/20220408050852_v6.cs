using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotMovieVort.Migrations
{
    public partial class v6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Genres_ItemFilms_ItemFilmId",
                table: "Genres");

            migrationBuilder.DropForeignKey(
                name: "FK_Genres_ItemSerials_ItemSerialsId",
                table: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_Genres_ItemFilmId",
                table: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_Genres_ItemSerialsId",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "ItemFilmId",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "ItemSerialsId",
                table: "Genres");

            migrationBuilder.CreateTable(
                name: "GenreItemFilm",
                columns: table => new
                {
                    GenresId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemFilmsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreItemFilm", x => new { x.GenresId, x.ItemFilmsId });
                    table.ForeignKey(
                        name: "FK_GenreItemFilm_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreItemFilm_ItemFilms_ItemFilmsId",
                        column: x => x.ItemFilmsId,
                        principalTable: "ItemFilms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenreItemSerials",
                columns: table => new
                {
                    GenresId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemSerialsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreItemSerials", x => new { x.GenresId, x.ItemSerialsId });
                    table.ForeignKey(
                        name: "FK_GenreItemSerials_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreItemSerials_ItemSerials_ItemSerialsId",
                        column: x => x.ItemSerialsId,
                        principalTable: "ItemSerials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GenreItemFilm_ItemFilmsId",
                table: "GenreItemFilm",
                column: "ItemFilmsId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreItemSerials_ItemSerialsId",
                table: "GenreItemSerials",
                column: "ItemSerialsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GenreItemFilm");

            migrationBuilder.DropTable(
                name: "GenreItemSerials");

            migrationBuilder.AddColumn<Guid>(
                name: "ItemFilmId",
                table: "Genres",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemSerialsId",
                table: "Genres",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genres_ItemFilmId",
                table: "Genres",
                column: "ItemFilmId");

            migrationBuilder.CreateIndex(
                name: "IX_Genres_ItemSerialsId",
                table: "Genres",
                column: "ItemSerialsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Genres_ItemFilms_ItemFilmId",
                table: "Genres",
                column: "ItemFilmId",
                principalTable: "ItemFilms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Genres_ItemSerials_ItemSerialsId",
                table: "Genres",
                column: "ItemSerialsId",
                principalTable: "ItemSerials",
                principalColumn: "Id");
        }
    }
}
