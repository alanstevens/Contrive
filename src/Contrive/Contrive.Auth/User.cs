using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Contrive.Auth
{
    public class User : IUser
    {
        public User()
        {
            Roles = new IRole[0];
        }

        [Key]
        public Int32 Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }

        public string PasswordSalt { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ICollection<IRole> Roles { get; set; }

        public bool IsNew { get { return Id == 0; } }
    }
}