namespace DesafioVeiculos.Domain.Dtos
{
    public class RevisaoPostDto
    {
        public int? Id { get; set; }
        public int Km { get; set; }
        public DateTime Data { get; set; }
        public decimal ValorDaRevisao { get; set; }
    }
}
