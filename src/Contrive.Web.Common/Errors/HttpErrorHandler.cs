using System.Diagnostics;
using System.Web;
using Contrive.Common.Extensions;

namespace Contrive.Web.Common.Errors
{
    // Based on https://github.com/PureKrome/MagicalUnicorn.MvcErrorToolkit
    public class HttpErrorHandler
    {
        public HttpErrorHandler(IWebConfigurationProvider configurationProvider, IErrorViewRenderer errorViewRenderer)
        {
            _configurationProvider = configurationProvider;
            _errorViewRenderer = errorViewRenderer;
        }

        readonly IWebConfigurationProvider _configurationProvider;
        readonly IErrorViewRenderer _errorViewRenderer;

        public void HandleError(HttpContextBase context, int statusCode)
        {
            var currentError = context.Error;

            if (currentError.IsNull()) return;

#if DEBUG
            Debugger.Break();
#endif

            this.LogException(currentError);

            if (!context.IsCustomErrorEnabled || !UseCustomErrors(context.Request.IsLocal)) return;

            var httpException = currentError as HttpException;
            if (httpException != null) statusCode = httpException.GetHttpCode();

            context.Response.SetStatus(statusCode);

            _errorViewRenderer.Render(context, statusCode, currentError);

            context.ClearError();

            context.Response.TrySkipIisCustomErrors = true;
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