using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DTO;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicamentosController : ControllerBase
    {
        private readonly CepdiPruebaContext _context;

        public MedicamentosController(CepdiPruebaContext context)
        {
            _context = context;
        }

        // GET: api/Medicamentos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Medicamento>>> GetMedicamentos(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            try
            {
                var skip = (page - 1) * pageSize;

                var medicamentos = await _context.Medicamentos
                    .Include(m => m.IdformafarmaceuticaNavigation)
                    .OrderBy(m => m.Nombre) 
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();

                var medicamentosDTO = medicamentos.Select(m => new MedicamentoDTO
                {
                    Idmedicamento = m.Idmedicamento,
                    Nombre = m.Nombre,
                    Concentracion = m.Concentracion,
                    Precio = m.Precio,
                    Stock = m.Stock,
                    Presentacion = m.Presentacion,
                    FormaFarmaceuticaNombre = m.IdformafarmaceuticaNavigation?.Nombre,
                    Idformafarmaceutica = m.Idformafarmaceutica,
                    Bhabilitado = m.Bhabilitado
                }).ToList();

                // Obtener el número total de registros para la paginación
                var totalMedicamentos = await _context.Medicamentos.CountAsync();

                // Crear un objeto para devolver tanto los medicamentos como los metadatos de paginación
                var resultado = new
                {
                    success = true,  // Indicamos que la operación fue exitosa
                    totalMedicamentos = totalMedicamentos,
                    TotalPaginas = (int)Math.Ceiling((double)totalMedicamentos / pageSize),
                    paginaActual = page,
                    Medicamentos = medicamentosDTO
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,  // Indicamos que hubo un error
                    message = "Hubo un error al obtener los medicamentos: " + ex.Message
                });
            }
        }


        // GET: api/Medicamentoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicamentoDTO>> GetMedicamento(int id)
        {
            var medicamento = await _context.Medicamentos
                .Include(m => m.IdformafarmaceuticaNavigation)
                .FirstOrDefaultAsync(m => m.Idmedicamento == id);

            if (medicamento == null)
            {
                return NotFound();
            }

            var medicamentoDTO = new MedicamentoDTO
            {
                Idmedicamento = medicamento.Idmedicamento,
                Nombre = medicamento.Nombre,
                Concentracion = medicamento.Concentracion,
                Precio = medicamento.Precio,
                Stock = medicamento.Stock,
                Presentacion = medicamento.Presentacion,
                FormaFarmaceuticaNombre = medicamento.IdformafarmaceuticaNavigation?.Nombre,
                Idformafarmaceutica = medicamento.Idformafarmaceutica,
                Bhabilitado = medicamento.Bhabilitado
            };

            return Ok(medicamentoDTO);
        }


        // PUT: api/Medicamentoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedicamento(int id, Medicamento medicamento)
        {
            if (id != medicamento.Idmedicamento)
            {
                return BadRequest();
            }

            _context.Entry(medicamento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicamentoExists(id))
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

        // POST: api/Medicamentoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Medicamento>> PostMedicamento(Medicamento medicamento)
        {
            _context.Medicamentos.Add(medicamento);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMedicamento", new { id = medicamento.Idmedicamento }, medicamento);
        }

        // DELETE: api/Medicamentoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicamento(int id)
        {
            var medicamento = await _context.Medicamentos.FindAsync(id);
            if (medicamento == null)
            {
                return NotFound();
            }

            _context.Medicamentos.Remove(medicamento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MedicamentoExists(int id)
        {
            return _context.Medicamentos.Any(e => e.Idmedicamento == id);
        }
    }
}
