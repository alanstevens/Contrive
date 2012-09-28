using System.Collections.Generic;
using Contrive.Core;

namespace Contrive.Sample.Areas.Contrive.Models
{
  public class ManageUsersViewModel
  {
    public IEnumerable<IUser> Users { get; set; }
  }
}