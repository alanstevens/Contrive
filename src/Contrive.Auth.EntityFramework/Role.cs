using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Contrive.Auth.Membership;

namespace Contrive.Auth.EntityFramework
{
  public class Role : IRole
  {
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(250)]
    public string Description { get; set; }

    public ICollection<IUserExtended> Users { get; set; }
  }
}