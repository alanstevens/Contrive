using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using Contrive.Common.Extensions;

namespace Contrive.Web.Common
{
  // Based on https://github.com/PureKrome/MagicalUnicorn.MvcErrorToolkit
  public class HttpErrorHandler
  {
    public HttpErrorHandler(IWebConfigurationProvider configurationProvider)
    {
      _configurationProvider = configurationProvider;
    }

    readonly IWebConfigurationProvider _configurationProvider;

    public void HandleError(HttpContextBase context, int statusCode)
    {
      var currentError = context.Error;

      this.LogException(currentError);

      if (!context.IsCustomErrorEnabled || !UseCustomErrors(context.Request.IsLocal)) return;

      var httpErrorException = currentError as HttpException;
      if (httpErrorException != null) statusCode = httpErrorException.GetHttpCode();

      context.Response.StatusCode = statusCode;

      RenderErrorView(context, statusCode, currentError);

      HttpContext.Current.ClearError();

      context.Response.TrySkipIisCustomErrors = true;
    }

    void RenderErrorView(HttpContextBase context, int statusCode, Exception currentError)
    {
      if (context.Request.IsAjaxRequest()) RenderAjaxView(context, currentError);
      else
      {
        var redirectPath = GetCustomErrorRedirect(statusCode);

        if (redirectPath.Blank()) RenderFallBackErrorView(context, currentError);
        else RenderCustomErrorView(context, redirectPath, currentError);
      }
    }

    static void RenderAjaxView(HttpContextBase context, Exception currentError)
    {
      var errorMessage = context.Request.ContentType.Contains("json")
                           ? currentError.Message
                           : String.Format(
                                           "An error occurred but we are unable to handle the request.ContentType [{0}]. Anyways, the error is: {1}",
                                           context.Request.ContentType, currentError.Message);

      var errorController = new FakeErrorController();
      var controllerContext = new ControllerContext(context.Request.RequestContext, errorController);
      var jsonResult = new JsonResult
      {JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new {error_message = errorMessage}};
      jsonResult.ExecuteResult(controllerContext);
    }

    static void RenderCustomErrorView(HttpContextBase context, string redirectPath, Exception currentError)
    {
      try
      {
        // TODO: HAS 09/28/2012 Check if we are in MVC or WebForms
        RenderMvcErrorView(context, redirectPath, currentError);
      }
      catch (Exception exception)
      {
        var errorMessage =
          "An error occurred while trying to render the custom error for this HttpStatusCode. RedirectPath: {0}; Message: {1}"
            .FormatWith(redirectPath.Blank() ? "--no redirect path was provided!! --" : redirectPath, exception.Message);

        RenderFallBackErrorView(context, new InvalidOperationException(errorMessage, currentError));
      }
    }

    static void RenderMvcErrorView(HttpContextBase context, string redirectPath, Exception currentError)
    {
      var errorController = new FakeErrorController();
      var controllerContext = new ControllerContext(context.Request.RequestContext, errorController);
      var view = new RazorView(controllerContext, redirectPath, null, false, null);
      var viewModel = new ErrorViewModel {Exception = currentError};
      var tempData = new TempDataDictionary();
      var viewContext = new ViewContext(controllerContext, view, new ViewDataDictionary(viewModel), tempData,
                                        context.Response.Output);
      view.Render(viewContext, context.Response.Output);
    }

    static void RenderFallBackErrorView(HttpContextBase context, Exception currentError)
    {
      currentError = currentError.GetBaseException();

      const string simpleErrorMessage =
        "<html><head><title>An error has occurred</title></head><body><h2>Sorry, an error occurred while processing your request.</h2><br/><br/><p><ul><li>Exception: {0}</li><li>Source: {1}</li></ul></p></body></html>";
      context.Response.Output.Write(simpleErrorMessage, currentError.Message, currentError.StackTrace);
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

    string GetCustomErrorRedirect(int statusCode)
    {
      var redirect = _configurationProvider.CustomRedirects[statusCode];

      redirect = redirect.Blank() ? _configurationProvider.DefaultRedirect : redirect;

      if (redirect.Blank()) Debug.Assert(false, "No redirect path configured.");

      return redirect;
    }
  }

  public class ErrorViewModel
  {
    public Exception Exception { get; set; }
  }

  internal class FakeErrorController : Controller {}
}