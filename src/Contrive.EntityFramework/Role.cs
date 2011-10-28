using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Contrive.Core;

namespace Contrive.EntityFramework
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

    public ICollection<IUser> Users { get; set; }
  }
}