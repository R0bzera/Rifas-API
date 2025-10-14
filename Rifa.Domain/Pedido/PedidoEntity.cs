namespace Rifa.Domain.Pedido
{
    public class PedidoEntity
    {
        public Guid Id { get; set; }
        public Guid RifaId { get; set; }
        public int TotalCotas { get; set; }
        public bool PagamentoConfirmado { get; set; }
        public Guid UsuarioId { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }

        public Rifa.RifaEntity Rifa { get; set; } = null!;
        public Usuario.UsuarioEntity Usuario { get; set; } = null!;
        public List<Cota.CotaEntity> Cotas { get; set; } = new();
    }
}
