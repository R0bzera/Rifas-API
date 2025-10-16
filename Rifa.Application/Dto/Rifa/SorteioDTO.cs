namespace Rifa.Application.Dto.Rifa
{
    public class SorteioResultadoDTO
    {
        public Guid RifaId { get; set; }
        public string RifaTitulo { get; set; } = string.Empty;
        public int NumeroSorteado { get; set; }
        public Guid GanhadorId { get; set; }
        public string GanhadorNome { get; set; } = string.Empty;
        public string GanhadorEmail { get; set; } = string.Empty;
        public DateTime DataSorteio { get; set; }
        public bool SorteioRealizado { get; set; }
        public string Mensagem { get; set; } = string.Empty;
    }

    public class StatusSorteioDTO
    {
        public Guid RifaId { get; set; }
        public string RifaTitulo { get; set; } = string.Empty;
        public int NumCotas { get; set; }
        public int CotasVendidas { get; set; }
        public int CotasDisponiveis { get; set; }
        public bool RifaCompleta { get; set; }
        public bool SorteioIniciado { get; set; }
        public bool SorteioFinalizado { get; set; }
        public DateTime? DataInicioSorteio { get; set; }
        public DateTime? DataSorteio { get; set; }
        public int? NumeroSorteado { get; set; }
        public Guid? GanhadorId { get; set; }
        public string? GanhadorNome { get; set; }
        public int TempoRestanteSegundos { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
