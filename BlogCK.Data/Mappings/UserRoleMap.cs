using BlogCK.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCK.Data.Mappings
{
    public class UserRoleMap : IEntityTypeConfiguration<AppUserRole>
    {
        public void Configure(EntityTypeBuilder<AppUserRole> builder)
        {
            // Primary key
            builder.HasKey(r => new { r.UserId, r.RoleId });

            // Maps to the AspNetUserRoles table
            builder.ToTable("AspNetUserRoles");

            builder.HasData(new AppUserRole
            {
                UserId = Guid.Parse("483C378A-BF29-49E4-A861-D3AB8B828778"),
                RoleId = Guid.Parse("48F1BA93-34AC-4B09-A628-C57FFA2B8BD4")
            },
            new AppUserRole
            {
                UserId = Guid.Parse("EE3AF905-56E3-4E2A-BA5B-DBDED5FE61B4"),
                RoleId = Guid.Parse("2425A9EC-DAD5-4C2B-A049-9141E5AF2E0D")
            });
        }
    }
}
