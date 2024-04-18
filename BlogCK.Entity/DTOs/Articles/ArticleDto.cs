using BlogCK.Entity.DTOs.Categories;
using BlogCK.Entity.Entities;

namespace BlogCK.Entity.DTOs.Articles
{
    public class ArticleDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Image Image { get; set; }
        public AppUser User { get; set; }
        public CategoryDto Category { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public bool isDeleted { get; set; }
        public int ViewCount { get; set; }

        //Bunlari web yox bu katmana elave elemeyimizin sebebi:
        //Reference ile baglidi
    }
}
