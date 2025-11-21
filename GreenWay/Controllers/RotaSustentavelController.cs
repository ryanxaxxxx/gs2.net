using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreenWay.Data;
using GreenWay.Models;
using System.Diagnostics;

namespace GreenWay.Controllers
{
    /// <summary>
    /// Controlador responsável pelo gerenciamento de rotas sustentáveis sugeridas.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class RotaSustentavelController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RotaSustentavelController> _logger;
        private readonly ActivitySource _activitySource;

        public RotaSustentavelController(AppDbContext context, ILogger<RotaSustentavelController> logger, ActivitySource activitySource)
        {
            _context = context;
            _logger = logger;
            _activitySource = activitySource;
        }

        /// <summary>
        /// Retorna todas as rotas sustentáveis com paginação.
        /// </summary>
        /// <param name="pageNumber">Número da página (padrão 1).</param>
        /// <param name="pageSize">Quantidade de itens por página (padrão 10).</param>
        /// <returns>Lista paginada de rotas sustentáveis.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<object>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var rotas = await _context.RotasSustentaveis
                .Include(r => r.Colaborador)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = rotas.Select(r => new
            {
                r.Id,
                Colaborador = new { r.Colaborador!.Id, r.Colaborador.Nome, r.Colaborador.Email },
                r.Origem,
                r.Destino,
                r.TipoRota,
                r.DistanciaKm,
                r.TempoEstimado,
                r.Co2PoupadoKg,
                r.Status,
                r.DataSugestao,
                links = CreateLinks("RotaSustentavel", r.Id)
            });

            return Ok(result);
        }

        /// <summary>
        /// Retorna uma rota sustentável pelo seu ID.
        /// </summary>
        /// <param name="id">ID da rota sustentável.</param>
        /// <returns>Objeto da rota sustentável correspondente ao ID fornecido.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RotaSustentavel>> GetById(int id)
        {
            var rota = await _context.RotasSustentaveis
                .Include(r => r.Colaborador)
                .FirstOrDefaultAsync(r => r.Id == id);
            
            if (rota == null) return NotFound();
            return Ok(rota);
        }

        /// <summary>
        /// Busca rotas sustentáveis por colaborador.
        /// </summary>
        /// <param name="colaboradorId">ID do colaborador.</param>
        /// <returns>Lista de rotas sustentáveis do colaborador.</returns>
        [HttpGet("colaborador/{colaboradorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RotaSustentavel>>> GetByColaborador(int colaboradorId)
        {
            var rotas = await _context.RotasSustentaveis
                .Include(r => r.Colaborador)
                .Where(r => r.ColaboradorId == colaboradorId)
                .ToListAsync();

            return Ok(rotas);
        }

        /// <summary>
        /// Cria uma nova rota sustentável.
        /// </summary>
        /// <param name="rota">Objeto da rota sustentável a ser criada.</param>
        /// <returns>A rota sustentável criada com o ID gerado.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<RotaSustentavel>> CreateRotaSustentavel(RotaSustentavel rota)
        {
            _context.RotasSustentaveis.Add(rota);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = rota.Id }, rota);
        }

        /// <summary>
        /// Atualiza uma rota sustentável existente pelo ID.
        /// </summary>
        /// <param name="id">ID da rota sustentável a ser atualizada.</param>
        /// <param name="rota">Objeto da rota sustentável com as alterações.</param>
        /// <returns>Sem conteúdo se atualizada com sucesso.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRotaSustentavel(int id, RotaSustentavel rota)
        {
            if (id != rota.Id) return BadRequest();

            _context.Entry(rota).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.RotasSustentaveis.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Remove uma rota sustentável pelo ID.
        /// </summary>
        /// <param name="id">ID da rota sustentável a ser removida.</param>
        /// <returns>Sem conteúdo se removida com sucesso.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRotaSustentavel(int id)
        {
            var rota = await _context.RotasSustentaveis.FindAsync(id);
            if (rota == null) return NotFound();

            _context.RotasSustentaveis.Remove(rota);
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
