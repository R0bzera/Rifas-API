using Microsoft.EntityFrameworkCore;
using Rifa.Domain.Usuario;
using Rifa.Domain.Rifa;
using Rifa.Domain.Pedido;
using Rifa.Domain.Cota;
using Rifa.Domain.Pagamentos;

namespace Rifa.Infrastructure.Config
{
    public class RifaDbContext : DbContext
    {
        public RifaDbContext(DbContextOptions<RifaDbContext> options) : base(options)
        {
        }

        public DbSet<UsuarioEntity> usuarios_rifa { get; set; }
        public DbSet<RifaEntity> rifas { get; set; }
        public DbSet<PedidoEntity> pedidos_rifa { get; set; }
        public DbSet<CotaEntity> cotas_rifa { get; set; }
        public DbSet<PaymentOrder> payment_orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UsuarioEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Telefone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Senha).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Role).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.DataCriacao).IsRequired().HasColumnType("timestamp without time zone");
                entity.Property(e => e.DataAlteracao).IsRequired().HasColumnType("timestamp without time zone");

                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<RifaEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Descricao).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Preco).IsRequired().HasColumnType("decimal(10,2)");
                entity.Property(e => e.NumCotas).IsRequired();
                entity.Property(e => e.Finalizada).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.DataCriacao).IsRequired().HasColumnType("timestamp without time zone");
                entity.Property(e => e.DataAlteracao).IsRequired().HasColumnType("timestamp without time zone");

                entity.HasOne(e => e.Ganhador)
                      .WithMany()
                      .HasForeignKey(e => e.GanhadorId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<PedidoEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalCotas).IsRequired();
                entity.Property(e => e.PagamentoConfirmado).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.DataCriacao).IsRequired().HasColumnType("timestamp without time zone");
                entity.Property(e => e.DataAlteracao).IsRequired().HasColumnType("timestamp without time zone");

                entity.HasOne(e => e.Rifa)
                      .WithMany()
                      .HasForeignKey(e => e.RifaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CotaEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Numero).IsRequired();
                entity.Property(e => e.DataCriacao).IsRequired().HasColumnType("timestamp without time zone");
                entity.Property(e => e.DataAlteracao).IsRequired().HasColumnType("timestamp without time zone");

                entity.HasOne(e => e.Rifa)
                      .WithMany()
                      .HasForeignKey(e => e.RifaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Pedido)
                      .WithMany(e => e.Cotas)
                      .HasForeignKey(e => e.PedidoId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => new { e.RifaId, e.Numero }).IsUnique();
            });

            modelBuilder.Entity<PaymentOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PaymentId).IsRequired();
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DataCriacao).IsRequired().HasColumnType("timestamp without time zone");
                entity.Property(e => e.DataAlteracao).IsRequired().HasColumnType("timestamp without time zone");

                entity.HasOne(e => e.Pedido)
                      .WithMany()
                      .HasForeignKey(e => e.PedidoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.PaymentId).IsUnique();
            });
        }
    }
}
