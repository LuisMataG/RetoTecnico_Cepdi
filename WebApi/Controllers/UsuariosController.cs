using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly CepdiPruebaContext _context;

        public UsuariosController(CepdiPruebaContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) page = 10;

            try
            {
                // Calcular la cantidad de registros que se deben omitir
                var skip = (page - 1) * pageSize;

                // Obtener los usuarios con paginación (salta 'skip' registros y toma 'pageSize' registros)
                var usuarios = await _context.Usuarios
                                             .OrderBy(u => u.Nombre)
                                             .Skip(skip)              // Saltar a la página correspondiente
                                             .Take(pageSize)          // Tomar el número de usuarios para la página actual
                                             .ToListAsync();

                // Obtener el número total de registros para la paginación
                var totalUsuarios = await _context.Usuarios.CountAsync();

                // Crear un objeto para devolver tanto los usuarios como los metadatos de paginación
                var resultado = new
                {
                    success = true,  // Indicamos que la operación fue exitosa
                    totalUsuarios = totalUsuarios,
                    totalPaginas = (int)Math.Ceiling((double)totalUsuarios / pageSize),
                    paginaActual = page,
                    usuarios = usuarios
                };

                return Ok(resultado); 
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,  // Indicamos que hubo un error
                    message = "Hubo un error al obtener los usuarios: " + ex.Message
                });
            }
        }


        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Idusuario)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.Idusuario }, usuario);
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Idusuario == id);
        }

        [HttpGet("username/{username}")]
        public async Task<ActionResult<Usuario>> GetUsuarioByUsername(string username)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Usuario1 == username);

            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return usuario;
        }
    }
}
