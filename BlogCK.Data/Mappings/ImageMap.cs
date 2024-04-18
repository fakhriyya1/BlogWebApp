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
    public class ImageMap : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasData(new Image
            {
                Id= Guid.Parse("A3E3367D-CA0D-4FFE-B859-40B32FEFB9D9"),
                FileName = "images/testimage",
                FileType = "jpg",
                CreatedBy = "Admin Test",
                CreatedDate = DateTime.Now,
                isDeleted = false
            },
            new Image
            {
                Id= Guid.Parse("206AC99A-DD22-40BD-8BB5-C3B501CB3B5E"),
                FileName= "images/vstest",
                FileType= "png",
                CreatedBy = "Admin Test",
                CreatedDate = DateTime.Now,
                isDeleted = false
            });
        }
    }
}
