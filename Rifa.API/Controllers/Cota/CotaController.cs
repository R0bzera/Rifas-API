using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rifa.Application.Dto.Cota;
using Rifa.Application.Interfaces;

namespace Rifa.API.Controllers.Cota
{
    [ApiController]
    [Route("api/[controller]")]
    public class CotaController : ControllerBase
    {
        private readonly ICotaService _cotaService;

        public CotaController(ICotaService cotaService)
        {
            _cotaService = cotaService;
        }

        [HttpGet]
        [Route("getcotas")]
        public async Task<IActionResult> ObterCotas()
        {
            try
            {
                var cotas = await _cotaService.ObterTodasAsync();
                return Ok(cotas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("getcota/{id}")]
        public async Task<IActionResult> ObterCota(Guid id)
        {
            try
            {
                var cota = await _cotaService.ObterPorIdAsync(id);
                return Ok(cota);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("getcotasbyrifas/{rifaId}")]
        public async Task<IActionResult> ObterCotasPorRifa(Guid rifaId)
        {
            try
            {
                var cotas = await _cotaService.ObterPorRifaAsync(rifaId);
                return Ok(cotas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("getcotasbyuser/{usuarioId}")]
        public async Task<IActionResult> ObterCotasPorUsuario(Guid usuarioId)
        {
            try
            {
                var cotas = await _cotaService.ObterPorUsuarioAsync(usuarioId);
                return Ok(cotas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("getcotasopen/{rifaId}")]
        public async Task<IActionResult> ObterCotasDisponiveis(Guid rifaId)
        {
            try
            {
                var cotas = await _cotaService.ObterDisponiveisPorRifaAsync(rifaId);
                return Ok(cotas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("createcota")]
        public async Task<IActionResult> CriarCota([FromBody] CadastroCotaDTO cota)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var cotaCriada = await _cotaService.CriarAsync(cota);
                return CreatedAtAction(nameof(ObterCota), new { id = cotaCriada.Id }, cotaCriada);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("buycota")]
        public async Task<IActionResult> ComprarCota([FromBody] ComprarCotaDTO comprarCota)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _cotaService.ComprarCotaAsync(comprarCota);
                return Ok(new { message = "Cota comprada com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("deletecota/{id}")]
        public async Task<IActionResult> ExcluirCota(Guid id)
        {
            try
            {
                await _cotaService.ExcluirAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}