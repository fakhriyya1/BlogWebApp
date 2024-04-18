using BlogCK.Entity.Entities;
using BlogCK.Service.Helpers.Images;
using BlogCK.Service.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using static BlogCK.Web.ResultMessages.Messages;

namespace BlogCK.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin, Superadmin")]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IDashboardService dashboardService;

        public HomeController(UserManager<AppUser> userManager, IDashboardService dashboardService)
        {
            this.userManager = userManager;
            this.dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var loggedInUser= await userManager.GetUserAsync(User);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MontlyArticlesCount()
        {
            var count = await dashboardService.GetMontlyArticleCount();
            return Json(JsonConvert.SerializeObject(count)); 
        }

        [HttpGet]
        public async Task<IActionResult> TotalArticleCount()
        {
            var count = await dashboardService.GetTotalArticleCount();
            return Json(count);
        }
        
        [HttpGet]
        public async Task<IActionResult> TotalCategoryCount()
        {
            var count = await dashboardService.GetTotalCategoryCount();
            return Json(count);
        }
    }
}
