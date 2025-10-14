using Rifa.Application.Dto.Usuario;
using Rifa.Application.Enums;
using Rifa.Application.Helpers;
using Rifa.Application.Interfaces;
using Rifa.Application.Models;
using Rifa.Domain.Usuario;

namespace Rifa.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;

        public UsuarioService(IUsuarioRepository usuarioRepository, ITokenService tokenService)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
        }

        public async Task<IEnumerable<UsuarioDTO>> ObterTodosAsync()
        {
            try
            {
                var usuarios = await _usuarioRepository.ObterTodosAsync();
                return usuarios.Select(u => MapToDTO(u));
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter usuários: {ex.Message}");
            }
        }

        public async Task<UsuarioDTO?> ObterPorIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new Exception("ID inválido");

                var usuario = await _usuarioRepository.ObterPorIdAsync(id);
                
                if (usuario == null)
                    throw new Exception($"Usuário com ID {id} não encontrado");

                return MapToDTO(usuario);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter usuário: {ex.Message}");
            }
        }

        public async Task<UsuarioDTO> CriarAsync(CadastroUsuarioDTO dto)
        {
            try
            {
                var emailExiste = await _usuarioRepository.EmailExisteAsync(dto.Email);
                if (emailExiste)
                    throw new Exception($"Email '{dto.Email}' já está cadastrado");

                if (dto.Senha.Length < 6)
                    throw new Exception("Senha deve ter pelo menos 6 caracteres");

                var usuario = new UsuarioEntity
                {
                    Nome = dto.Nome,
                    Email = dto.Email,
                    Telefone = dto.Telefone,
                    Senha = PasswordHelper.GerarHash(dto.Senha),
                    Role = (int)UsuarioRole.Usuario
                };

                var usuarioCriado = await _usuarioRepository.CriarAsync(usuario);
                return MapToDTO(usuarioCriado);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar usuário: {ex.Message}");
            }
        }

        public async Task<UsuarioDTO> AtualizarAsync(Guid id, AtualizacaoUsuarioDTO dto)
        {
            try
            {
                var usuarioExistente = await _usuarioRepository.ObterPorIdAsync(id);
                if (usuarioExistente == null)
                    throw new Exception($"Usuário com ID {id} não encontrado");

                if (usuarioExistente.Email != dto.Email)
                {
                    var emailExiste = await _usuarioRepository.EmailExisteAsync(dto.Email);
                    if (emailExiste)
                        throw new Exception($"Email '{dto.Email}' já está cadastrado");
                }

                usuarioExistente.Nome = dto.Nome;
                usuarioExistente.Email = dto.Email;
                usuarioExistente.Telefone = dto.Telefone;

                var usuarioAtualizado = await _usuarioRepository.AtualizarAsync(usuarioExistente);
                return MapToDTO(usuarioAtualizado);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar usuário: {ex.Message}");
            }
        }

        public async Task ExcluirAsync(Guid id)
        {
            try
            {
                var existe = await _usuarioRepository.ExisteAsync(id);
                if (!existe)
                    throw new Exception($"Usuário com ID {id} não encontrado");

                await _usuarioRepository.ExcluirAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir usuário: {ex.Message}");
            }
        }

        public async Task<TokenResponse> LoginAsync(LoginUsuarioDTO dto)
        {
            try
            {
                var usuario = await _usuarioRepository.ObterPorEmailAsync(dto.Email);
                if (usuario == null)
                    throw new Exception("Email ou senha inválidos");

                if (!PasswordHelper.VerificarSenha(dto.Senha, usuario.Senha))
                    throw new Exception("Email ou senha inválidos");

                var token = _tokenService.GerarToken(usuario);
                return token;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao fazer login: {ex.Message}");
            }
        }

        public async Task AlterarSenhaAsync(Guid id, AlterarSenhaDTO dto)
        {
            try
            {
                var usuario = await _usuarioRepository.ObterPorIdAsync(id);
                if (usuario == null)
                    throw new Exception($"Usuário com ID {id} não encontrado");

                if (!PasswordHelper.VerificarSenha(dto.SenhaAtual, usuario.Senha))
                    throw new Exception("Senha atual incorreta");

                if (dto.NovaSenha.Length < 6)
                    throw new Exception("Nova senha deve ter pelo menos 6 caracteres");

                usuario.Senha = PasswordHelper.GerarHash(dto.NovaSenha);
                await _usuarioRepository.AtualizarAsync(usuario);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao alterar senha: {ex.Message}");
            }
        }

        public async Task AlterarRoleAsync(Guid id, AlterarRoleDTO dto)
        {
            try
            {
                var usuario = await _usuarioRepository.ObterPorIdAsync(id);
                if (usuario == null)
                    throw new Exception($"Usuário com ID {id} não encontrado");

                usuario.Role = (int)dto.Role;
                await _usuarioRepository.AtualizarAsync(usuario);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao alterar role: {ex.Message}");
            }
        }

        private UsuarioDTO MapToDTO(UsuarioEntity entity)
        {
            return new UsuarioDTO
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Email = entity.Email,
                Telefone = entity.Telefone,
                DataCriacao = entity.DataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                DataAlteracao = entity.DataAlteracao.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }
    }
}