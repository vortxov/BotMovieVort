using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotMovieVort.Migrations
{
    public partial class v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Year",
                table: "ItemSerials",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "RatingIMDB",
                table: "ItemSerials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VotesIMDB",
                table: "ItemSerials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VotesKP",
                table: "ItemSerials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VotesIMDB",
                table: "ItemFilms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VotesKP",
                table: "ItemFilms",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RatingIMDB",
                table: "ItemSerials");

            migrationBuilder.DropColumn(
                name: "VotesIMDB",
                table: "ItemSerials");

            migrationBuilder.DropColumn(
                name: "VotesKP",
                table: "ItemSerials");

            migrationBuilder.DropColumn(
                name: "VotesIMDB",
                table: "ItemFilms");

            migrationBuilder.DropColumn(
                name: "VotesKP",
                table: "ItemFilms");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "ItemSerials",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
