namespace Rifa.Domain.Rifa
{
    public class RifaEntity
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string? Imagem { get; set; }
        public decimal Preco { get; set; }
        public int NumCotas { get; set; }
        public Guid? GanhadorId { get; set; }
        public bool Finalizada { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }

        public Usuario.UsuarioEntity? Ganhador { get; set; }
    }
}
