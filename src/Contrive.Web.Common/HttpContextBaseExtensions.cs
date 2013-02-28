using System.Net;
using System.Web;

namespace Contrive.Web.Common
{
  public static class HttpContextBaseExtensions
  {
    public static void SetStatus(this HttpResponseBase response, HttpStatusCode httpStatusCode)
    {
      SetStatus(response, (int) httpStatusCode);
    }

    public static void SetStatus(this HttpResponseBase response, int httpStatusCode)
    {
      response.StatusCode = httpStatusCode;
    }

    public static void SetStatusAndReturn(this HttpResponseBase response, HttpStatusCode httpStatusCode)
    {
      SetStatus(response, (int) httpStatusCode);
      response.End();
    }
  }
}