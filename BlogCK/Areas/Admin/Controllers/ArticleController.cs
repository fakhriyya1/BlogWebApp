using AutoMapper;
using BlogCK.Entity.DTOs.Articles;
using BlogCK.Entity.Entities;
using BlogCK.Service.Extensions;
using BlogCK.Service.Services.Abstractions;
using BlogCK.Web.Consts;
using BlogCK.Web.ResultMessages;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace BlogCK.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticleController : Controller
    {
        private readonly IArticleService articleService;
        private readonly ICategoryService categoryService;
        private readonly IMapper mapper;
        private readonly IValidator<Article> validator;
        private readonly IToastNotification toastNotification;

        public ArticleController(IArticleService articleService, ICategoryService categoryService, IMapper mapper, IValidator<Article> validator, IToastNotification toastNotification)
        {
            this.articleService = articleService;
            this.categoryService = categoryService;
            this.mapper = mapper;
            this.validator = validator;
            this.toastNotification = toastNotification;
        }

        [Authorize(Roles = $"{Roles.Superadmin}, {Roles.Admin}, {Roles.User}")]
        public async Task<IActionResult> Index()
        {
            var articles = await articleService.GetAllArticlesWithCategoryNonDeletedAsync();
            return View(articles);
        }

        [HttpGet]
        [Authorize(Roles = $"{Roles.Superadmin}, {Roles.Admin}")]
        public async Task<IActionResult> DeletedArticles()
        {
            var articles = await articleService.GetAllArticlesWithCategoryDeletedAsync();
            return View(articles);
        }

        #region Add

        [HttpGet]
        [Authorize(Roles = $"{Roles.Superadmin}, {Roles.Admin}")]
        public async Task<IActionResult> Add()
        {
            var categories = await categoryService.GetAllCategoriesNonDeletedAsync();
            return View(new ArticleAddDto { Categories = categories });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Superadmin}, {Roles.Admin}")]
        public async Task<IActionResult> Add(ArticleAddDto articleAddDto)
        {
            var map = mapper.Map<Article>(articleAddDto);
            var result = await validator.ValidateAsync(map);

            if (articleAddDto.Photo == null)
            {
                ModelState.AddModelError("Photo", "Please provide a photo.");
            }

            if (ModelState.IsValid)
            {
                await articleService.CreateArticleAsync(articleAddDto);   //kiminse catIdni inspectden qurdalayib sile bileceyini nezere almamisan!! Qalan metodlarinda da hemcinin
                toastNotification.AddSuccessToastMessage(Messages.Article.Add(articleAddDto.Title));
                return RedirectToAction("Index", "Article", new { Area = "Admin" });
            }
            else
            {
                result.AddToModelStateLoop(this.ModelState);
            }


            var categories = await categoryService.GetAllCategoriesNonDeletedAsync();
            return View(new ArticleAddDto { Categories = categories });

            //burada niye update-deki kimi maplemedik deye agilma gelse ==> cunki burada update-den ferqli olaraq view-da sadece olaraq kateqorileri gormeliyik.

        }

        #endregion

        #region Update

        [HttpGet]
        [Authorize(Roles = $"{Roles.Superadmin}, {Roles.Admin}")]
        public async Task<IActionResult> Update(Guid articleId)
        {
            var entityexists=await articleService.DoesEntityExist(articleId);

            if (!entityexists)
            {
                return NotFound();
            }

            var article = await articleService.GetArticleWithCategoryNonDeletedAsync(articleId);
            var categories = await categoryService.GetAllCategoriesNonDeletedAsync();

            var articleUpdateDto = mapper.Map<ArticleUpdateDto>(article);
            articleUpdateDto.Categories = categories;

            return View(articleUpdateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Superadmin}, {Roles.Admin}")]
        public async Task<IActionResult> Update(ArticleUpdateDto articleUpdateDto)
        {
            var map = mapper.Map<Article>(articleUpdateDto);
            var result = await validator.ValidateAsync(map);
            var categories = await categoryService.GetAllCategoriesNonDeletedAsync();
            articleUpdateDto.Categories = categories;

            if (result.IsValid)
            {
                string title = await articleService.UpdateArticleAsync(articleUpdateDto);
                toastNotification.AddSuccessToastMessage(Messages.Article.Update(title));
                return RedirectToAction("Index", "Article", new { Area = "Admin" });
            }
            else
            {
                result.AddToModelStateLoop(this.ModelState);
                return View(articleUpdateDto);
            }

        }

        #endregion

        [Authorize(Roles = $"{Roles.Superadmin}, {Roles.Admin}")]
        public async Task<IActionResult> Delete(Guid articleId)
        {
            var entityexists = await articleService.DoesEntityExist(articleId);

            if (!entityexists)
            {
                return NotFound();
            }

            await articleService.SafeDeleteArticleAsync(articleId);

            return RedirectToAction("Index", "Article", new { Area = "Admin" });
        }

        [Authorize(Roles = $"{Roles.Superadmin}")]
        public async Task<IActionResult> UndoDelete(Guid articleId)
        {
            var entityexists = await articleService.DoesEntityExist(articleId);

            if (!entityexists)
            {
                return NotFound();
            }

            await articleService.UndoDeleteArticleAsync(articleId);

            return RedirectToAction("Index", "Article", new { Area = "Admin" });
        }
    }
}
