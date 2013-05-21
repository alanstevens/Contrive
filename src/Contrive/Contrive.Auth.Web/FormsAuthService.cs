using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using Contrive.Common;
using Contrive.Common.Extensions;
using Contrive.Common.Web;
using Newtonsoft.Json;

namespace Contrive.Auth.Web
{
    // Usage:
    //    protected void Application_PostAuthenticateRequest(Object sender, EventArgs args)
    //{
    //    if (!User.Identity.IsAuthenticated) return;
    //
    //    var formsAuthService = ServiceLocator.Current.GetInstance<IFormsAuthService>();
    //
    //    Context.User = formsAuthService.GetUpdatedPrincipalFor(User);
    //}
    public class FormsAuthService : IFormsAuthService
    {
        public FormsAuthService(IUserService userService)
        {
            _userService = userService;
        }

        readonly IUserService _userService;

        static DateTime ExpirationTime
        {
            get
            {
                var authenticationTimeout = Convert.ToInt32(FormsAuthentication.Timeout.TotalMinutes);
                return DateTime.Now.AddMinutes(authenticationTimeout);
            }
        }

        public static HttpContextBase Context { get { return HttpContextProvider.GetContext(); } }

        /// <summary>
        ///     To be called on post authenticate request with Application.User
        /// </summary>
        public IPrincipal GetUpdatedPrincipalFor(IPrincipal principal)
        {
            var genericPrincipal = new GenericPrincipal(new GenericIdentity(""), new string[0]);

            if (!ValidatePrincipal(principal)) return genericPrincipal;

            IUser user = null;
            FormsAuthenticationTicket currentTicket = null;
            var userName = principal.Identity.Name;
            var authCookie = GetCurrentCookie();

            if (ValidateCookie(authCookie))
            {
                var encryptedTicketValue = authCookie.Value;
                currentTicket = GetCurrentTicket(encryptedTicketValue);
                var userData = currentTicket.UserData;

                // When this works, we avoid another hit to the database.
                if (userData.IsNotBlank())
                    user = DeserializeUser(userData, userName);
            }

            if (user.IsNull())
            {
                user = _userService.GetUserByUserName(userName);
                currentTicket = null;
            }

            if (user.IsNull()) return genericPrincipal;

            _userService.CurrentUser = user;

            return GetUpdatedPrincipalFor(user, false, currentTicket);
        }

        public IPrincipal GetUpdatedPrincipalFor(IUser user, bool stayLoggedIn = false, FormsAuthenticationTicket currentTicket = null)
        {
            Verify.NotNull(user, "user");
            Verify.False(user.IsNew, "user.IsNew"); // User must be saved to the DB first.
            var issueDate = DateTime.Now;

            if (currentTicket.IsNotNull())
            {
                issueDate = currentTicket.IssueDate;
                stayLoggedIn = currentTicket.IsPersistent;
            }
            var userData = SerializeUser(user);

            var newTicket = NewTicketFrom(user.UserName, issueDate, ExpirationTime, stayLoggedIn, userData);

            // Because of this, this method must be called after successful authentication
            RenewCookieWith(newTicket);

            IIdentity identity = new FormsIdentity(newTicket);

            var roleNames = user.Roles.Select(r => r.Name).ToArray();

            // TODO: HAS 03/03/2013 Create a custom Principal type to hold custom data
            return new GenericPrincipal(identity, roleNames);
        }

        protected static FormsAuthenticationTicket GetCurrentTicket(string encryptedTicketValue)
        {
            Verify.NotEmpty(encryptedTicketValue, "encryptedTicketValue");
            try
            {
                return FormsAuthentication.Decrypt(encryptedTicketValue);
            }
            catch (HttpException ex)
            {
                throw new HttpException("Failed to decrypt FormsAuthenticationTicket.", ex);
            }
        }

        static FormsAuthenticationTicket NewTicketFrom(string userName, DateTime issueDate, DateTime expires, bool stayLoggedIn, string userData)
        {
            return new FormsAuthenticationTicket(2, userName, issueDate, expires, stayLoggedIn, userData);
        }

        static void RenewCookieWith(FormsAuthenticationTicket ticket)
        {
            Verify.NotNull(ticket, "ticket");
            var cookie = FormsAuthentication.GetAuthCookie(ticket.Name, ticket.IsPersistent);
            cookie.Value = FormsAuthentication.Encrypt(ticket);
            cookie.Expires = ticket.Expiration;

            Context.Response.Cookies.Set(cookie);
            Context.Request.Cookies.Set(cookie);
        }

        static HttpCookie GetCurrentCookie()
        {
            if (!FormsAuthentication.CookiesSupported)
                throw new HttpException("Cookieless Forms Authentication is not supported for this application.");

            var cookieName = FormsAuthentication.FormsCookieName;
            var authCookie = HttpContextProvider.GetContext().Request.Cookies[cookieName];
            return authCookie;
        }

        static bool ValidateCookie(HttpCookie authCookie)
        {
            var invalid = authCookie.IsNull() || authCookie.Value.Blank();
            return !invalid;
        }

        static bool ValidatePrincipal(IPrincipal principal)
        {
            var invalid = principal.IsNull() || !principal.Identity.IsAuthenticated;
            return !invalid;
        }

        protected virtual string SerializeUser(IUser user)
        {
            Verify.NotNull(user, "user");
            return JsonConvert.SerializeObject(user);
        }

        protected virtual IUser DeserializeUser(string userData, string userName)
        {
            Verify.NotEmpty(userData, "userData");
            Verify.NotEmpty(userName, "userName");
            var user = JsonConvert.DeserializeObject<User>(userData);
            return user.UserName.Equals(userName) ? user : null;
        }
    }
}