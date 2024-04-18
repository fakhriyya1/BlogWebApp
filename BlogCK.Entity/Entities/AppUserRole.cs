using BlogCK.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCK.Entity.Entities
{
    public class AppUserRole:IdentityUserRole<Guid>, IEntityBase
    {
    }
}
