using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GreenWay.Controllers;
using GreenWay.Data;
using GreenWay.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GreenWay.Tests.Controllers
{
    public class ColaboradorControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ColaboradorControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove o DbContext real
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Adiciona DbContext em memória para testes
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString());
                    });
                });
            });

            _client = _factory.CreateClient();
            _client.DefaultRequestHeaders.Add("X-Api-Key", "dev-secret-key-change-me");
        }

        [Fact]
        public async Task GetAll_DeveRetornarListaPaginada()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Adiciona dados de teste
            context.Colaboradores.AddRange(new[]
            {
                new Colaborador { Nome = "João Silva", Email = "joao@empresa.com", Endereco = "Rua A", MeioTransporte = "Carro", HorarioEntrada = "08:00", HorarioSaida = "18:00" },
                new Colaborador { Nome = "Maria Santos", Email = "maria@empresa.com", Endereco = "Rua B", MeioTransporte = "Bicicleta", HorarioEntrada = "09:00", HorarioSaida = "17:00" }
            });
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/v1/colaborador?pageNumber=1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var colaboradores = await response.Content.ReadFromJsonAsync<object[]>();
            colaboradores.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_ComIdValido_DeveRetornarColaborador()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var colaborador = new Colaborador
            {
                Nome = "Teste",
                Email = "teste@empresa.com",
                Endereco = "Rua Teste",
                MeioTransporte = "Carro",
                HorarioEntrada = "08:00",
                HorarioSaida = "18:00"
            };
            context.Colaboradores.Add(colaborador);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync($"/api/v1/colaborador/{colaborador.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<Colaborador>();
            result.Should().NotBeNull();
            result!.Nome.Should().Be("Teste");
        }

        [Fact]
        public async Task GetById_ComIdInvalido_DeveRetornarNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/colaborador/99999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateColaborador_ComDadosValidos_DeveCriarColaborador()
        {
            // Arrange
            var novoColaborador = new Colaborador
            {
                Nome = "Novo Colaborador",
                Email = "novo@empresa.com",
                Endereco = "Rua Nova",
                MeioTransporte = "Transporte Público",
                HorarioEntrada = "08:00",
                HorarioSaida = "18:00",
                DisponivelCaronas = true
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/colaborador", novoColaborador);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.Content.ReadFromJsonAsync<Colaborador>();
            created.Should().NotBeNull();
            created!.Id.Should().BeGreaterThan(0);
            created.Nome.Should().Be("Novo Colaborador");
        }

        [Fact]
        public async Task UpdateColaborador_ComDadosValidos_DeveAtualizarColaborador()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var colaborador = new Colaborador
            {
                Nome = "Original",
                Email = "original@empresa.com",
                Endereco = "Rua Original",
                MeioTransporte = "Carro",
                HorarioEntrada = "08:00",
                HorarioSaida = "18:00"
            };
            context.Colaboradores.Add(colaborador);
            await context.SaveChangesAsync();

            colaborador.Nome = "Atualizado";

            // Act
            var response = await _client.PutAsJsonAsync($"/api/v1/colaborador/{colaborador.Id}", colaborador);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteColaborador_ComIdValido_DeveRemoverColaborador()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var colaborador = new Colaborador
            {
                Nome = "Para Deletar",
                Email = "deletar@empresa.com",
                Endereco = "Rua Delete",
                MeioTransporte = "Carro",
                HorarioEntrada = "08:00",
                HorarioSaida = "18:00"
            };
            context.Colaboradores.Add(colaborador);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.DeleteAsync($"/api/v1/colaborador/{colaborador.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verifica se foi removido
            var deleted = await context.Colaboradores.FindAsync(colaborador.Id);
            deleted.Should().BeNull();
        }
    }
}

