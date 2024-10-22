using DesafioVeiculos.Domain.Entities;
using DesafioVeiculos.Infra.Data;

namespace DesafioVeiculos.Domain.Interfaces
{
    public interface IVeiculoService
    {
        Task<PagedResult<Veiculo>> GetVeiculosPaginadosAsync(int page, int pageSize, string placa, string modelo, int? ano, string cor);
    }
}
