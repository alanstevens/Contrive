using System;
using System.Web;
using Contrive.Common.Extensions;

namespace Contrive.Web.Common.Errors
{
  public class HttpErrorHandler
  {
    public HttpErrorHandler(IWebConfigurationProvider configurationProvider, IErrorView errorView)
    {
      _configurationProvider = configurationProvider;
      _errorView = errorView;
    }

    readonly IWebConfigurationProvider _configurationProvider;
    readonly IErrorView _errorView;

    public void HandleError(HttpRequestBase request,
                            HttpResponseBase response,
                            int statusCode,
                            Exception currentError,
                            bool isCustomErrorEnabled)
    {
      this.LogException(currentError.GetBaseException());
      this.LogException(currentError);

      if (!isCustomErrorEnabled || !UseCustomErrors(request.IsLocal)) return;

      var httpErrorException = currentError as HttpException;
      if (httpErrorException.IsNotNull()) statusCode = httpErrorException.GetHttpCode();

      response.SetStatus(statusCode);

      _errorView.Render(response, request, statusCode, currentError);

      HttpContext.Current.ClearError();

      response.TrySkipIisCustomErrors = true;
    }

    bool UseCustomErrors(bool isLocal)
    {
      switch (_configurationProvider.CustomErrorsMode)
      {
        case "Off":
          return false;
        case "RemoteOnly":
          return !isLocal;
        default:
          return true;
      }
    }
  }
}