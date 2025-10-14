using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rifa.Application.Dto.Usuario;
using Rifa.Application.Enums;
using Rifa.Application.Interfaces;

namespace Rifa.API.Controllers.Usuario
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        [Route("getallusers")]
        public async Task<IActionResult> ObterUsuarios()
        {
            try
            {
                var usuarios = await _usuarioService.ObterTodosAsync();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("getuser/{id}")]
        public async Task<IActionResult> ObterUsuario(Guid id)
        {
            try
            {
                var usuario = await _usuarioService.ObterPorIdAsync(id);
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("createuser")]
        public async Task<IActionResult> CriarUsuario([FromBody] CadastroUsuarioDTO usuario)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuarioCriado = await _usuarioService.CriarAsync(usuario);
                return CreatedAtAction(nameof(ObterUsuario), new { id = usuarioCriado.Id }, usuarioCriado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut]
        [Route("updateuser/{id}")]
        public async Task<IActionResult> AtualizarUsuario(Guid id, [FromBody] AtualizacaoUsuarioDTO usuario)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuarioAtualizado = await _usuarioService.AtualizarAsync(id, usuario);
                return Ok(usuarioAtualizado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete]
        [Route("desableuser/{id}")]
        public async Task<IActionResult> ExcluirUsuario(Guid id)
        {
            try
            {
                await _usuarioService.ExcluirAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUsuarioDTO login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var tokenResponse = await _usuarioService.LoginAsync(login);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.Parse(tokenResponse.ExpiresAt)
                };

                Response.Cookies.Append("AuthToken", tokenResponse.Token, cookieOptions);

                return Ok(new
                {
                    message = "Login realizado com sucesso",
                    usuarioId = tokenResponse.UsuarioId,
                    nome = tokenResponse.Nome,
                    email = tokenResponse.Email,
                    role = tokenResponse.Role,
                    token = tokenResponse.Token,
                    expiresAt = tokenResponse.ExpiresAt
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("me")]
        public IActionResult ObterUsuarioAutenticado()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var nome = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
                {
                    return Unauthorized(new 
                    { 
                        message = "Token inválido ou corrompido. Faça login novamente." 
                    });
                }

                if (!Enum.TryParse<UsuarioRole>(role, out var roleEnum))
                {
                    return Unauthorized(new 
                    { 
                        message = "Role inválido no token. Faça login novamente." 
                    });
                }

                return Ok(new
                {
                    usuarioId = userId,
                    nome = nome,
                    email = email,
                    role = role,
                    isAdmin = roleEnum == UsuarioRole.Administrador
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    message = "Erro ao processar dados do usuário",
                    details = ex.Message 
                });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            return Ok(new { message = "Logout realizado com sucesso" });
        }

        [Authorize]
        [HttpPut]
        [Route("changepassword/{id}")]
        public async Task<IActionResult> AlterarSenha(Guid id, [FromBody] AlterarSenhaDTO alterarSenha)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _usuarioService.AlterarSenhaAsync(id, alterarSenha);
                return Ok(new { message = "Senha alterada com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut]
        [Route("changerole/{id}")]
        public async Task<IActionResult> AlterarRole(Guid id, [FromBody] AlterarRoleDTO alterarRole)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _usuarioService.AlterarRoleAsync(id, alterarRole);
                return Ok(new { message = "Role alterada com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}