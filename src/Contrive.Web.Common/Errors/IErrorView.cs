using System;
using System.Web;

namespace Contrive.Web.Common.Errors
{
  public interface IErrorView
  {
    void Render(HttpResponseBase response, HttpRequestBase request, int statusCode, Exception currentError);
  }
}