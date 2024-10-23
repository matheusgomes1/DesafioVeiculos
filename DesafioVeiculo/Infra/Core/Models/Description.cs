namespace DesafioVeiculos.Infra.Core.Models
{
    public class Description
    {
        public string Message { get; }
        public Description(string message)
        {
            Message = message;
        }
        public override string ToString() => Message;
    }
}
