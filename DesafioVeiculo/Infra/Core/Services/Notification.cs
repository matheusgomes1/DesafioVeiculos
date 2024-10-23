using DesafioVeiculos.Infra.Core.Models;

namespace DesafioVeiculos.Infra.Core.Services
{
    public class Notification : INotification
    {
        public IList<object> Notifications { get; } = new List<object>();
        public bool HasNotifications => Notifications.Any();
        public bool Includes(Description error)
        {
            return Notifications.Contains(error);
        }
        public void Add(Description description)
        {
            Notifications.Add(description);
        }
    }
}
