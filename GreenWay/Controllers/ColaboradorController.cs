using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreenWay.Data;
using GreenWay.Models;
using System.Diagnostics;

namespace GreenWay.Controllers
{
    /// <summary>
    /// Controlador responsável pelo gerenciamento de colaboradores.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ColaboradorController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ColaboradorController> _logger;
        private readonly ActivitySource _activitySource;

        public ColaboradorController(AppDbContext context, ILogger<ColaboradorController> logger, ActivitySource activitySource)
        {
            _context = context;
            _logger = logger;
            _activitySource = activitySource;
        }

        /// <summary>
        /// Retorna todos os colaboradores com paginação e links HATEOAS.
        /// </summary>
        /// <param name="pageNumber">Número da página (padrão 1).</param>
        /// <param name="pageSize">Quantidade de itens por página (padrão 10).</param>
        /// <returns>Lista paginada de colaboradores com links HATEOAS.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<object>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            using var activity = _activitySource.StartActivity("GetAllColaboradores");
            activity?.SetTag("pageNumber", pageNumber);
            activity?.SetTag("pageSize", pageSize);

            _logger.LogInformation("Buscando colaboradores - Página: {PageNumber}, Tamanho: {PageSize}", pageNumber, pageSize);

            try
            {
                var colaboradores = await _context.Colaboradores
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = colaboradores.Select(c => new
                {
                    c.Id,
                    c.Nome,
                    c.Email,
                    c.Endereco,
                    c.MeioTransporte,
                    c.HorarioEntrada,
                    c.HorarioSaida,
                    c.DisponivelCaronas,
                    links = CreateLinks("Colaborador", c.Id)
                });

                _logger.LogInformation("Retornados {Count} colaboradores", colaboradores.Count);
                activity?.SetTag("resultCount", colaboradores.Count);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar colaboradores");
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Retorna um colaborador pelo seu ID.
        /// </summary>
        /// <param name="id">ID do colaborador.</param>
        /// <returns>Objeto do colaborador correspondente ao ID fornecido.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Colaborador>> GetById(int id)
        {
            using var activity = _activitySource.StartActivity("GetColaboradorById");
            activity?.SetTag("colaboradorId", id);

            _logger.LogInformation("Buscando colaborador com ID: {Id}", id);

            var colaborador = await _context.Colaboradores.FindAsync(id);
            if (colaborador == null)
            {
                _logger.LogWarning("Colaborador com ID {Id} não encontrado", id);
                activity?.SetTag("found", false);
                return NotFound();
            }

            _logger.LogInformation("Colaborador {Id} encontrado: {Nome}", id, colaborador.Nome);
            activity?.SetTag("found", true);
            return Ok(colaborador);
        }

        /// <summary>
        /// Busca colaboradores por email corporativo.
        /// </summary>
        /// <param name="email">Email corporativo do colaborador.</param>
        /// <returns>Colaborador com o email fornecido.</returns>
        [HttpGet("email/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Colaborador>> GetByEmail(string email)
        {
            var colaborador = await _context.Colaboradores
                .FirstOrDefaultAsync(c => c.Email == email);
            
            if (colaborador == null) return NotFound();
            return Ok(colaborador);
        }

        /// <summary>
        /// Busca colaboradores disponíveis para caronas próximos a um endereço.
        /// </summary>
        /// <param name="endereco">Endereço para busca de proximidade.</param>
        /// <returns>Lista de colaboradores disponíveis para caronas.</returns>
        [HttpGet("disponiveis-caronas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Colaborador>>> GetDisponiveisCaronas()
        {
            var colaboradores = await _context.Colaboradores
                .Where(c => c.DisponivelCaronas == true)
                .ToListAsync();

            return Ok(colaboradores);
        }

        /// <summary>
        /// Cria um novo colaborador.
        /// </summary>
        /// <param name="colaborador">Objeto do colaborador a ser criado.</param>
        /// <returns>O colaborador criado com o ID gerado.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Colaborador>> CreateColaborador(Colaborador colaborador)
        {
            using var activity = _activitySource.StartActivity("CreateColaborador");
            activity?.SetTag("colaborador.email", colaborador.Email);

            _logger.LogInformation("Criando novo colaborador: {Email}", colaborador.Email);

            try
            {
                _context.Colaboradores.Add(colaborador);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Colaborador criado com sucesso - ID: {Id}, Email: {Email}", colaborador.Id, colaborador.Email);
                activity?.SetTag("colaboradorId", colaborador.Id);

                return CreatedAtAction(nameof(GetById), new { id = colaborador.Id }, colaborador);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar colaborador: {Email}", colaborador.Email);
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Atualiza um colaborador existente pelo ID.
        /// </summary>
        /// <param name="id">ID do colaborador a ser atualizado.</param>
        /// <param name="colaborador">Objeto do colaborador com as alterações.</param>
        /// <returns>Sem conteúdo se atualizado com sucesso.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateColaborador(int id, Colaborador colaborador)
        {
            if (id != colaborador.Id) return BadRequest();

            _context.Entry(colaborador).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Colaboradores.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Remove um colaborador pelo ID.
        /// </summary>
        /// <param name="id">ID do colaborador a ser removido.</param>
        /// <returns>Sem conteúdo se removido com sucesso.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteColaborador(int id)
        {
            using var activity = _activitySource.StartActivity("DeleteColaborador");
            activity?.SetTag("colaboradorId", id);

            _logger.LogInformation("Removendo colaborador com ID: {Id}", id);

            var colaborador = await _context.Colaboradores.FindAsync(id);
            if (colaborador == null)
            {
                _logger.LogWarning("Colaborador com ID {Id} não encontrado para remoção", id);
                activity?.SetTag("found", false);
                return NotFound();
            }

            _context.Colaboradores.Remove(colaborador);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Colaborador {Id} removido com sucesso", id);
            activity?.SetTag("found", true);
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
