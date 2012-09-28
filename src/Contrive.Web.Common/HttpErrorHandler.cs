using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using Contrive.Common.Extensions;
using Contrive.Common.Web;

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

    public void HandleError(HttpContextBase context, int statusCode = 400)
    {
      // Lets remember the current error.
      var currentError = context.Error;

      this.LogException(currentError);

      // Do not show the custom errors if
      // a) CustomErrors mode == "off" or not set.
      // b) Mode == RemoteOnly and we are on our local development machine.
      if (!context.IsCustomErrorEnabled || !UseCustomErrors(context))
      {
        // Damn it :( Fine.... lets just bounce outta-here!
        return;
      }

      // Do we have an HttpErrorException? Eg. 404 Not found or 401 Not Authorized?
      var httpErrorException = currentError as HttpException;
      if (httpErrorException != null) statusCode = httpErrorException.GetHttpCode();

      // Lets make sure we set the correct Error Status code :)
      context.Response.StatusCode = statusCode;

      // Render the view, be it html or json, etc.
      RenderErrorView(context, statusCode, currentError);

      // Lets clear all the errors otherwise it shows the error page.
      HttpContext.Current.ClearError();

      // Avoid any IIS low level errors.
      context.Response.TrySkipIisCustomErrors = true;
    }

    void RenderErrorView(HttpContextBase context, int statusCode, Exception currentError)
    {
      // Is this an AJAX request?
      if (context.Request.IsAjaxRequest()) RenderAjaxView(context, currentError);
      else
      {
        // What is the view we require?
        var redirectPath = GetCustomErrorRedirect(statusCode);

        // Now - what do we render? The view provided or some basic content because one HASN'T been provided?
        if (String.IsNullOrEmpty(redirectPath)) RenderFallBackErrorView(context, currentError);
        else RenderCustomErrorView(context, redirectPath, currentError);
      }
    }

    void RenderAjaxView(HttpContextBase context, Exception currentError)
    {
      // Ok. lets check if this content type contains a request for json.
      var errorMessage = context.Request.ContentType.Contains("json")
                           ? currentError.Message
                           : String.Format(
                                           "An error occurred but we are unable to handle the request.ContentType [{0}]. Anyways, the error is: {1}",
                                           context.Request.ContentType, currentError.Message);

      var errorController = new FakeErrorController();
      var controllerContext = new ControllerContext(context.Request.RequestContext, errorController);
      var jsonResult = new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { error_message = errorMessage } };
      jsonResult.ExecuteResult(controllerContext);
    }

    void RenderCustomErrorView(HttpContextBase context, string viewPath, Exception currentError)
    {
      try
      {
        // We need to render the view now.
        // This means we need a viewContext ... which requires a controller.
        // So we instantiate a fake controller which does nothing
        // and then work our way to rendering the view.
        var errorController = new FakeErrorController();
        var controllerContext = new ControllerContext(context.Request.RequestContext, errorController);
        var view = new RazorView(controllerContext, viewPath, null, false, null);
        var viewModel = new ErrorViewModel { Exception = currentError };
        var tempData = new TempDataDictionary();
        var viewContext = new ViewContext(controllerContext, view, new ViewDataDictionary(viewModel), tempData,
                                          context.Response.Output);
        view.Render(viewContext, context.Response.Output);
      }
      catch (Exception exception)
      {
        // Damn it! Something -really- crap just happened. 
        // eg. the path to the redirect might not exist, etc.
        var errorMessage =
          String.Format(
                        "An error occurred while trying to Render the custom error view which you provided, for this HttpStatusCode. ViewPath: {0}; Message: {1}",
                        String.IsNullOrEmpty(viewPath) ? "--no view path was provided!! --" : viewPath,
                        exception.Message);

        RenderFallBackErrorView(context, new InvalidOperationException(errorMessage, currentError));
      }
    }

    void RenderFallBackErrorView(HttpContextBase context, Exception currentError)
    {
      currentError = currentError.GetBaseException();

      const string simpleErrorMessage =
        "<html><head><title>An error has occurred</title></head><body><h2>Sorry, an error occurred while processing your request.</h2><br/><br/><p><ul><li>Exception: {0}</li><li>Source: {1}</li></ul></p></body></html>";
      context.Response.Output.Write(simpleErrorMessage, currentError.Message, currentError.StackTrace);
    }

    bool UseCustomErrors(HttpContextBase context)
    {
      if (context.IsNull()) return false;

      switch (_configurationProvider.CustomErrorsMode)
      {
        case "Off":
          return false;
        case "RemoteOnly":
          return !context.Request.IsLocal;
        default:
          return true;
      }
    }

    string GetCustomErrorRedirect(int statusCode)
    {
      var redirect = _configurationProvider.CustomRedirects[statusCode];

      redirect = String.IsNullOrEmpty(redirect) ? _configurationProvider.DefaultRedirect : redirect;

      if (redirect.Blank()) Debug.Assert(false, "No redirect path was determined.");

      return redirect;
    }
  }

  public interface IErrorViewModel
  {
    Exception Exception { get; set; }
  }

  public class ErrorViewModel
  {
    public Exception Exception { get; set; }
  }

  internal class FakeErrorController : Controller { }
}