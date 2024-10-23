using DesafioVeiculos.Infra.Core.Models;
using DesafioVeiculos.Infra.Core.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace DesafioVeiculos.Infra.Core.Controller
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {

        private readonly INotification _notification;

        public BaseApiController(INotification notification)
        {
            _notification = notification;
        }

        protected bool isValid()
        {
            return !_notification.HasNotifications;
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if (isValid())
            {
                return Ok(new CustomResult() {
                    success = true,
                    data = result
                });
            }

            return BadRequest(new CustomResult()
            {
                success = false,
                errors = _notification.Notifications.Select(n => n.ToString())
            });
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid) NotificationModelisValid(modelState);
            return CustomResponse();
        }

        protected void NotificationModelisValid(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(e => e.Errors);
            foreach (var erro in erros)
            {
                var errorMsg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
                AddNotification(errorMsg);
            }
        }

        protected void AddNotification(string mensagem)
        {
            _notification.Add(new Description(mensagem));
        }
    }

}
