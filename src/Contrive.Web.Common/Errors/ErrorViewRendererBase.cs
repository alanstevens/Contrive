using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using Contrive.Common.Extensions;

namespace Contrive.Web.Common.Errors
{
    public abstract class ErrorViewRendererBase : IErrorViewRenderer
    {
        protected ErrorViewRendererBase(IWebConfigurationProvider config)
        {
            _config = config;
        }

        const string SIMPLE_ERROR_MESSAGE =
            "<html><head><title>An error has occurred</title></head><body><h2>Sorry, an error occurred while processing your request.</h2><br/><br/><p><ul><li>Exception: {0}</li><li>Source: {1}</li></ul></p></body></html>";

        readonly IWebConfigurationProvider _config;

        public void Render(HttpContextBase context, int statusCode, Exception currentError)
        {
            if (context.Request.IsAjaxRequest()) RenderAjaxView(context.Request, currentError);
            else
            {
                var redirectPath = GetCustomErrorRedirect(statusCode);

                if (redirectPath.Blank()) RenderFallBackErrorView(context.Response, currentError);
                else RenderCustomErrorView(context.Request, context.Response, redirectPath, currentError);
            }
        }

        protected virtual void RenderAjaxView(HttpRequestBase request, Exception currentError)
        {
            var jsonResult = new JsonResult
                             {
                                 JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                 Data = new {error_message = GetJsonErrorMessage(request, currentError)}
                             };

            jsonResult.ExecuteResult(NewControllerContext(request));
        }

        protected static void RenderFallBackErrorView(HttpResponseBase response, Exception baseException)
        {
            baseException = baseException.GetBaseException();

            response.Output.Write(SIMPLE_ERROR_MESSAGE, baseException.Message, baseException.StackTrace);
        }

        protected abstract void RenderCustomErrorView(HttpRequestBase request,
                                                      HttpResponseBase response,
                                                      string redirectPath,
                                                      Exception currentError);

        string GetCustomErrorRedirect(int statusCode)
        {
            var redirect = _config.CustomRedirects[statusCode];

            redirect = redirect.Blank() ? _config.DefaultRedirect : redirect;

            if (redirect.Blank()) Debug.Assert(false, "No redirect path configured.");

            return redirect;
        }

        protected static string GetJsonErrorMessage(HttpRequestBase request, Exception currentError)
        {
            return request.ContentType.Contains("json")
                       ? currentError.Message
                       : "An error occurred but we are unable to handle the request.ContentType [{0}]. The error is: {1}"
                             .FormatWith(request.ContentType, currentError.Message);
        }

        protected static ControllerContext NewControllerContext(HttpRequestBase request)
        {
            var errorController = new FakeErrorController();
            var controllerContext = new ControllerContext(request.RequestContext, errorController);
            return controllerContext;
        }

        public class ErrorViewModel
        {
            public Exception Exception { get; set; }
        }

        internal class FakeErrorController : Controller {}
    }
}