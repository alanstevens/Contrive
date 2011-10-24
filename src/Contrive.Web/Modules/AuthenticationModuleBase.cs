using System;
using System.Web;
using Contrive.Core;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Web.Modules
{
  public abstract class AuthenticationModuleBase : IHttpModule
  {
    protected AuthenticationModuleBase()
    {
      _users = ServiceLocator.Current.GetInstance<IUserProvider>();
      _config = ServiceLocator.Current.GetInstance<IConfigurationProvider>();
    }

    const int ACCESS_DENIED_STATUS_CODE = 401;
    const string ACCESS_DENIED_STATUS_TEXT = "Access Denied";
    protected const string CHALLENGE_HEADER_NAME = "WWW-Authenticate";
    public const string RESPONSE_HEADER_NAME = "Authorization";
    protected readonly IConfigurationProvider _config;

    protected readonly IUserProvider _users;
    protected string _realm;

    /// <summary>
    ///   Inits the specified module.
    /// </summary>
    /// <param name = "application">The HTTP application containing the module.</param>
    public virtual void Init(HttpApplication application)
    {
      // Attach event handlers
      application.AuthenticateRequest += ApplicationAuthenticateRequest;
      application.EndRequest += ApplicationEndRequest;
    }

    protected static void AccessDenied(HttpApplication app)
    {
      app.Response.StatusCode = ACCESS_DENIED_STATUS_CODE;
      app.Response.StatusDescription = ACCESS_DENIED_STATUS_TEXT;
      app.Response.Write(ACCESS_DENIED_STATUS_TEXT);
      app.CompleteRequest();
    }

    /// <summary>
    ///   Handles the AuthenticateRequest event of the application control.
    /// </summary>
    /// <param name = "sender">The source of the event.</param>
    /// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
    /// <remarks>
    ///   The main work is done here. We parse incoming headers, get user name and password from them.
    ///   Then we verify the user name and password against configured membership provider.
    /// </remarks>
    protected void ApplicationAuthenticateRequest(object sender, EventArgs e)
    {
      var app = sender as HttpApplication;
      if (app == null) return;

      if (!Authenticate(app)) AccessDenied(app);
    }

    protected abstract bool Authenticate(HttpApplication app);

    /// <summary>
    ///   Handles the EndRequest event of the application control.
    /// </summary>
    /// <param name = "sender">The source of the event.</param>
    /// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
    /// <remarks>
    ///   This methods adds the "WWW-Authenticate" challenge header to all requests ending with the
    ///   HTTP status code 401 (Access Denied), set either in this module's application_AuthenticateRequest
    ///   or in any other place in application (most likely the authorization module).
    /// </remarks>
    protected virtual void ApplicationEndRequest(object sender, EventArgs e)
    {
      var app = sender as HttpApplication;
      if (app == null) return;

      if (app.Response.StatusCode != ACCESS_DENIED_STATUS_CODE) return;

      app.Response.AppendHeader(CHALLENGE_HEADER_NAME, BuildChallengeHeader(app));
    }

    protected abstract string BuildChallengeHeader(HttpApplication app);

    public virtual void Dispose() { }
  }
}