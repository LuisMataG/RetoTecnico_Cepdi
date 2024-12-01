using System;
using System.ComponentModel.DataAnnotations;

namespace WebSite.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        [Required(ErrorMessage ="El campo nombre es obligatorio.")]
        public string Nombre { get; set; }
        public DateTime FechaCreacion { get; set; }
        [Required(ErrorMessage = "El campo usuario es obligatorio.")]
        public string Usuario1 { get; set; }
        [Required(ErrorMessage = "El campo contraseña es obligatorio.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\d])[A-Za-z\d\W]{8,}$", ErrorMessage = "La contraseña debe contener al menos una mayúscula, un número y un carácter especial.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Por favor seleccione un perfil.")]
        public int IdPerfil { get; set; }
        [Required(ErrorMessage = "Por favor seleccione un estatus.")]
        public int Estatus { get; set; }
    }
}