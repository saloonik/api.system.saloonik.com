using Microsoft.AspNetCore.Identity;
using System.Xml.Linq;

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
