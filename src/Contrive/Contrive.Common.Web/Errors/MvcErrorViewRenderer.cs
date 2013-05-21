using System;
using System.Web;
using System.Web.Mvc;
using Contrive.Common.Extensions;

namespace Contrive.Common.Web.Errors
{
    // Based on https://github.com/PureKrome/MagicalUnicorn.MvcErrorToolkit
    public class MvcErrorViewRenderer : ErrorViewRendererBase
    {
        public MvcErrorViewRenderer(IWebConfigurationProvider config) : base(config) {}

        protected override void RenderCustomErrorView(HttpRequestBase request,
                                                      HttpResponseBase response,
                                                      string redirectPath,
                                                      Exception currentError)
        {
            try
            {
                RenderMvcErrorView(request, response, redirectPath, currentError);
            }
            catch (Exception exception)
            {
                var errorMessage =
                    "An error occurred while trying to render the custom error for this HttpStatusCode. RedirectPath: {0}; Message: {1}"
                        .FormatWith(redirectPath.Blank() ? "--no redirect path was provided!! --" : redirectPath, exception.Message);

                RenderFallBackErrorView(response, new InvalidOperationException(errorMessage, currentError));
            }
        }

        static void RenderMvcErrorView(HttpRequestBase request,
                                       HttpResponseBase response,
                                       string redirectPath,
                                       Exception currentError)
        {
            var controllerContext = NewControllerContext(request);
            var view = new RazorView(controllerContext, redirectPath, null, false, null);
            var viewModel = new ErrorViewModel {Exception = currentError};
            var viewContext = new ViewContext(controllerContext, view, new ViewDataDictionary(viewModel), new TempDataDictionary(),
                                              response.Output);
            view.Render(viewContext, response.Output);
        }
    }
}