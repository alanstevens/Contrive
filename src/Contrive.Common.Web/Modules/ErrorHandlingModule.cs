using System;
using System.Web;
using Contrive.Common.Extensions;
using Contrive.Common.Web.Errors;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Common.Web.Modules
{
    public sealed class ErrorHandlingModule : HttpModuleBase
    {
        public ErrorHandlingModule()
        {
            var errorHandledKey = Guid.Empty;

            OnError = () =>
                      {
                          HttpErrorHandler.HandleError(Context, ERROR_STATUS_CODE);
                          errorHandledKey = Guid.NewGuid();
                          Context.Items.Add(errorHandledKey, true);
                      };

            OnEndRequest = () =>
                           {
                               if (!Context.Items.Contains(errorHandledKey)
                                   && IsAnErrorResponse(Context.Response))
                                   HttpErrorHandler.HandleError(Context, Context.Response.StatusCode);

                               errorHandledKey = Guid.Empty;
                           };
        }

        const int ERROR_STATUS_CODE = 400;
        HttpErrorHandler _httpErrorHandler;

        public HttpErrorHandler HttpErrorHandler
        {
            get
            {
                if (_httpErrorHandler.IsNull())
                    _httpErrorHandler = ServiceLocator.Current.GetInstance<HttpErrorHandler>();
                return _httpErrorHandler;
            }
        }

        static bool IsAnErrorResponse(HttpResponseBase httpResponse)
        {
            return httpResponse.StatusCode >= ERROR_STATUS_CODE;
        }

        protected override void OnDisposing(bool disposing) {}
    }
}