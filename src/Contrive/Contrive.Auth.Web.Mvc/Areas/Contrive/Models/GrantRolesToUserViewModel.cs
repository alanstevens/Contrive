using System.Web.Mvc;
using System.Web.Security;

namespace Contrive.Auth.Web.Mvc.Areas.Contrive.Models
{
    public class GrantRolesToUserViewModel
    {
        public MembershipUser User { get; set; }
        public string UserName { get; set; }
        public SelectList AvailableRoles { get; set; }
        public SelectList GrantedRoles { get; set; }
    }
}