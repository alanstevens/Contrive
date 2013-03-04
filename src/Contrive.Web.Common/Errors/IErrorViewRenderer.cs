using System;
using System.Web;

namespace Contrive.Web.Common.Errors
{
    public interface IErrorViewRenderer
    {
        void Render(HttpContextBase context, int statusCode, Exception currentError);
    }
}