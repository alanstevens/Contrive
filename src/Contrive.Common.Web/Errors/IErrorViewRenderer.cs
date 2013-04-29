using System;
using System.Web;

namespace Contrive.Common.Web.Errors
{
    public interface IErrorViewRenderer
    {
        void Render(HttpContextBase context, int statusCode, Exception currentError);
    }
}