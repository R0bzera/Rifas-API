using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rifa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentOrdersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "payment_orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentId = table.Column<long>(type: "bigint", nullable: false),
                    PedidoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_payment_orders_pedidos_rifa_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "pedidos_rifa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_payment_orders_PaymentId",
                table: "payment_orders",
                column: "PaymentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_orders_PedidoId",
                table: "payment_orders",
                column: "PedidoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payment_orders");
        }
    }
}
