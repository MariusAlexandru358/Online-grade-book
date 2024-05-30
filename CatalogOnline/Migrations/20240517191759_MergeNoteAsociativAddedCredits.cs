using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogOnline.Migrations
{
    /// <inheritdoc />
    public partial class MergeNoteAsociativAddedCredits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Nota",
                table: "ProfesorMaterieStudent",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Credite",
                table: "Materie",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nota",
                table: "ProfesorMaterieStudent");

            migrationBuilder.DropColumn(
                name: "Credite",
                table: "Materie");
        }
    }
}
