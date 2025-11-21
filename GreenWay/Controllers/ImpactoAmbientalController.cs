using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreenWay.Data;
using GreenWay.Models;
using System.Diagnostics;

namespace GreenWay.Controllers
{
    /// <summary>
    /// Controlador responsável pelo gerenciamento de impactos ambientais (CO₂ poupado, km ecológicos).
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ImpactoAmbientalController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ImpactoAmbientalController> _logger;
        private readonly ActivitySource _activitySource;

        public ImpactoAmbientalController(AppDbContext context, ILogger<ImpactoAmbientalController> logger, ActivitySource activitySource)
        {
            _context = context;
            _logger = logger;
            _activitySource = activitySource;
        }

        /// <summary>
        /// Retorna todos os impactos ambientais com paginação.
        /// </summary>
        /// <param name="pageNumber">Número da página (padrão 1).</param>
        /// <param name="pageSize">Quantidade de itens por página (padrão 10).</param>
        /// <returns>Lista paginada de impactos ambientais.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<object>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var impactos = await _context.ImpactosAmbientais
                .Include(i => i.Colaborador)
                .Include(i => i.Carona)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = impactos.Select(i => new
            {
                i.Id,
                Colaborador = i.Colaborador != null ? new { i.Colaborador.Id, i.Colaborador.Nome } : null,
                i.TipoTransporte,
                i.DistanciaKm,
                i.Co2PoupadoKg,
                i.KmEcologicos,
                i.DataRegistro,
                links = CreateLinks("ImpactoAmbiental", i.Id)
            });

            return Ok(result);
        }

        /// <summary>
        /// Retorna um impacto ambiental pelo seu ID.
        /// </summary>
        /// <param name="id">ID do impacto ambiental.</param>
        /// <returns>Objeto do impacto ambiental correspondente ao ID fornecido.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ImpactoAmbiental>> GetById(int id)
        {
            var impacto = await _context.ImpactosAmbientais
                .Include(i => i.Colaborador)
                .Include(i => i.Carona)
                .FirstOrDefaultAsync(i => i.Id == id);
            
            if (impacto == null) return NotFound();
            return Ok(impacto);
        }

        /// <summary>
        /// Calcula o total de CO₂ poupado por colaborador.
        /// </summary>
        /// <param name="colaboradorId">ID do colaborador.</param>
        /// <returns>Total de CO₂ poupado em kg.</returns>
        [HttpGet("colaborador/{colaboradorId}/total-co2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetTotalCo2PorColaborador(int colaboradorId)
        {
            var totalCo2 = await _context.ImpactosAmbientais
                .Where(i => i.ColaboradorId == colaboradorId)
                .SumAsync(i => i.Co2PoupadoKg);

            var totalKm = await _context.ImpactosAmbientais
                .Where(i => i.ColaboradorId == colaboradorId)
                .SumAsync(i => i.KmEcologicos);

            return Ok(new
            {
                ColaboradorId = colaboradorId,
                TotalCo2PoupadoKg = totalCo2,
                TotalKmEcologicos = totalKm
            });
        }

        /// <summary>
        /// Retorna o total geral de CO₂ poupado e km ecológicos.
        /// </summary>
        /// <returns>Total geral de impacto ambiental.</returns>
        [HttpGet("total-geral")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetTotalGeral()
        {
            var totalCo2 = await _context.ImpactosAmbientais
                .SumAsync(i => i.Co2PoupadoKg);

            var totalKm = await _context.ImpactosAmbientais
                .SumAsync(i => i.KmEcologicos);

            var totalRegistros = await _context.ImpactosAmbientais
                .CountAsync();

            return Ok(new
            {
                TotalCo2PoupadoKg = totalCo2,
                TotalKmEcologicos = totalKm,
                TotalRegistros = totalRegistros
            });
        }

        /// <summary>
        /// Cria um novo registro de impacto ambiental.
        /// </summary>
        /// <param name="impacto">Objeto do impacto ambiental a ser criado.</param>
        /// <returns>O impacto ambiental criado com o ID gerado.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ImpactoAmbiental>> CreateImpactoAmbiental(ImpactoAmbiental impacto)
        {
            _context.ImpactosAmbientais.Add(impacto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = impacto.Id }, impacto);
        }

        /// <summary>
        /// Atualiza um impacto ambiental existente pelo ID.
        /// </summary>
        /// <param name="id">ID do impacto ambiental a ser atualizado.</param>
        /// <param name="impacto">Objeto do impacto ambiental com as alterações.</param>
        /// <returns>Sem conteúdo se atualizado com sucesso.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateImpactoAmbiental(int id, ImpactoAmbiental impacto)
        {
            if (id != impacto.Id) return BadRequest();

            _context.Entry(impacto).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ImpactosAmbientais.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Remove um impacto ambiental pelo ID.
        /// </summary>
        /// <param name="id">ID do impacto ambiental a ser removido.</param>
        /// <returns>Sem conteúdo se removido com sucesso.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteImpactoAmbiental(int id)
        {
            var impacto = await _context.ImpactosAmbientais.FindAsync(id);
            if (impacto == null) return NotFound();

            _context.ImpactosAmbientais.Remove(impacto);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Helper para gerar links HATEOAS para a entidade.
        /// </summary>
        private object CreateLinks(string entityName, int id)
        {
            return new[]
            {
                new { rel = "self", href = Url.Action("GetById", entityName, new { id }) },
                new { rel = "update", href = Url.Action("Update" + entityName, entityName, new { id }) },
                new { rel = "delete", href = Url.Action("Delete" + entityName, entityName, new { id }) }
            };
        }
    }
}
