using System;
using System.Collections.Generic;

namespace Contrive.Auth
{
  public interface IRole
  {
    Guid Id { get; set; }

    string Name { get; set; }

    string Description { get; set; }

    ICollection<IUser> Users { get; set; }
  }
}