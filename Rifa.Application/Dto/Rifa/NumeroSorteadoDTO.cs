namespace Rifa.Application.Dto.Rifa
{
    public class NumeroSorteadoDTO
    {
        public Guid RifaId { get; set; }
        public int NumeroSorteado { get; set; }
        public int TotalCotas { get; set; }
        public DateTime DataGeracao { get; set; }
        public string Mensagem { get; set; } = string.Empty;
    }
}
