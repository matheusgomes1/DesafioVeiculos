namespace DesafioVeiculos.Domain.Entities
{
    public class Revisao
    {
        public int Id { get; set; }
        public int Km { get; set; }
        public DateTime Data { get; set; }
        public decimal ValorDaRevisao { get; set; }
        public int VeiculoId { get; set; } 
        public Veiculo Veiculo { get; set; }
    }
}
