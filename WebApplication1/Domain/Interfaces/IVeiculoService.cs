using DesafioVeiculos.Domain.Dtos;
using DesafioVeiculos.Domain.Entities;
using DesafioVeiculos.Infra.Data;

namespace DesafioVeiculos.Domain.Interfaces
{
    public interface IVeiculoService
    {
        Task<Veiculo> AdicionarVeiculoAsync(VeiculoPostDto veiculoDto);
        Task<PagedResult<VeiculoGetDto>> ObterVeiculosPaginadosAsync(int page, int pageSize, string placa, string modelo, int? ano, string cor);
        Task<VeiculoPorIdGetDto> ObterPorIdAsync(int id);
        Task AtualizarVeiculoAsync(VeiculoPutDto veiculoDto);
    }
}
