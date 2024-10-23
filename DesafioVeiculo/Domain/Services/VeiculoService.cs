using DesafioVeiculos.Domain.Entities;
using DesafioVeiculos.Domain.Interfaces;
using DesafioVeiculos.Infra.Data.Repository;
using DesafioVeiculos.Infra.Data;
using Microsoft.EntityFrameworkCore;
using DesafioVeiculos.Domain.Dtos;
using DesafioVeiculos.Domain.Enums;
using System.Reflection;
using DesafioVeiculos.Infra.Core.Services;

namespace DesafioVeiculos.Domain.Services
{
    public class VeiculoService : IVeiculoService
    {
        private readonly IRepository<Veiculo> _veiculoRepository;
        private readonly IRepository<Revisao> _revisaoRepository;
        private readonly INotification _notification;

        public VeiculoService(IRepository<Veiculo> veiculoRepository, IRepository<Revisao> revisaoRepository, INotification notification)
        {
            _veiculoRepository = veiculoRepository;
            _revisaoRepository = revisaoRepository;
            _notification = notification;
        }

        public async Task<PagedResult<VeiculoGetDto>> ObterVeiculosPaginadosAsync(int page, int pageSize, string texto, string orderBy = "Id", bool desc = false)
        {
            var query = _veiculoRepository.AsQueryable();

            if (int.TryParse(texto, out var ano))
                query = query.Where(v => v.Ano == ano);
            else if (!string.IsNullOrEmpty(texto))
                query = query.Where(v => v.Placa.Contains(texto) || v.Cor.Contains(texto) || v.Modelo.Contains(texto));

            query = AplicarOrdenacao(query, orderBy, desc);

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

        private IQueryable<Veiculo> AplicarOrdenacao(IQueryable<Veiculo> query, string orderBy, bool desc)
        {
            var propertyInfo = typeof(Veiculo).GetProperty(orderBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null)
            {
                throw new ArgumentException($"A coluna '{orderBy}' não é válida para ordenação.");
            }

            if (desc)
            {
                return query.OrderByDescending(p => EF.Property<object>(p, orderBy));
            }
            else
            {
                return query.OrderBy(p => EF.Property<object>(p, orderBy));
            }
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
                _notification.Add(new("Tipo de veículo inválido"));
                return null;
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
                _notification.Add(new("Veículo não encontrado."));
                return;
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
                _notification.Add(new("Veículo não encontrado."));
                return null;
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
                _notification.Add(new("Veículo não encontrado."));
                return;
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
