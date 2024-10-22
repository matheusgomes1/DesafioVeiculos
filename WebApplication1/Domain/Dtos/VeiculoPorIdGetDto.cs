﻿using DesafioVeiculos.Domain.Enums;

namespace DesafioVeiculos.Domain.Dtos
{
    public class VeiculoPorIdGetDto
    {
        public int Id { get; set; }
        public string Placa { get; set; }
        public int Ano { get; set; }
        public string Cor { get; set; }
        public string Modelo { get; set; }
        public ETipoVeiculo TipoVeiculo { get; set; }
        public int? CapacidadeCarga { get; set; }
        public int? CapacidadePassageiro { get; set; }
        public List<RevisaoGetDto> Revisoes { get; set;}
    }
}