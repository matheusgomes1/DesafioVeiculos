namespace DesafioVeiculos.Infra.Core.Models
{
    public class CustomResult
    {
        public bool success { get; set; }
        public object? data { get; set; }
        public IEnumerable<string> errors { get; set; }
    }
}
