using Microsoft.AspNetCore.Identity;
namespace Olympus.Data.Identity;
public class ApplicationUser : IdentityUser
{
    public string DisplayName {get;set;} = string.Empty;
}