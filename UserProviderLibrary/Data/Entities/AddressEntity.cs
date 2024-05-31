using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserProviderLibrary.Data.Entities;

public class AddressEntity
{
    [Key]
    public int AddressId { get; set; }
    public string? AddressTitle { get; set; }
    public string? AddressLine_1 { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }

    [ForeignKey("AccountId")]
    public int AccountId { get; set; }

    public AccountUserEntity? AccountUser { get; set; }
}
