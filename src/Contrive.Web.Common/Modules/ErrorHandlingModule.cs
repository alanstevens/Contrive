using System;
using System.Globalization;
using System.Web;
using Contrive.Common;
using Contrive.Common.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Web.Common.Modules
{
  public sealed class ErrorHandlingModule : DisposableBase, IHttpModule
  {
    public ErrorHandlingModule()
    {
      _httpErrorHandler = ServiceLocator.Current.GetInstance<HttpErrorHandler>();
    }

    readonly HttpErrorHandler _httpErrorHandler;

    public void Init(HttpApplication application)
    {
      var context = new HttpContextWrapper(application.Context);
      var errorHandledKey = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture).CalculateSha512Hash();

      application.Error += (s, e) =>
                           {
                             _httpErrorHandler.HandleError(context, 400);

                             context.Items.Add(errorHandledKey, true);
                           };

      application.EndRequest +=
        (s, e) => { if (!context.Items.Contains(errorHandledKey) && IsAnErrorResponse(context.Response)) _httpErrorHandler.HandleError(context, context.Response.StatusCode); };
    }

    static bool IsAnErrorResponse(HttpResponseBase httpResponse)
    {
      return httpResponse.StatusCode >= 400;
    }

    protected override void OnDisposing(bool disposing) {}
  }
}