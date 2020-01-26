using Microsoft.AspNetCore.Identity;

namespace CLM.Models
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public string RoleAssigner { get; set; }
    }
}
