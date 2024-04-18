using BlogCK.Entity.DTOs.Articles;
using BlogCK.Entity.Entities;

namespace BlogCK.Service.Services.Abstractions
{
    public interface IArticleService
    {
        Task<List<ArticleDto>> GetAllArticlesWithCategoryDeletedAsync();
        Task<List<ArticleDto>> GetAllArticlesWithCategoryNonDeletedAsync();  //async islem edeceyimiz ucun Task elave edirik
        Task<List<ArticleDto>> Get3RecentArticlesWithCategoryNonDeletedAsync();
        Task CreateArticleAsync(ArticleAddDto articleAddDto);
        Task<ArticleDto> GetArticleWithCategoryNonDeletedAsync(Guid articleId);
        Task<string> UpdateArticleAsync(ArticleUpdateDto articleUpdateDto);
        Task SafeDeleteArticleAsync(Guid articleId);
        Task UndoDeleteArticleAsync(Guid articleId);
        Task<ArticleListDto> GetAllArticlesByPaging(Guid? categoryId, int currentPage = 1, int pageSize = 3, bool isAscending = false);
        Task<ArticleListDto> SearchAsync(string keyword, int currentPage = 1, int pageSize = 3, bool isAscending = false);
        Task<ArticleDto> TriggerViewCount(Guid articleId);
        Task<bool> DoesEntityExist(Guid articleId);
    }
}
