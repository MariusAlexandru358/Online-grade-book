using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogOnline.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMaterie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "An",
                table: "Materie");

            migrationBuilder.DropColumn(
                name: "Semestru",
                table: "Materie");

            migrationBuilder.AddColumn<int>(
                name: "An",
                table: "ProfesorMaterieStudent",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Semestru",
                table: "ProfesorMaterieStudent",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "An",
                table: "ProfesorMaterieStudent");

            migrationBuilder.DropColumn(
                name: "Semestru",
                table: "ProfesorMaterieStudent");

            migrationBuilder.AddColumn<int>(
                name: "An",
                table: "Materie",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Semestru",
                table: "Materie",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
