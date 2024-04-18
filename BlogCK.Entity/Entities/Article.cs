using BlogCK.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCK.Entity.Entities
{
    public class Article : EntityBase
    {
        public Article()
        {
            
        }

        public Article(string title, string content, Guid categoryId, Guid imageId, Guid userId, string createdBy)
        {
            Title = title;
            Content = content;
            CategoryId = categoryId;
            ImageId = imageId;
            UserId = userId;
            CreatedBy = createdBy;
        }  
        
        //constructor yaratmagimizin en esas sebeblerinden biri hansi deyisenleri daxil etmeyimizin mecburi oldugunu bilmek ucundur. Kod oxunaqligini artirir, mes ModifiedBy default olaraq "Unidentified" alir, yeni new Article yaradanda buna deyer vermek mecburi olmaya bilers

        public string Title { get; set; }
        public string Content { get; set; }
        public int ViewCount { get; set; } = 0;

        public Guid CategoryId { get; set; }
        public Category Category { get; set; }

        public Guid? ImageId { get; set; }
        public Image Image { get; set; }

        public Guid UserId { get; set; }
        public AppUser User { get; set; }

        public ICollection<Visitor> ArticleVisitors { get; set; }
    }
}
