using System.Collections.Generic;
using System.Web.Mvc;

namespace Contrive.Auth.Web.Mvc.Areas.Contrive.Models
{
  public class ManageRolesViewModel
  {
    public SelectList Roles { get; set; }
    public IEnumerable<IRole> RoleList { get; set; }
  }
}