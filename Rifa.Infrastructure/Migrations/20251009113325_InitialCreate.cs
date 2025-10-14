using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rifa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "usuarios_rifa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Senha = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios_rifa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "rifas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Preco = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    NumCotas = table.Column<int>(type: "integer", nullable: false),
                    GanhadorId = table.Column<Guid>(type: "uuid", nullable: true),
                    Finalizada = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rifas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rifas_usuarios_rifa_GanhadorId",
                        column: x => x.GanhadorId,
                        principalTable: "usuarios_rifa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "pedidos_rifa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RifaId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalCotas = table.Column<int>(type: "integer", nullable: false),
                    PagamentoConfirmado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedidos_rifa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pedidos_rifa_rifas_RifaId",
                        column: x => x.RifaId,
                        principalTable: "rifas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pedidos_rifa_usuarios_rifa_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios_rifa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cotas_rifa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Numero = table.Column<int>(type: "integer", nullable: false),
                    RifaId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    PedidoId = table.Column<Guid>(type: "uuid", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cotas_rifa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cotas_rifa_pedidos_rifa_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "pedidos_rifa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_cotas_rifa_rifas_RifaId",
                        column: x => x.RifaId,
                        principalTable: "rifas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cotas_rifa_usuarios_rifa_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios_rifa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cotas_rifa_PedidoId",
                table: "cotas_rifa",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_cotas_rifa_RifaId_Numero",
                table: "cotas_rifa",
                columns: new[] { "RifaId", "Numero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cotas_rifa_UsuarioId",
                table: "cotas_rifa",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_rifa_RifaId",
                table: "pedidos_rifa",
                column: "RifaId");

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_rifa_UsuarioId",
                table: "pedidos_rifa",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_rifas_GanhadorId",
                table: "rifas",
                column: "GanhadorId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_rifa_Email",
                table: "usuarios_rifa",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cotas_rifa");

            migrationBuilder.DropTable(
                name: "pedidos_rifa");

            migrationBuilder.DropTable(
                name: "rifas");

            migrationBuilder.DropTable(
                name: "usuarios_rifa");
        }
    }
}
