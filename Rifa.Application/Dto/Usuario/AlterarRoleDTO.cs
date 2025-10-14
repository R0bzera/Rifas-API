using System.ComponentModel.DataAnnotations;
using Rifa.Application.Enums;

namespace Rifa.Application.Dto.Usuario
{
    public class AlterarRoleDTO
    {
        [Required(ErrorMessage = "Role é obrigatória")]
        public UsuarioRole Role { get; set; }
    }
}
