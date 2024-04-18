using BlogCK.Data.UnitOfWorks;
using BlogCK.Entity.Entities;
using BlogCK.Models;
using BlogCK.Service.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlogCK.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly IArticleService articleService;

        public HomeController(IArticleService articleService)
        {
            //_logger = logger;
            this.articleService = articleService;
        }

        public async Task<IActionResult> Index(Guid? categoryId, int currentPage = 1, int pageSize = 3, bool isAscending = false)
        {
            var articles = await articleService.GetAllArticlesByPaging(categoryId, currentPage, pageSize, isAscending);
            return View(articles);
        }

        public async Task<IActionResult> Search(string keyword, int currentPage = 1, int pageSize = 3, bool isAscending = false)
        {
            var articles = await articleService.SearchAsync(keyword, currentPage, pageSize, isAscending);
            return View(articles);
        }

        public async Task<IActionResult> Detail(Guid articleId)
        {
            var result=await articleService.TriggerViewCount(articleId);

            return View(result);
        }



        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}