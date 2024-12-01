﻿using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class Auth
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}