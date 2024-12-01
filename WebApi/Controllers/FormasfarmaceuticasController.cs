using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormasfarmaceuticasController : ControllerBase
    {
        private readonly CepdiPruebaContext _context;

        public FormasfarmaceuticasController(CepdiPruebaContext context)
        {
            _context = context;
        }

        // GET: api/Formasfarmaceuticas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Formasfarmaceutica>>> GetFormasfarmaceuticas()
        {
            return await _context.Formasfarmaceuticas.ToListAsync();
        }

        // GET: api/Formasfarmaceuticas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Formasfarmaceutica>> GetFormasfarmaceutica(int id)
        {
            var formasfarmaceutica = await _context.Formasfarmaceuticas.FindAsync(id);

            if (formasfarmaceutica == null)
            {
                return NotFound();
            }

            return formasfarmaceutica;
        }

        // PUT: api/Formasfarmaceuticas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFormasfarmaceutica(int id, Formasfarmaceutica formasfarmaceutica)
        {
            if (id != formasfarmaceutica.Idformafarmaceutica)
            {
                return BadRequest();
            }

            _context.Entry(formasfarmaceutica).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FormasfarmaceuticaExists(id))
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

        // POST: api/Formasfarmaceuticas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Formasfarmaceutica>> PostFormasfarmaceutica(Formasfarmaceutica formasfarmaceutica)
        {
            _context.Formasfarmaceuticas.Add(formasfarmaceutica);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFormasfarmaceutica", new { id = formasfarmaceutica.Idformafarmaceutica }, formasfarmaceutica);
        }

        // DELETE: api/Formasfarmaceuticas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFormasfarmaceutica(int id)
        {
            var formasfarmaceutica = await _context.Formasfarmaceuticas.FindAsync(id);
            if (formasfarmaceutica == null)
            {
                return NotFound();
            }

            _context.Formasfarmaceuticas.Remove(formasfarmaceutica);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FormasfarmaceuticaExists(int id)
        {
            return _context.Formasfarmaceuticas.Any(e => e.Idformafarmaceutica == id);
        }
    }
}
