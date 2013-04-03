using System.Collections.Generic;

namespace Contrive.Auth.Membership
{
    public interface IRoleExtended : IRole
    {
        new ICollection<IUserExtended> Users { get; set; }
    }
}