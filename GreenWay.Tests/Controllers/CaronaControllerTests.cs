using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GreenWay.Data;
using GreenWay.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GreenWay.Tests.Controllers
{
    public class CaronaControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public CaronaControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

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
        public async Task CreateCarona_ComDadosValidos_DeveCriarCarona()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var motorista = new Colaborador
            {
                Nome = "Motorista",
                Email = "motorista@empresa.com",
                Endereco = "Rua A",
                MeioTransporte = "Carro",
                HorarioEntrada = "08:00",
                HorarioSaida = "18:00"
            };

            var passageiro = new Colaborador
            {
                Nome = "Passageiro",
                Email = "passageiro@empresa.com",
                Endereco = "Rua B",
                MeioTransporte = "Carro",
                HorarioEntrada = "08:00",
                HorarioSaida = "18:00"
            };

            context.Colaboradores.AddRange(motorista, passageiro);
            await context.SaveChangesAsync();

            var carona = new Carona
            {
                MotoristaId = motorista.Id,
                PassageiroId = passageiro.Id,
                DataCarona = DateTime.Now.AddDays(1),
                Horario = "08:00",
                Origem = "Origem",
                Destino = "Destino",
                Status = "Agendada",
                DistanciaKm = 10.5
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/carona", carona);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.Content.ReadFromJsonAsync<Carona>();
            created.Should().NotBeNull();
            created!.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetAll_DeveRetornarListaPaginada()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var motorista = new Colaborador
            {
                Nome = "Motorista",
                Email = "motorista@empresa.com",
                Endereco = "Rua A",
                MeioTransporte = "Carro",
                HorarioEntrada = "08:00",
                HorarioSaida = "18:00"
            };

            var passageiro = new Colaborador
            {
                Nome = "Passageiro",
                Email = "passageiro@empresa.com",
                Endereco = "Rua B",
                MeioTransporte = "Carro",
                HorarioEntrada = "08:00",
                HorarioSaida = "18:00"
            };

            context.Colaboradores.AddRange(motorista, passageiro);
            await context.SaveChangesAsync();

            var carona = new Carona
            {
                MotoristaId = motorista.Id,
                PassageiroId = passageiro.Id,
                DataCarona = DateTime.Now,
                Horario = "08:00",
                Origem = "Origem",
                Destino = "Destino",
                Status = "Agendada"
            };

            context.Caronas.Add(carona);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/v1/carona?pageNumber=1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}

