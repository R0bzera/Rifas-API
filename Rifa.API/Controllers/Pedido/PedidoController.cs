using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rifa.Application.Dto.Pedido;
using Rifa.Application.Interfaces;

namespace Rifa.API.Controllers.Pedido
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpGet]
        [Route("getpedidos")]
        public async Task<IActionResult> ObterPedidos()
        {
            try
            {
                var pedidos = await _pedidoService.ObterTodosAsync();
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("getpedido/{id}")]
        public async Task<IActionResult> ObterPedido(Guid id)
        {
            try
            {
                var pedido = await _pedidoService.ObterPorIdAsync(id);
                return Ok(pedido);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("getpedidosbyuser/{usuarioId}")]
        public async Task<IActionResult> ObterPedidosPorUsuario(Guid usuarioId)
        {
            try
            {
                var pedidos = await _pedidoService.ObterPorUsuarioAsync(usuarioId);
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("createpedido")]
        public async Task<IActionResult> CriarPedido([FromBody] CadastroPedidoDTO pedido)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var pedidoCriado = await _pedidoService.CriarAsync(pedido);
                return CreatedAtAction(nameof(ObterPedido), new { id = pedidoCriado.Id }, pedidoCriado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("confirmpagamento/{id}")]
        public async Task<IActionResult> ConfirmarPagamento(Guid id)
        {
            try
            {
                await _pedidoService.ConfirmarPagamentoAsync(id);
                return Ok(new { message = "Pagamento confirmado com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("deletepedido/{id}")]
        public async Task<IActionResult> ExcluirPedido(Guid id)
        {
            try
            {
                await _pedidoService.ExcluirAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}