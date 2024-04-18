using BlogCK.Service.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace BlogCK.Web.ViewComponents
{
    public class HomeCategoriesViewComponent : ViewComponent
    {
        private readonly ICategoryService categoryService;

        public HomeCategoriesViewComponent(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await categoryService.GetAllCategoriesNonDeletedAsyncTake10();

            return View(categories);
        }
    }
}
