using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Models;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUsuarioService _usuarioService;

        public AuthController(IConfiguration configuration, IUsuarioService usuarioService)
        {
            _configuration = configuration;
            _usuarioService = usuarioService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Auth auth)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar las credenciales del usuario usando el servicio
            var usuario = await _usuarioService.GetUsuarioByUsernameAsync(auth.Username);

            if (usuario == null || usuario.Password != auth.Password)
            {
                return Unauthorized(new { message = "Usuario o contraseña incorrectos." });
            }

            // Generar el JWT
            var token = GenerateJwtToken(usuario);

            return Ok(new { token });
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var secretKey = _configuration["Jwt:Key"];
            if (usuario.Usuario1 != null && !string.IsNullOrEmpty(secretKey))
            {
                // Configurar las reclamaciones (claims) del JWT
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, usuario.Usuario1),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                // Clave secreta para firmar el JWT
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // Crear el token
                // Crea una nueva instancia de un JwtSecurityToken
                var token = new JwtSecurityToken(

                    
                    issuer: _configuration["Jwt:Issuer"], // Establece el 'issuer' (emisor) del token, que es la entidad que genera el token.
                    audience: _configuration["Jwt:Audience"],// Establece la 'audience' (audiencia) del token, que es la entidad a la que está destinado el token.
                    // Establece las 'claims' (reclamaciones) del token, que contienen información adicional sobre el usuario.
                    // 'claims' es un array de objetos Claim, que puede incluir datos como el nombre de usuario o roles del usuario.
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(60),// Esto es útil para garantizar que los tokens no sean válidos de forma indefinida.
                    // Establece las credenciales de firma. 'creds' contiene la clave secreta y el algoritmo de firma a utilizar.
                    // Esto asegura que el token no pueda ser modificado sin invalidarlo.
                    signingCredentials: creds
                );


                // Retornar el token en formato string
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            else
            {
                return "";
            }
            
        }
    }
}
