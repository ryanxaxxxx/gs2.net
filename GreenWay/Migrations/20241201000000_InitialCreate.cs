using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreenWay.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Colaboradores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Endereco = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    MeioTransporte = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    HorarioEntrada = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    HorarioSaida = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    DisponivelCaronas = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Observacoes = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colaboradores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Caronas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MotoristaId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PassageiroId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DataCarona = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Horario = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    Origem = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Destino = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DistanciaKm = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Observacoes = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caronas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Caronas_Colaboradores_MotoristaId",
                        column: x => x.MotoristaId,
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Caronas_Colaboradores_PassageiroId",
                        column: x => x.PassageiroId,
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImpactosAmbientais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ColaboradorId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    CaronaId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DataRegistro = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TipoTransporte = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DistanciaKm = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Co2PoupadoKg = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    KmEcologicos = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImpactosAmbientais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImpactosAmbientais_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImpactosAmbientais_Caronas_CaronaId",
                        column: x => x.CaronaId,
                        principalTable: "Caronas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RotasSustentaveis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ColaboradorId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Origem = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Destino = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    TipoRota = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DistanciaKm = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    TempoEstimado = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Co2PoupadoKg = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    DescricaoRota = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DataSugestao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RotasSustentaveis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RotasSustentaveis_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Caronas_MotoristaId",
                table: "Caronas",
                column: "MotoristaId");

            migrationBuilder.CreateIndex(
                name: "IX_Caronas_PassageiroId",
                table: "Caronas",
                column: "PassageiroId");

            migrationBuilder.CreateIndex(
                name: "IX_ImpactosAmbientais_CaronaId",
                table: "ImpactosAmbientais",
                column: "CaronaId");

            migrationBuilder.CreateIndex(
                name: "IX_ImpactosAmbientais_ColaboradorId",
                table: "ImpactosAmbientais",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_RotasSustentaveis_ColaboradorId",
                table: "RotasSustentaveis",
                column: "ColaboradorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImpactosAmbientais");

            migrationBuilder.DropTable(
                name: "RotasSustentaveis");

            migrationBuilder.DropTable(
                name: "Caronas");

            migrationBuilder.DropTable(
                name: "Colaboradores");
        }
    }
}

