using Microsoft.AspNetCore.Identity;

namespace UserService.DAL.Models
{
    public class User : IdentityUser<Guid>
    {
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresOn { get; set; }
    }
}
