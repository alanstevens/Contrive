using System.Collections.Generic;

namespace Contrive.Auth.Membership
{
    public interface IRoleExtended : IRole
    {
        ICollection<IUserExtended> Users { get; set; }
    }
}