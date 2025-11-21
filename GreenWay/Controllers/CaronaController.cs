using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreenWay.Data;
using GreenWay.Models;
using System.Diagnostics;

namespace GreenWay.Controllers
{
    /// <summary>
    /// Controlador responsável pelo gerenciamento de caronas corporativas.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CaronaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CaronaController> _logger;
        private readonly ActivitySource _activitySource;

        public CaronaController(AppDbContext context, ILogger<CaronaController> logger, ActivitySource activitySource)
        {
            _context = context;
            _logger = logger;
            _activitySource = activitySource;
        }

        /// <summary>
        /// Retorna todas as caronas com paginação.
        /// </summary>
        /// <param name="pageNumber">Número da página (padrão 1).</param>
        /// <param name="pageSize">Quantidade de itens por página (padrão 10).</param>
        /// <returns>Lista paginada de caronas.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<object>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var caronas = await _context.Caronas
                .Include(c => c.Motorista)
                .Include(c => c.Passageiro)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = caronas.Select(c => new
            {
                c.Id,
                Motorista = new { c.Motorista!.Id, c.Motorista.Nome, c.Motorista.Email },
                Passageiro = new { c.Passageiro!.Id, c.Passageiro.Nome, c.Passageiro.Email },
                c.DataCarona,
                c.Horario,
                c.Origem,
                c.Destino,
                c.Status,
                c.DistanciaKm,
                links = CreateLinks("Carona", c.Id)
            });

            return Ok(result);
        }

        /// <summary>
        /// Retorna uma carona pelo seu ID.
        /// </summary>
        /// <param name="id">ID da carona.</param>
        /// <returns>Objeto da carona correspondente ao ID fornecido.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Carona>> GetById(int id)
        {
            var carona = await _context.Caronas
                .Include(c => c.Motorista)
                .Include(c => c.Passageiro)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (carona == null) return NotFound();
            return Ok(carona);
        }

        /// <summary>
        /// Busca caronas por colaborador (como motorista ou passageiro).
        /// </summary>
        /// <param name="colaboradorId">ID do colaborador.</param>
        /// <returns>Lista de caronas do colaborador.</returns>
        [HttpGet("colaborador/{colaboradorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Carona>>> GetByColaborador(int colaboradorId)
        {
            var caronas = await _context.Caronas
                .Include(c => c.Motorista)
                .Include(c => c.Passageiro)
                .Where(c => c.MotoristaId == colaboradorId || c.PassageiroId == colaboradorId)
                .ToListAsync();

            return Ok(caronas);
        }

        /// <summary>
        /// Cria uma nova carona.
        /// </summary>
        /// <param name="carona">Objeto da carona a ser criada.</param>
        /// <returns>A carona criada com o ID gerado.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Carona>> CreateCarona(Carona carona)
        {
            using var activity = _activitySource.StartActivity("CreateCarona");
            activity?.SetTag("carona.motoristaId", carona.MotoristaId);
            activity?.SetTag("carona.passageiroId", carona.PassageiroId);

            _logger.LogInformation("Criando nova carona - Motorista: {MotoristaId}, Passageiro: {PassageiroId}", carona.MotoristaId, carona.PassageiroId);

            try
            {
                _context.Caronas.Add(carona);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Carona criada com sucesso - ID: {Id}", carona.Id);
                activity?.SetTag("caronaId", carona.Id);

                return CreatedAtAction(nameof(GetById), new { id = carona.Id }, carona);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar carona");
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Atualiza uma carona existente pelo ID.
        /// </summary>
        /// <param name="id">ID da carona a ser atualizada.</param>
        /// <param name="carona">Objeto da carona com as alterações.</param>
        /// <returns>Sem conteúdo se atualizada com sucesso.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCarona(int id, Carona carona)
        {
            if (id != carona.Id) return BadRequest();

            _context.Entry(carona).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Caronas.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Remove uma carona pelo ID.
        /// </summary>
        /// <param name="id">ID da carona a ser removida.</param>
        /// <returns>Sem conteúdo se removida com sucesso.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCarona(int id)
        {
            var carona = await _context.Caronas.FindAsync(id);
            if (carona == null) return NotFound();

            _context.Caronas.Remove(carona);
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
