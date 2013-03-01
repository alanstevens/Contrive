using System;
using System.Collections.Generic;

namespace Contrive.Auth.Membership
{
    public interface IRole
    {
        Guid Id { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        ICollection<IUserExtended> Users { get; set; }
    }
}