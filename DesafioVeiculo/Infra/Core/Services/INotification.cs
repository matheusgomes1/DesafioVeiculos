using DesafioVeiculos.Infra.Core.Models;

namespace DesafioVeiculos.Infra.Core.Services
{
    public interface INotification
    {
        IList<object> Notifications { get; }
        bool HasNotifications { get; }
        bool Includes(Description error);
        void Add(Description error);
    }
}
