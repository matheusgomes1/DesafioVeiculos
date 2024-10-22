using DesafioVeiculos.Domain.Entities;
using DesafioVeiculos.Domain.Interfaces;
using DesafioVeiculos.Infra.Data.Repository;
using DesafioVeiculos.Infra.Data;
using Microsoft.EntityFrameworkCore;
using DesafioVeiculos.Domain.Dtos;
using DesafioVeiculos.Domain.Enums;

namespace DesafioVeiculos.Domain.Services
{
    public class VeiculoService : IVeiculoService
    {
        private readonly IRepository<Veiculo> _veiculoRepository;
        private readonly IRepository<Revisao> _revisaoRepository;

        public VeiculoService(IRepository<Veiculo> veiculoRepository, IRepository<Revisao> revisaoRepository)
        {
            _veiculoRepository = veiculoRepository;
            _revisaoRepository = revisaoRepository;
        }

        public async Task<PagedResult<VeiculoGetDto>> ObterVeiculosPaginadosAsync(int page, int pageSize, string placa, string modelo, int? ano, string cor)
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

            return new PagedResult<VeiculoGetDto>
            {
                Items = veiculos.Select(s => new VeiculoGetDto
                {
                    Id = s.Id,
                    Ano = s.Ano,
                    Cor = s.Cor,
                    Modelo = s.Modelo,
                    Placa = s.Placa
                }),
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<Veiculo> AdicionarVeiculoAsync(VeiculoPostDto veiculoDto)
        {
            Veiculo veiculo;
            if (veiculoDto.TipoVeiculo == ETipoVeiculo.CARRO)
            {
                veiculo = new Carro
                {
                    Placa = veiculoDto.Placa,
                    Ano = veiculoDto.Ano,
                    Cor = veiculoDto.Cor,
                    Modelo = veiculoDto.Modelo,
                    CapacidadePassageiro = veiculoDto.CapacidadePassageiro.GetValueOrDefault()
                };
            }
            else if (veiculoDto.TipoVeiculo == ETipoVeiculo.CAMINHAO)
            {
                veiculo = new Caminhao
                {
                    Placa = veiculoDto.Placa,
                    Ano = veiculoDto.Ano,
                    Cor = veiculoDto.Cor,
                    Modelo = veiculoDto.Modelo,
                    CapacidadeCarga = veiculoDto.CapacidadeCarga.GetValueOrDefault()
                };
            }
            else
            {
                throw new ArgumentException("Tipo de veículo inválido");
            }

            if (veiculoDto.Revisoes != null && veiculoDto.Revisoes.Any())
            {
                veiculo.Revisoes = veiculoDto.Revisoes.Select(r => new Revisao
                {
                    Km = r.Km,
                    Data = r.Data,
                    ValorDaRevisao = r.ValorDaRevisao
                }).ToList();
            }

            await _veiculoRepository.AdicionarAsync(veiculo);
            return veiculo;
        }

        public async Task AtualizarVeiculoAsync(VeiculoPutDto veiculoDto)
        {
            var veiculo = await _veiculoRepository.AsQueryable()
                                .Include(v => v.Revisoes)
                                .FirstOrDefaultAsync(v => v.Id == veiculoDto.Id);

            if (veiculo == null)
            {
                throw new KeyNotFoundException("Veículo não encontrado.");
            }

            veiculo.Placa = veiculoDto.Placa;
            veiculo.Ano = veiculoDto.Ano;
            veiculo.Cor = veiculoDto.Cor;
            veiculo.Modelo = veiculoDto.Modelo;

            if (veiculo is Carro carro && veiculoDto.TipoVeiculo == ETipoVeiculo.CARRO)
            {
                carro.CapacidadePassageiro = veiculoDto.CapacidadePassageiro ?? carro.CapacidadePassageiro;
            }
            else if (veiculo is Caminhao caminhao && veiculoDto.TipoVeiculo == ETipoVeiculo.CAMINHAO)
            {
                caminhao.CapacidadeCarga = veiculoDto.CapacidadeCarga ?? caminhao.CapacidadeCarga;
            }

            if (veiculoDto.Revisoes != null)
            {
                foreach (var revisaoDto in veiculoDto.Revisoes)
                {
                    var revisaoExistente = veiculo.Revisoes.FirstOrDefault(r => r.Id == revisaoDto.Id);

                    if (revisaoExistente != null)
                    {
                        revisaoExistente.Km = revisaoDto.Km;
                        revisaoExistente.Data = revisaoDto.Data;
                        revisaoExistente.ValorDaRevisao = revisaoDto.ValorDaRevisao;
                    }
                    else
                    {
                        veiculo.Revisoes.Add(new Revisao
                        {
                            Km = revisaoDto.Km,
                            Data = revisaoDto.Data,
                            ValorDaRevisao = revisaoDto.ValorDaRevisao
                        });
                    }
                }

                var revisoesDtoIds = veiculoDto.Revisoes.Where(r => r.Id.HasValue).Select(r => r.Id.Value).ToList();
                var revisoesParaRemover = veiculo.Revisoes.Where(r => r.Id != 0 && !revisoesDtoIds.Contains(r.Id)).ToList();

                foreach (var revisao in revisoesParaRemover)
                {
                    veiculo.Revisoes.Remove(revisao);
                    await _revisaoRepository.DeletarAsync(revisao);
                }
            }

            await _veiculoRepository.AtualizarAsync(veiculo);
        }

        public async Task<VeiculoPorIdGetDto> ObterPorIdAsync(int id)
        {
            var veiculo = await _veiculoRepository.AsQueryable()
                                .Include(v => v.Revisoes)
                                .FirstOrDefaultAsync(v => v.Id == id);

            if (veiculo == null)
            {
                throw new KeyNotFoundException("Veículo não encontrado.");
            }

            var veiculoDto = new VeiculoPorIdGetDto
            {
                Id = veiculo.Id,
                Placa = veiculo.Placa,
                Ano = veiculo.Ano,
                Cor = veiculo.Cor,
                Modelo = veiculo.Modelo,
                TipoVeiculo = veiculo is Carro ? ETipoVeiculo.CARRO : ETipoVeiculo.CAMINHAO,
                CapacidadePassageiro = veiculo is Carro carro ? carro.CapacidadePassageiro : null,
                CapacidadeCarga = veiculo is Caminhao caminhao ? caminhao.CapacidadeCarga : null,
                Revisoes = veiculo.Revisoes.Select(r => new RevisaoGetDto
                {
                    Id = r.Id,
                    Km = r.Km,
                    Data = r.Data,
                    ValorDaRevisao = r.ValorDaRevisao
                }).ToList()
            };

            return veiculoDto;
        }

        public async Task DeletarAsync(int veiculoId)
        {
            var veiculo = await _veiculoRepository.AsQueryable()
                                .Include(v => v.Revisoes)
                                .FirstOrDefaultAsync(v => v.Id == veiculoId);

            if (veiculo == null)
            {
                throw new KeyNotFoundException("Veículo não encontrado.");
            }

            var revisoesParaRemover = veiculo.Revisoes;

            foreach (var revisao in revisoesParaRemover)
            {
                veiculo.Revisoes.Remove(revisao);
                await _revisaoRepository.DeletarAsync(revisao);
            }

            await _veiculoRepository.DeletarAsync(veiculo);
        }
    }
}
