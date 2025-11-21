using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using GreenWay.Data;
using Oracle.EntityFrameworkCore.Metadata;

#nullable disable

namespace GreenWay.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GreenWay.Models.Carona", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)")
                        .HasAnnotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1");

                    b.Property<DateTime>("DataCarona")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<double?>("DistanciaKm")
                        .HasColumnType("BINARY_DOUBLE");

                    b.Property<string>("Destino")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("NVARCHAR2(255)");

                    b.Property<string>("Horario")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("NVARCHAR2(10)");

                    b.Property<int>("MotoristaId")
                        .HasColumnType("NUMBER(10)");

                    b.Property<string>("Observacoes")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<int>("PassageiroId")
                        .HasColumnType("NUMBER(10)");

                    b.Property<string>("Origem")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("NVARCHAR2(255)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("NVARCHAR2(50)");

                    b.HasKey("Id");

                    b.HasIndex("MotoristaId");

                    b.HasIndex("PassageiroId");

                    b.ToTable("Caronas");
                });

            modelBuilder.Entity("GreenWay.Models.Colaborador", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)")
                        .HasAnnotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1");

                    b.Property<bool>("DisponivelCaronas")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("NVARCHAR2(255)");

                    b.Property<string>("Endereco")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("NVARCHAR2(255)");

                    b.Property<string>("HorarioEntrada")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("NVARCHAR2(10)");

                    b.Property<string>("HorarioSaida")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("NVARCHAR2(10)");

                    b.Property<string>("MeioTransporte")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("NVARCHAR2(50)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("NVARCHAR2(100)");

                    b.Property<string>("Observacoes")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.HasKey("Id");

                    b.ToTable("Colaboradores");
                });

            modelBuilder.Entity("GreenWay.Models.ImpactoAmbiental", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)")
                        .HasAnnotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1");

                    b.Property<int?>("CaronaId")
                        .HasColumnType("NUMBER(10)");

                    b.Property<int?>("ColaboradorId")
                        .HasColumnType("NUMBER(10)");

                    b.Property<double>("Co2PoupadoKg")
                        .HasColumnType("BINARY_DOUBLE");

                    b.Property<DateTime>("DataRegistro")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<string>("Descricao")
                        .HasMaxLength(255)
                        .HasColumnType("NVARCHAR2(255)");

                    b.Property<double>("DistanciaKm")
                        .HasColumnType("BINARY_DOUBLE");

                    b.Property<double>("KmEcologicos")
                        .HasColumnType("BINARY_DOUBLE");

                    b.Property<string>("TipoTransporte")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("NVARCHAR2(50)");

                    b.HasKey("Id");

                    b.HasIndex("CaronaId");

                    b.HasIndex("ColaboradorId");

                    b.ToTable("ImpactosAmbientais");
                });

            modelBuilder.Entity("GreenWay.Models.RotaSustentavel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)")
                        .HasAnnotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1");

                    b.Property<double>("Co2PoupadoKg")
                        .HasColumnType("BINARY_DOUBLE");

                    b.Property<int>("ColaboradorId")
                        .HasColumnType("NUMBER(10)");

                    b.Property<DateTime>("DataSugestao")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<string>("DescricaoRota")
                        .HasMaxLength(500)
                        .HasColumnType("NVARCHAR2(500)");

                    b.Property<string>("Destino")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("NVARCHAR2(255)");

                    b.Property<double>("DistanciaKm")
                        .HasColumnType("BINARY_DOUBLE");

                    b.Property<string>("Origem")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("NVARCHAR2(255)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("NVARCHAR2(50)");

                    b.Property<string>("TempoEstimado")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("NVARCHAR2(20)");

                    b.Property<string>("TipoRota")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("NVARCHAR2(50)");

                    b.HasKey("Id");

                    b.HasIndex("ColaboradorId");

                    b.ToTable("RotasSustentaveis");
                });

            modelBuilder.Entity("GreenWay.Models.Carona", b =>
                {
                    b.HasOne("GreenWay.Models.Colaborador", "Motorista")
                        .WithMany()
                        .HasForeignKey("MotoristaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("GreenWay.Models.Colaborador", "Passageiro")
                        .WithMany()
                        .HasForeignKey("PassageiroId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Motorista");

                    b.Navigation("Passageiro");
                });

            modelBuilder.Entity("GreenWay.Models.ImpactoAmbiental", b =>
                {
                    b.HasOne("GreenWay.Models.Carona", "Carona")
                        .WithMany()
                        .HasForeignKey("CaronaId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("GreenWay.Models.Colaborador", "Colaborador")
                        .WithMany()
                        .HasForeignKey("ColaboradorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Carona");

                    b.Navigation("Colaborador");
                });

            modelBuilder.Entity("GreenWay.Models.RotaSustentavel", b =>
                {
                    b.HasOne("GreenWay.Models.Colaborador", "Colaborador")
                        .WithMany()
                        .HasForeignKey("ColaboradorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Colaborador");
                });
#pragma warning restore 612, 618
        }
    }
}

