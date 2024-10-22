namespace DesafioVeiculos.Domain.Entities
{
    public abstract class Veiculo
    {
        public int Id { get; set; }
        public string Placa { get; set; }
        public int Ano { get; set; }
        public string Cor { get; set; }
        public string Modelo { get; set; }
        public ICollection<Revisao> Revisoes { get; set; }
    }
}
