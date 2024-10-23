using DesafioVeiculos.Domain.Dtos;
using DesafioVeiculos.Domain.Interfaces;
using DesafioVeiculos.Infra.Core.Controller;
using DesafioVeiculos.Infra.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace DesafioVeiculos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeiculoController : BaseApiController
    {
        private readonly IVeiculoService _veiculoService;

        public VeiculoController(IVeiculoService veiculoService, INotification notification)
            :base(notification)
        {
            _veiculoService = veiculoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetVeiculos(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string texto = "",
            [FromQuery] string orderBy = "Id",
            [FromQuery] bool desc = false)
        {
            var result = await _veiculoService.ObterVeiculosPaginadosAsync(page, pageSize, texto, orderBy, desc);

            return CustomResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> PostVeiculo([FromBody] VeiculoPostDto dto)
        {
            await _veiculoService.AdicionarVeiculoAsync(dto);

            return CustomResponse(true);
        }

        [HttpPut]
        public async Task<IActionResult> AtualizarVeiculo([FromBody] VeiculoPutDto dto)
        {
            await _veiculoService.AtualizarVeiculoAsync(dto);

            return CustomResponse(true);
        }

        [HttpGet("ObterPorId/{id}")]
        public async Task<IActionResult> GetPorId(int id)
        {
            var result = await _veiculoService.ObterPorIdAsync(id);

            return CustomResponse(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarVeiculo(int id)
        {
            await _veiculoService.DeletarAsync(id);

            return CustomResponse(true);
        }
    }
}
