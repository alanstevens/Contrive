using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using Contrive.Common;
using Contrive.Common.Extensions;
using Contrive.Web.Common;

namespace Contrive.Auth.Web
{
    // protected void Application_AuthenticateRequest(Object sender, EventArgs e)
    // {
    //     ServiceLocator.Current.GetInstance<IUserService>().UpdateCurrentUserWith(User);
    //     If(User.Identity.IsAuthenticated) RedirectToLogin();
    // }
    public class FormsAuthService : IFormsAuthService
    {
        public FormsAuthService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        const string CURRENT_USER_KEY = "CurrentUser";

        readonly IRoleRepository _roleRepository;

        static HttpContextBase Context { get { return new HttpContextWrapper(HttpContext.Current); } }

        static DateTime ExpirationTime
        {
            get
            {
                var authenticationTimeout = Convert.ToInt32(FormsAuthentication.Timeout.TotalMinutes);
                return DateTime.Now.AddMinutes(authenticationTimeout);
            }
        }

        public IUser CurrentUser { get { return Context.Items[CURRENT_USER_KEY].As<IUser>(); } private set { Context.Items[CURRENT_USER_KEY] = value; } }

        /// <summary>
        ///     To be called on authenticate request with Application.User
        /// </summary>
        public void UpdateCurrentUserWith(IPrincipal principal)
        {
            var authCookie = GetCurrentCookie();

            if (principal.IsNull() || !principal.Identity.IsAuthenticated || authCookie.IsNull() || authCookie.Value.Blank())
            {
                UpdateCurrentUserWith(new User());
                return;
            }

            var currentTicket = GetCurrentTicket(authCookie.Value);
            IEnumerable<string> roleNames;
            var user = ParseUserData(currentTicket.UserData, out roleNames);
            UpdateCurrentUserWith(user, currentTicket.IsPersistent, roleNames);
        }

        public void UpdateCurrentUserWith(IUser user, bool stayLoggedIn = false, IEnumerable<string> roleNames = null)
        {
            Verify.NotNull(user, "user");
            var authCookie = GetCurrentCookie();

            FormsAuthenticationTicket currentTicket;
            if (authCookie.IsNull() || authCookie.Value.Blank()) currentTicket = NewTicket(ExpirationTime, stayLoggedIn);
            else currentTicket = GetCurrentTicket(authCookie.Value);

            var userData = FormatUserDataFor(user);
            currentTicket = RenewTicket(user, currentTicket, userData, ExpirationTime);

            var cookie = RenewCookie(currentTicket);

            Context.Response.Cookies.Set(cookie);
            Context.Request.Cookies.Set(cookie);

            if (roleNames.IsNull() || roleNames.IsEmpty())
            {
                if (!user.IsNew && user.Roles.Any())
                    roleNames = user.Roles.Select(r => r.Name);
                else roleNames = new string[] { };
            }
            else
            {
                if (!user.IsNew && !user.Roles.Any())
                    user.Roles = _roleRepository.GetRolesForRoleNames(roleNames).ToList();
            }

            var identity = new FormsIdentity(currentTicket);

            // TODO: HAS 03/03/2013 Create a custom Principal type to hold custom data
            Context.User = new GenericPrincipal(identity, roleNames.ToArray());

            CurrentUser = user;
        }

        protected static FormsAuthenticationTicket GetCurrentTicket(string cookieValue)
        {
            Verify.NotEmpty(cookieValue, "cookieValue");
            return FormsAuthentication.Decrypt(cookieValue);
        }

        static HttpCookie GetCurrentCookie()
        {
            var cookieName = FormsAuthentication.FormsCookieName;
            var authCookie = Context.Request.Cookies[cookieName];
            return authCookie;
        }

        static FormsAuthenticationTicket NewTicket(DateTime expires, bool stayLoggedIn)
        {
            return new FormsAuthenticationTicket(2, "", DateTime.Now, expires, stayLoggedIn, "");
        }

        FormsAuthenticationTicket RenewTicket(IUser user, FormsAuthenticationTicket ticket, string userData, DateTime expirationTime)
        {
            return new FormsAuthenticationTicket(ticket.Version,
                                                 user.UserName,
                                                 ticket.IssueDate,
                                                 expirationTime,
                                                 ticket.IsPersistent,
                                                 userData);
        }

        static HttpCookie RenewCookie(FormsAuthenticationTicket ticket)
        {
            var cookie = FormsAuthentication.GetAuthCookie(ticket.Name, false);
            cookie.Value = FormsAuthentication.Encrypt(ticket);
            cookie.Expires = ticket.Expiration;
            return cookie;
        }

        /// <summary>
        ///     Override this method to store custom information in the ticket.
        /// </summary>
        protected virtual string FormatUserDataFor(IUser user)
        {
            if (user.IsNull()) return "";
            var roleNames = user.Roles.Select(r => r.Name).Join("|");

            return "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}"
                .FormatWith(user.Id, user.UserName, user.Email, user.Password, user.PasswordSalt, user.FirstName, user.LastName, roleNames);
        }

        /// <summary>
        ///     Override this method to retrieve custom data from the ticket.
        /// </summary>
        protected virtual IUser ParseUserData(string userData, out IEnumerable<string> roleNames)
        {
            roleNames = null;
            if (userData.Blank()) return new User();

            var propertyValues = userData.Split(new[] { ',' });

            roleNames = propertyValues[7].Split(new[] { '|' });

            return new User
                   {
                       Id = Guid.Parse(propertyValues[0].Trim()),
                       UserName = propertyValues[1].Trim(),
                       Email = propertyValues[2].Trim(),
                       Password = propertyValues[3].Trim(),
                       PasswordSalt = propertyValues[4].Trim(),
                       FirstName = propertyValues[5].Trim(),
                       LastName = propertyValues[6].Trim()
                   };
        }
    }
}