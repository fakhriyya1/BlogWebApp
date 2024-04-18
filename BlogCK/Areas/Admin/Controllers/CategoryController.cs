using AutoMapper;
using BlogCK.Data.UnitOfWorks;
using BlogCK.Entity.DTOs.Categories;
using BlogCK.Entity.Entities;
using BlogCK.Service.Extensions;
using BlogCK.Service.Services.Abstractions;
using BlogCK.Service.Services.Concrete;
using BlogCK.Web.Consts;
using BlogCK.Web.ResultMessages;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace BlogCK.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly IValidator<Category> validator;
        private readonly IMapper mapper;
        private readonly IToastNotification toast;

        public CategoryController(ICategoryService categoryService, IValidator<Category> validator, IMapper mapper, IToastNotification toast)
        {
            this.categoryService = categoryService;
            this.validator = validator;
            this.mapper = mapper;
            this.toast = toast;
        }

        [Authorize(Roles = $"{Roles.Superadmin}, {Roles.Admin}, {Roles.User}")]
        public async Task<IActionResult> Index()
        {
            var categories = await categoryService.GetAllCategoriesNonDeletedAsync();
            return View(categories);
        }

        [Authorize(Roles = $"{Roles.Superadmin}, {Roles.Admin}")]
        public async Task<IActionResult> DeletedCategories()
        {
            var categories = await categoryService.GetAllCategoriesDeletedAsync();
            return View(categories);
        }

        [Authorize(Roles = $"{Roles.Superadmin}, {Roles.Admin}, {Roles.User}")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize(Roles = $"{Roles.Superadmin}, {Roles.Admin}, {Roles.User}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CategoryAddDto categoryAddDto)
        {
            var map = mapper.Map<Category>(categoryAddDto);
            var result = await validator.ValidateAsync(map);

            var categoryUpdateDto = mapper.Map<CategoryUpdateDto>(categoryAddDto);
            var exists = await categoryService.Exists(categoryUpdateDto);

            if (exists)
            {
                result.Errors.Add(new ValidationFailure("Name", "This category name already exists."));
            }

            if (result.IsValid)
            {
                await categoryService.CreateCategoryAsync(categoryAddDto);
                toast.AddSuccessToastMessage(Messages.Category.Add(categoryAddDto.Name));
                return RedirectToAction("Index", "Category", new { Area = "Admin" });
            }

            result.AddToModelStateLoop(this.ModelState);
            return View();
        }

        [Authorize(Roles = $"{Roles.Superadmin}, {Roles.Admin}, {Roles.User}")]
        [HttpPost]
        public async Task<IActionResult> AddWithAjax([FromBody] CategoryAddDto categoryAddDto)
        {
            var map = mapper.Map<Category>(categoryAddDto);
            var result = await validator.ValidateAsync(map);

            var categoryUpdateDto = mapper.Map<CategoryUpdateDto>(categoryAddDto);
            var exists = await categoryService.Exists(categoryUpdateDto);

            if (exists)
            {
                result.Errors.Add(new ValidationFailure("Name", "This category name already exists."));
            }

            if (result.IsValid)
            {
                await categoryService.CreateCategoryAsync(categoryAddDto);
                toast.AddSuccessToastMessage(Messages.Category.Add(categoryAddDto.Name));
                return Json(Messages.Category.Add(categoryAddDto.Name));
            }
            else
            {
                toast.AddErrorToastMessage(result.Errors.First().ErrorMessage);
                return Json(result.Errors.First().ErrorMessage);
            }
        }

        [Authorize(Roles = $"{Roles.Superadmin}, {Roles.Admin}")]
        [HttpGet]
        public async Task<IActionResult> Update(Guid categoryId)
        {
            var entityexists = await categoryService.DoesEntityExist(categoryId);

            if (!entityexists)
            {
                return NotFound();
            }

            var category = await categoryService.GetCategoryByGuid(categoryId);
            var categoryUpdateDto = mapper.Map<CategoryUpdateDto>(category);

            return View(categoryUpdateDto);
        }

        [Authorize(Roles = $"{Roles.Superadmin}, {Roles.Admin}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CategoryUpdateDto categoryUpdateDto)
        {
            var map = mapper.Map<Category>(categoryUpdateDto);
            var result = await validator.ValidateAsync(map);

            var exists = await categoryService.Exists(categoryUpdateDto);

            if (exists)
            {
                result.Errors.Add(new ValidationFailure("Name", "This category name already exists."));
            }

            if (result.IsValid)
            {
                var name = await categoryService.UpdateCategoryAsync(categoryUpdateDto);
                toast.AddSuccessToastMessage(Messages.Category.Update(name));
                return RedirectToAction("Index", "Category", new { Area = "Admin" });
            }

            result.AddToModelStateLoop(this.ModelState);
            return View();
        }

        [Authorize(Roles = $"{Roles.Superadmin}, {Roles.Admin}")]
        public async Task<IActionResult> Delete(Guid categoryId)
        {
            var entityexists = await categoryService.DoesEntityExist(categoryId);

            if (!entityexists)
            {
                return NotFound();
            }

            await categoryService.SafeDeleteCategoryAsync(categoryId);

            return RedirectToAction("Index", "Category", new { Area = "Admin" });
        }

        [Authorize(Roles = $"{Roles.Superadmin}")]
        [Authorize(Roles = "Superadmin")]
        public async Task<IActionResult> UndoDelete(Guid categoryId)
        {
            var entityexists = await categoryService.DoesEntityExist(categoryId);

            if (!entityexists)
            {
                return NotFound();
            }

            await categoryService.UndoDeleteCategoryAsync(categoryId);

            return RedirectToAction("Index", "Category", new { Area = "Admin" });
        }
    }
}
