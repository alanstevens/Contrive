using System;
using System.Collections.Generic;

namespace Contrive.Core
{
  public interface IRole
  {
    Guid Id { get; set; }
    string Name { get; set; }
    string Description { get; set; }
    ICollection<IUser> Users { get; set; }
  }
}