using BlogCK.Entity.DTOs.Articles;
using BlogCK.Entity.DTOs.Categories;
using BlogCK.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCK.Service.Services.Abstractions
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllCategoriesNonDeletedAsync();
        Task<List<CategoryDto>> GetAllCategoriesNonDeletedAsyncTake10();
        Task<List<CategoryDto>> GetAllCategoriesDeletedAsync();
        Task<Category> GetCategoryByGuid(Guid categoryId);
        Task CreateCategoryAsync(CategoryAddDto categoryAddDto);
        Task<string> UpdateCategoryAsync(CategoryUpdateDto categoryUpdateDto);
        Task SafeDeleteCategoryAsync(Guid categoryId);
        Task UndoDeleteCategoryAsync(Guid categoryId);
        Task<bool> Exists(CategoryUpdateDto categoryUpdateDto);
        Task<bool> DoesEntityExist(Guid categoryId);
    }
}
