using Contrive.Common;
using Contrive.Web.Common.Errors;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Web.Common.Modules
{
    public sealed class ErrorHandlingModule : HttpModuleBase
    {
        public ErrorHandlingModule()
        {
            var sl = ServiceLocator.Current;
            var errorHandler = sl.GetInstance<HttpErrorHandler>();
            var cryptographer = sl.GetInstance<ICryptographer>();
            var errorToken = cryptographer.GenerateToken();
            var request = _context.Request;
            var response = _context.Response;
            var currentError = _context.Error;
            var isCustomErrorEnabled = _context.IsCustomErrorEnabled;
            var contextItems = _context.Items;

            OnError = () =>
                      {
                          errorHandler.HandleError(request, response, ERROR_STATUS_CODE, currentError, isCustomErrorEnabled);

                          contextItems.Add(errorToken, true);
                      };

            OnEndRequest = () =>
                           {
                               if (!contextItems.Contains(errorToken) && response.StatusCode >= ERROR_STATUS_CODE)
                                   errorHandler.HandleError(request, response, response.StatusCode, currentError, isCustomErrorEnabled);
                           };
        }

        const int ERROR_STATUS_CODE = 400;

        protected override void OnDisposing(bool disposing)
        {
            OnError = null;
            OnEndRequest = null;
        }
    }
}