using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionOrdinateurs.API.Migrations
{
    /// <inheritdoc />
    public partial class AjoutHeritageOrdinateur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AutonomieBatterie",
                table: "Ordinateurs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Boitier",
                table: "Ordinateurs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TailleEcran",
                table: "Ordinateurs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeOrdinateur",
                table: "Ordinateurs",
                type: "TEXT",
                maxLength: 21,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutonomieBatterie",
                table: "Ordinateurs");

            migrationBuilder.DropColumn(
                name: "Boitier",
                table: "Ordinateurs");

            migrationBuilder.DropColumn(
                name: "TailleEcran",
                table: "Ordinateurs");

            migrationBuilder.DropColumn(
                name: "TypeOrdinateur",
                table: "Ordinateurs");
        }
    }
}
