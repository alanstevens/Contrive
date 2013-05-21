using System;
using System.Collections.Generic;

namespace Contrive.Auth
{
    public interface IUser
    {
        int Id { get; set; }

        string UserName { get; set; }

        string Email { get; set; }

        string PasswordHash { get; set; }

        string PasswordSalt { get; set; }

        string FirstName { get; set; }

        string LastName { get; set; }

        ICollection<IRole> Roles { get; set; }

        bool IsNew { get; }
    }
}