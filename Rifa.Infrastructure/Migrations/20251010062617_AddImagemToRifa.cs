using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rifa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImagemToRifa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Imagem",
                table: "rifas",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "rifas");
        }
    }
}
