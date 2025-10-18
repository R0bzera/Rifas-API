using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rifa.Application.Dto.Rifa;
using Rifa.Application.Interfaces;

namespace Rifa.API.Controllers.Rifa
{
    [ApiController]
    [Route("api/[controller]")]
    public class RifaController : ControllerBase
    {
        private readonly IRifaService _rifaService;
        private readonly ICotaRepository _cotaRepository;
        private const string ImageDirectory = @"C:\imagens";

        public RifaController(IRifaService rifaService, ICotaRepository cotaRepository)
        {
            _rifaService = rifaService;
            _cotaRepository = cotaRepository;
            
            if (!Directory.Exists(ImageDirectory))
            {
                Directory.CreateDirectory(ImageDirectory);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("getrifas")]
        public async Task<IActionResult> ObterRifas()
        {
            try
            {
                var rifas = await _rifaService.ObterTodasAsync();
                return Ok(rifas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("getrifa/{id}")]
        public async Task<IActionResult> ObterRifa(Guid id)
        {
            try
            {
                var rifa = await _rifaService.ObterPorIdAsync(id);
                return Ok(rifa);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("getrifasativas")]
        public async Task<IActionResult> ObterRifasAtivas()
        {
            try
            {
                var rifas = await _rifaService.ObterAtivasAsync();
                return Ok(rifas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("image/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            try
            {
                var filePath = Path.Combine(ImageDirectory, fileName);
                
                if (!System.IO.File.Exists(filePath))
                    return NotFound(new { message = "Imagem não encontrada" });

                var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
                var contentType = fileExtension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".webp" => "image/webp",
                    _ => "application/octet-stream"
                };

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao carregar imagem: {ex.Message}" });
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [Route("uploadimage")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            try
            {
                if (image == null || image.Length == 0)
                    return BadRequest(new { message = "Nenhuma imagem foi enviada" });

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest(new { message = "Formato de imagem não permitido. Use: jpg, jpeg, png, gif ou webp" });

                if (image.Length > 5 * 1024 * 1024)
                    return BadRequest(new { message = "Imagem muito grande. Tamanho máximo: 5MB" });

                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(ImageDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                return Ok(new { imagePath = fileName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao salvar imagem: {ex.Message}" });
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [Route("createrifa")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CriarRifa([FromForm] CriarRifaRequest request)
        {
            try
            {
                string? imagePath = null;

                if (request.Imagem != null && request.Imagem.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var fileExtension = Path.GetExtension(request.Imagem.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(fileExtension))
                        return BadRequest(new { message = "Formato de imagem não permitido" });

                    if (request.Imagem.Length > 5 * 1024 * 1024)
                        return BadRequest(new { message = "Imagem muito grande. Tamanho máximo: 5MB" });

                    var fileName = $"{Guid.NewGuid()}{fileExtension}";
                    var filePath = Path.Combine(ImageDirectory, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.Imagem.CopyToAsync(stream);
                    }

                    imagePath = fileName;
                }

                var dto = new CadastroRifaDTO
                {
                    Titulo = request.Titulo,
                    Descricao = request.Descricao,
                    Imagem = imagePath,
                    Preco = request.Preco,
                    NumCotas = request.NumCotas
                };

                var rifaCriada = await _rifaService.CriarAsync(dto);
                return CreatedAtAction(nameof(ObterRifa), new { id = rifaCriada.Id }, rifaCriada);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut]
        [Route("updaterifa/{id}")]
        public async Task<IActionResult> AtualizarRifa(Guid id, [FromBody] AtualizacaoRifaDTO rifa)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var rifaAtualizada = await _rifaService.AtualizarAsync(id, rifa);
                return Ok(rifaAtualizada);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [Route("finishrifa/{id}")]
        public async Task<IActionResult> FinalizarRifa(Guid id, [FromBody] FinalizarRifaDTO finalizarRifa)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _rifaService.FinalizarAsync(id, finalizarRifa);
                return Ok(new { message = "Rifa finalizada com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete]
        [Route("deleterifa/{id}")]
        public async Task<IActionResult> ExcluirRifa(Guid id)
        {
            try
            {
                await _rifaService.ExcluirAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sortear/{id}")]
        public async Task<IActionResult> SortearRifa(Guid id)
        {
            try
            {
                var resultado = await _rifaService.SortearRifaAsync(id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("gerar-numero-sorteado/{id}")]
        public async Task<IActionResult> GerarNumeroSorteado(Guid id)
        {
            try
            {
                var resultado = await _rifaService.GerarNumeroSorteadoAsync(id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("status-sorteio/{id}")]
        public async Task<IActionResult> ObterStatusSorteio(Guid id)
        {
            try
            {
                var status = await _rifaService.ObterStatusSorteioAsync(id);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("verificar-sorteio-automatico/{id}")]
        public async Task<IActionResult> VerificarSorteioAutomatico(Guid id)
        {
            try
            {
                var rifa = await _rifaService.ObterPorIdAsync(id);
                if (rifa == null)
                    return NotFound(new { message = "Rifa não encontrada" });

                var cotas = await _cotaRepository.ObterPorRifaAsync(id);
                var cotasVendidas = cotas.Count(c => c.UsuarioId != null);
                var rifaCompleta = cotasVendidas == rifa.NumCotas;

                return Ok(new { 
                    rifaCompleta = rifaCompleta,
                    cotasVendidas = cotasVendidas,
                    totalCotas = rifa.NumCotas,
                    finalizada = rifa.Finalizada
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}