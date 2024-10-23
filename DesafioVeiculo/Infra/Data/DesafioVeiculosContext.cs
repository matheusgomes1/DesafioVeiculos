using DesafioVeiculos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DesafioVeiculos.Infra.Data
{
    public class DesafioVeiculosContext : DbContext
    {
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Carro> Carros { get; set; }
        public DbSet<Caminhao> Caminhoes { get; set; }
        public DbSet<Revisao> Revisoes { get; set; }

        public DesafioVeiculosContext(DbContextOptions<DesafioVeiculosContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mapeamento da estratégia TPT
            modelBuilder.Entity<Veiculo>()
                .ToTable("Veiculo")
                .HasKey(v => v.Id);

            modelBuilder.Entity<Carro>()
                .ToTable("Carro")
                .HasBaseType<Veiculo>();

            modelBuilder.Entity<Caminhao>()
                .ToTable("Caminhao")
                .HasBaseType<Veiculo>();

            modelBuilder.Entity<Revisao>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.ToTable("Revisao");
                entity.Property(r => r.ValorDaRevisao).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Revisao>()
                .HasOne(r => r.Veiculo)
                .WithMany(v => v.Revisoes)
                .HasForeignKey(r => r.VeiculoId);
        }
    }
}
