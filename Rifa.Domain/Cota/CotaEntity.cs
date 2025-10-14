namespace Rifa.Domain.Cota
{
    public class CotaEntity
    {
        public Guid Id { get; set; }
        public int Numero { get; set; }
        public Guid RifaId { get; set; }
        public Guid? UsuarioId { get; set; }
        public Guid? PedidoId { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }

        public Rifa.RifaEntity Rifa { get; set; } = null!;
        public Usuario.UsuarioEntity? Usuario { get; set; }
        public Pedido.PedidoEntity? Pedido { get; set; }
    }
}
