using BlogCK.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCK.Entity.Entities
{
    public class AppUser : IdentityUser<Guid>, IEntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Guid ImageId { get; set; } = Guid.Parse("d6925294-1c29-4fa8-96ee-1a45e155d30f");
        public Image Image { get; set; }

        public ICollection<Article> Articles { get; set; }
    }
}
