using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using Contrive.Auth;
using Contrive.Common.Extensions;

namespace Contrive.Web.Common
{
    public class UserService : IUserService
    {
        static HttpRequestBase Request { get { return new HttpRequestWrapper(HttpContext.Current.Request); } }
        static HttpResponseBase Response { get { return new HttpResponseWrapper(HttpContext.Current.Response); } }
        static HttpContextBase Context { get { return new HttpContextWrapper(HttpContext.Current); } }

        static DateTime ExpirationTime
        {
            get
            {
                var authenticationTimeout = Convert.ToInt32(FormsAuthentication.Timeout.TotalMinutes);
                return DateTime.Now.AddMinutes(authenticationTimeout);
            }
        }

        static HttpCookie CurrentCookie
        {
            get
            {
                var cookieName = FormsAuthentication.FormsCookieName;
                if (Request.Cookies[cookieName].IsNull())
                    Request.Cookies.Set(FormsAuthentication.GetAuthCookie("", false));
                return Request.Cookies[cookieName];
            }
        }

        protected static FormsAuthenticationTicket CurrentTicket
        {
            get
            {
                var currentCookie = CurrentCookie;
                var cookieValue = currentCookie.Value;
                if (!String.IsNullOrEmpty(cookieValue)) return FormsAuthentication.Decrypt(cookieValue);
                return new FormsAuthenticationTicket(2, "", DateTime.Now, currentCookie.Expires, false, "");
            }
        }

        public IUser CurrentUser { get; private set; }

        public void UpdateCurrentUserWith(IPrincipal principal)
        {
            IUser user = new User();

            if (principal.IsNotNull() && principal.Identity.IsAuthenticated)
                user = RetrieveUserFor(principal.Identity);

            UpdateCurrentUserWith(user);
        }

        public void UpdateCurrentUserWith(IUser user, Dictionary<string, string> userCapabilities = null)
        {
            RenewUser(user);

            CurrentUser = user;
        }

        static void RenewUser(IUser user)
        {
            var userData = FormatUserDataFrom(user);

            var ticket = RenewTicket(user.UserName, userData, CurrentTicket);

            RenewCookie(ticket);

            var identity = ticket.Name == ""
                               ? (IIdentity) new GenericIdentity("")
                               : new FormsIdentity(ticket);

            Context.User = new GenericPrincipal(identity, new string[0]);
        }

        static IUser RetrieveUserFor(IIdentity identity)
        {
            var userData = !identity.IsAuthenticated ? "" : CurrentTicket.UserData;
            var user = ParseUserData(userData, identity.Name);
            return user;
        }

        static FormsAuthenticationTicket RenewTicket(string userName, string userData, FormsAuthenticationTicket ticket)
        {
            var newTicket = new FormsAuthenticationTicket(ticket.Version,
                                                          userName,
                                                          ticket.IssueDate,
                                                          ExpirationTime,
                                                          ticket.IsPersistent,
                                                          userData);
            RenewCookie(newTicket);

            return newTicket;
        }

        static void RenewCookie(FormsAuthenticationTicket ticket)
        {
            HttpCookie cookie = FormsAuthentication.GetAuthCookie(ticket.Name, false);
            cookie.Value = FormsAuthentication.Encrypt(ticket);
            cookie.Expires = ticket.Expiration;

            Response.Cookies.Set(cookie);
            Request.Cookies.Set(cookie);
        }

        static string FormatUserDataFrom(IUser user)
        {
            return user.Id + "," + user.PasswordSalt;
        }

        static IUser ParseUserData(string userData, string userName)
        {
            var userId = Guid.Empty;
            var passwordHash = "";

            if (!String.IsNullOrEmpty(userData))
            {
                var propertyValues = userData.Split(new[] {','});

                userId = Guid.Parse(propertyValues[0].Trim());
                passwordHash = propertyValues[1].Trim();
            }

            return new User{Id =userId, UserName = userName, Password = passwordHash};
        }
    }
}