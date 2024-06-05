using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogOnline.Migrations
{
    /// <inheritdoc />
    public partial class MesajProfesorCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MesajProfesor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfesorId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MesajProfesor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MesajProfesor_Profesor_ProfesorId",
                        column: x => x.ProfesorId,
                        principalTable: "Profesor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MesajProfesor_ProfesorId",
                table: "MesajProfesor",
                column: "ProfesorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MesajProfesor");
        }
    }
}
