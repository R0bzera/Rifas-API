namespace Rifa.Application.Interfaces
{
    public interface ISorteioAutomaticoService
    {
        Task<bool> VerificarEExecutarSorteioAutomaticoAsync(Guid rifaId);
        Task<IEnumerable<Guid>> VerificarRifasProntasParaSorteioAsync();
    }
}
