using WebApi.Models;
using WebApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Services.Impementations
{
    public class UsuarioService : IUsuarioService
    {
        private readonly CepdiPruebaContext _context;

        public UsuarioService(CepdiPruebaContext context)
        {
            _context = context;
        }

        public async Task<Usuario> GetUsuarioByUsernameAsync(string username)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Usuario1 == username);
            return usuario ?? throw new Exception("Usuario no encontrado");
        }
    }

}
