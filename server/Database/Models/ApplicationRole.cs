using Microsoft.AspNetCore.Identity;

namespace beautysalon.Database.Models
{
    public class ApplicationRole: IdentityRole<Guid>
    {
        public ApplicationRole () : base() { }
        public ApplicationRole (string roleName) : base()
        {
            Name = roleName;
        }
    }
}
