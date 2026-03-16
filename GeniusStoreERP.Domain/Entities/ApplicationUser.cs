using Microsoft.AspNetCore.Identity;

namespace GeniusStoreERP.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
