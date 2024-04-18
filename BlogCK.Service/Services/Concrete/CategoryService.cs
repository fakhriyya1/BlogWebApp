using AutoMapper;
using BlogCK.Data.UnitOfWorks;
using BlogCK.Entity.DTOs.Categories;
using BlogCK.Entity.Entities;
using BlogCK.Service.Extensions;
using BlogCK.Service.Helpers.Images;
using BlogCK.Service.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogCK.Service.Services.Concrete
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ClaimsPrincipal user;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            user = httpContextAccessor.HttpContext.User;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesNonDeletedAsync()
        {
            var categories = await unitOfWork.GetRepository<Category>().GetAllAsync(x => !x.isDeleted);
            var map = mapper.Map<List<CategoryDto>>(categories);

            return map;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesNonDeletedAsyncTake10()
        {
            var categories = await unitOfWork.GetRepository<Category>().GetAllAsync(x => !x.isDeleted);
            var map = mapper.Map<List<CategoryDto>>(categories);

            return map.Take(10).ToList();
        }

        public async Task<Category> GetCategoryByGuid(Guid categoryId)
        {
            var category = await unitOfWork.GetRepository<Category>().GetByGuidAsync(categoryId);

            return category;
        }

        public async Task CreateCategoryAsync(CategoryAddDto categoryAddDto)
        {
            var userEmail = user.GetLoggedInUserEmail();
            Category category = new Category(categoryAddDto.Name, userEmail);

            await unitOfWork.GetRepository<Category>().AddAsync(category);
            await unitOfWork.SaveAsync();
        }

        public async Task<string> UpdateCategoryAsync(CategoryUpdateDto categoryUpdateDto)
        {
            var category = await unitOfWork.GetRepository<Category>().GetTAsync(x => !x.isDeleted && x.Id == categoryUpdateDto.Id);
            var userEmail = user.GetLoggedInUserEmail();
            string categoryName = category.Name;

            
            mapper.Map(categoryUpdateDto, category);

            category.ModifiedDate = DateTime.Now;
            category.ModifiedBy = userEmail;

            await unitOfWork.GetRepository<Category>().UpdateAsync(category);
            await unitOfWork.SaveAsync();

            return categoryName;
        }

        public async Task SafeDeleteCategoryAsync(Guid categoryId)
        {
            var category = await unitOfWork.GetRepository<Category>().GetByGuidAsync(categoryId);
            var userEmail = user.GetLoggedInUserEmail();

            category.isDeleted = true;
            category.DeletedDate = DateTime.Now;
            category.DeletedBy = userEmail;

            await unitOfWork.GetRepository<Category>().UpdateAsync(category);
            await unitOfWork.SaveAsync();
        }

        public async Task<bool> Exists(CategoryUpdateDto categoryUpdateDto)
        {
            bool val = await unitOfWork.GetRepository<Category>().AnyAsync(x => x.Name == categoryUpdateDto.Name && x.Id!=categoryUpdateDto.Id);
            return val;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesDeletedAsync()
        {
            var categories = await unitOfWork.GetRepository<Category>().GetAllAsync(x => x.isDeleted);
            var map = mapper.Map<List<CategoryDto>>(categories);

            return map;
        }

        public async Task UndoDeleteCategoryAsync(Guid categoryId)
        {
            var category = await unitOfWork.GetRepository<Category>().GetByGuidAsync(categoryId);

            category.isDeleted = false;
            category.DeletedDate = null;
            category.DeletedBy = null;

            await unitOfWork.GetRepository<Category>().UpdateAsync(category);
            await unitOfWork.SaveAsync();
        }

        public async Task<bool> DoesEntityExist(Guid categoryId)
        {
            return await unitOfWork.GetRepository<Category>().AnyAsync(x => x.Id == categoryId);
        }
    }
}
