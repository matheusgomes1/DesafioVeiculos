using DesafioVeiculos.Domain.Entities;
using DesafioVeiculos.Domain.Interfaces;
using DesafioVeiculos.Infra.Data.Repository;
using DesafioVeiculos.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace DesafioVeiculos.Domain.Services
{
    public class VeiculoService : IVeiculoService
    {
        private readonly IRepository<Veiculo> _veiculoRepository;

        public VeiculoService(IRepository<Veiculo> veiculoRepository)
        {
            _veiculoRepository = veiculoRepository;
        }

        public async Task<PagedResult<Veiculo>> GetVeiculosPaginadosAsync(int page, int pageSize, string placa, string modelo, int? ano, string cor)
        {
            var query = _veiculoRepository.AsQueryable();

            if (!string.IsNullOrEmpty(placa))
                query = query.Where(v => v.Placa.Contains(placa));

            if (!string.IsNullOrEmpty(modelo))
                query = query.Where(v => v.Modelo.Contains(modelo));

            if (ano.HasValue)
                query = query.Where(v => v.Ano == ano.Value);

            if (!string.IsNullOrEmpty(cor))
                query = query.Where(v => v.Cor.Contains(cor));

            var totalItems = query.Count();
            var veiculos = await query.Skip((page - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToListAsync();

            return new PagedResult<Veiculo>
            {
                Items = veiculos,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize
            };
        }
    }
}
