﻿// <auto-generated />
using System;
using DesafioVeiculos.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DesafioVeiculos.Migrations
{
    [DbContext(typeof(DesafioVeiculosContext))]
    partial class DesafioVeiculosContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DesafioVeiculos.Domain.Entities.Revisao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Data")
                        .HasColumnType("datetime2");

                    b.Property<int>("Km")
                        .HasColumnType("int");

                    b.Property<decimal>("ValorDaRevisao")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("VeiculoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VeiculoId");

                    b.ToTable("Revisao", (string)null);
                });

            modelBuilder.Entity("DesafioVeiculos.Domain.Entities.Veiculo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Ano")
                        .HasColumnType("int");

                    b.Property<string>("Cor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Modelo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Placa")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Veiculo", (string)null);

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("DesafioVeiculos.Domain.Entities.Caminhao", b =>
                {
                    b.HasBaseType("DesafioVeiculos.Domain.Entities.Veiculo");

                    b.Property<int>("CapacidadeCarga")
                        .HasColumnType("int");

                    b.ToTable("Caminhao", (string)null);
                });

            modelBuilder.Entity("DesafioVeiculos.Domain.Entities.Carro", b =>
                {
                    b.HasBaseType("DesafioVeiculos.Domain.Entities.Veiculo");

                    b.Property<int>("CapacidadePassageiro")
                        .HasColumnType("int");

                    b.ToTable("Carro", (string)null);
                });

            modelBuilder.Entity("DesafioVeiculos.Domain.Entities.Revisao", b =>
                {
                    b.HasOne("DesafioVeiculos.Domain.Entities.Veiculo", "Veiculo")
                        .WithMany("Revisoes")
                        .HasForeignKey("VeiculoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Veiculo");
                });

            modelBuilder.Entity("DesafioVeiculos.Domain.Entities.Caminhao", b =>
                {
                    b.HasOne("DesafioVeiculos.Domain.Entities.Veiculo", null)
                        .WithOne()
                        .HasForeignKey("DesafioVeiculos.Domain.Entities.Caminhao", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DesafioVeiculos.Domain.Entities.Carro", b =>
                {
                    b.HasOne("DesafioVeiculos.Domain.Entities.Veiculo", null)
                        .WithOne()
                        .HasForeignKey("DesafioVeiculos.Domain.Entities.Carro", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DesafioVeiculos.Domain.Entities.Veiculo", b =>
                {
                    b.Navigation("Revisoes");
                });
#pragma warning restore 612, 618
        }
    }
}