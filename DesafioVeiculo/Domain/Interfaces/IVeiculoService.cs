using DesafioVeiculos.Domain.Dtos;
using DesafioVeiculos.Domain.Entities;
using DesafioVeiculos.Infra.Data;

namespace DesafioVeiculos.Domain.Interfaces
{
    public interface IVeiculoService
    {
        Task<Veiculo> AdicionarVeiculoAsync(VeiculoPostDto veiculoDto);
        Task<VeiculoPorIdGetDto> ObterPorIdAsync(int id);
        Task AtualizarVeiculoAsync(VeiculoPutDto veiculoDto);
        Task DeletarAsync(int veiculoId);
        Task<PagedResult<VeiculoGetDto>> ObterVeiculosPaginadosAsync(int page, int pageSize, string texto, string orderBy = "Id", bool desc = false);
    }
}
