using Microsoft.AspNetCore.Identity;

namespace LegalEagles.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; } = "";
    }
}
