using System.Collections.Generic;
using System.Web.Mvc;
using Contrive.Core;

namespace Contrive.Sample.Areas.Contrive.Models
{
    public class ManageRolesViewModel
    {
        public SelectList Roles { get; set; }
        public IEnumerable<IRole> RoleList { get; set; }
    }
}
