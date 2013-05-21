using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Contrive.Auth
{
    public class Role : IRole
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<IUser> Users { get; set; }
    }
}