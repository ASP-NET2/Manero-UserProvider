using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Manero_UserProvider.Models;

public class ProfileModel
{
    public int AccountId { get; set; }
    [ProtectedPersonalData]
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = null!;

    [ProtectedPersonalData]
    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = null!;

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }
    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    public string? IdentityUserId { get; set; }
}
