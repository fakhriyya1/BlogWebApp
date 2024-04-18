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
    public class ArticleMap : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.HasData(new Article
            {
                Id = Guid.NewGuid(),
                Title = "Asp.net Core test article",
                Content = "C# test article Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, discovered the undoubtable source. Lorem Ipsum comes from sections 1.10.32 and 1.10.33 of \"de Finibus Bonorum et Malorum\" (The Extremes of Good and Evil) by Cicero, written in 45 BC. This book is a treatise on the theory of ethics, very popular during the Renaissance. The first line of Lorem Ipsum, \"Lorem ipsum dolor sit amet..\", comes from a line in section 1.10.32.",
                ViewCount = 15,
                CategoryId = Guid.Parse("35AFD272-C251-4AF8-AF28-56EB61061770"),
                ImageId = Guid.Parse("A3E3367D-CA0D-4FFE-B859-40B32FEFB9D9"),
                CreatedBy = "Admin test",
                CreatedDate = DateTime.Now,
                isDeleted = false,
                UserId = Guid.Parse("EE3AF905-56E3-4E2A-BA5B-DBDED5FE61B4")
            },
            new Article
            {
                Id = Guid.NewGuid(),
                Title = "Visual studio test article",
                Content = "Visual studio test article Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, discovered the undoubtable source. Lorem Ipsum comes from sections 1.10.32 and 1.10.33 of \"de Finibus Bonorum et Malorum\" (The Extremes of Good and Evil) by Cicero, written in 45 BC. This book is a treatise on the theory of ethics, very popular during the Renaissance. The first line of Lorem Ipsum, \"Lorem ipsum dolor sit amet..\", comes from a line in section 1.10.32.",
                ViewCount = 15,
                CategoryId = Guid.Parse("E029AF58-1C52-484B-B0AA-D963BD51187E"),
                ImageId = Guid.Parse("206AC99A-DD22-40BD-8BB5-C3B501CB3B5E"),
                CreatedBy = "Admin test",
                CreatedDate = DateTime.Now,
                isDeleted = false,
                UserId = Guid.Parse("483C378A-BF29-49E4-A861-D3AB8B828778")
            });
        }
    }
}
