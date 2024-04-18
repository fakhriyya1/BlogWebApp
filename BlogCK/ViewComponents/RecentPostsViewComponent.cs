using BlogCK.Service.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace BlogCK.Web.ViewComponents
{
    public class RecentPostsViewComponent : ViewComponent
    {
        private readonly IArticleService articleService;

        public RecentPostsViewComponent(IArticleService articleService)
        {
            this.articleService = articleService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var articles = await articleService.Get3RecentArticlesWithCategoryNonDeletedAsync();

            return View(articles);
        }
    }
}
