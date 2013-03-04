using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Contrive.Auth.Membership
{
    public class RoleExtended : IRoleExtended
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<IUserExtended> Users { get; set; }

        // TODO: HAS 03/03/2013 Get rid of this member
        ICollection<IUser> IRole.Users { get { return null; } set { } }
    }
}