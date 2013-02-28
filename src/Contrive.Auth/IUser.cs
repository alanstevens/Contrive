﻿using System;

namespace Contrive.Auth
{
    public interface IUser
    {
        Guid Id { get; set; }

        string UserName { get; set; }

        string Email { get; set; }

        string Password { get; set; }

        string PasswordSalt { get; set; }

        string FirstName { get; set; }

        string LastName { get; set; }
    }
}