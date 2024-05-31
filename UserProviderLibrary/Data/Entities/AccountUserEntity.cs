using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserProviderLibrary.Data.Entities;

public class AccountUserEntity
{
    [Key]
    public int AccountId { get; set; }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string? Location { get; set; }
    public string? PhoneNumber { get; set; }

    [ForeignKey("IdentityUserId")]
    public string? IdentityUserId { get; set; }

    public ICollection<AddressEntity> Addresses { get; set; } = new List<AddressEntity>();
}
