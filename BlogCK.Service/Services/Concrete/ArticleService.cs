using AutoMapper;
using BlogCK.Data.UnitOfWorks;
using BlogCK.Entity.DTOs.Articles;
using BlogCK.Entity.Entities;
using BlogCK.Entity.Enums;
using BlogCK.Service.Extensions;
using BlogCK.Service.Helpers.Images;
using BlogCK.Service.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogCK.Service.Services.Concrete
{
    public class ArticleService : IArticleService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IImageHelper imageHelper;
        private readonly ClaimsPrincipal user;

        public ArticleService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, IImageHelper imageHelper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.imageHelper = imageHelper;
            user = httpContextAccessor.HttpContext.User;
        }

        public async Task<List<ArticleDto>> GetAllArticlesWithCategoryNonDeletedAsync()
        {
            var articles = await unitOfWork.GetRepository<Article>().GetAllAsync(x => !x.isDeleted && !x.Category.isDeleted, y => y.Category);
            var map = mapper.Map<List<ArticleDto>>(articles);

            return map;
        }

        public async Task<List<ArticleDto>> Get3RecentArticlesWithCategoryNonDeletedAsync()
        {
            var articles = await unitOfWork.GetRepository<Article>().GetAllAsync(x => !x.isDeleted && !x.Category.isDeleted, y => y.Category);
            articles = articles.OrderByDescending(x=>x.CreatedDate).Take(3).ToList();
            var map = mapper.Map<List<ArticleDto>>(articles);

            return map;
        }


        public async Task<ArticleListDto> GetAllArticlesByPaging(Guid? categoryId, int currentPage = 1, int pageSize = 3, bool isAscending = false)
        {
            pageSize = pageSize > 20 ? 20 : pageSize;

            var articles = categoryId == null
                ? await unitOfWork.GetRepository<Article>().GetAllAsync(a => !a.isDeleted, c => c.Category, i => i.Image, u => u.User)
                : await unitOfWork.GetRepository<Article>().GetAllAsync(a => a.CategoryId == categoryId && !a.isDeleted, c => c.Category, i => i.Image, u => u.User);

            var sortedArticles = isAscending
                ? articles.OrderBy(x => x.CreatedDate).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
                : articles.OrderByDescending(x => x.CreatedDate).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            return new ArticleListDto
            {
                Articles = sortedArticles,
                CategoryId = categoryId == null ? null : categoryId.Value,
                CurrentPage = currentPage,
                PageSize = pageSize,
                IsAscending = isAscending,
                TotalCount = articles.Count()
            };
        }

        public async Task CreateArticleAsync(ArticleAddDto articleAddDto)
        {
            var userId = user.GetLoggedInUserId();
            var userEmail = user.GetLoggedInUserEmail();

            var imageUpload = await imageHelper.Upload(articleAddDto.Title, articleAddDto.Photo, ImageType.Post);
            Image image = new(imageUpload.FullName, articleAddDto.Photo.ContentType, userEmail);
            await unitOfWork.GetRepository<Image>().AddAsync(image);

            var article = new Article(articleAddDto.Title, articleAddDto.Content, articleAddDto.CategoryId, image.Id, userId, userEmail);

            await unitOfWork.GetRepository<Article>().AddAsync(article);
            await unitOfWork.SaveAsync();
        }

        public async Task<ArticleDto> GetArticleWithCategoryNonDeletedAsync(Guid articleId)
        {
            var article = await unitOfWork.GetRepository<Article>().GetTAsync(x => !x.isDeleted && x.Id == articleId, y => y.Category, z => z.Image, u=>u.User);
            var map = mapper.Map<ArticleDto>(article);

            return map;
        }

        public async Task<string> UpdateArticleAsync(ArticleUpdateDto articleUpdateDto)
        {
            var article = await unitOfWork.GetRepository<Article>().GetTAsync(x => !x.isDeleted && x.Id == articleUpdateDto.Id, y => y.Category, z => z.Image);
            var userEmail = user.GetLoggedInUserEmail();
            string articleTitle = article.Title;

            if (articleUpdateDto.Photo != null)
            {
                imageHelper.Delete(article.Image.FileName);

                var imageUpload = await imageHelper.Upload(articleUpdateDto.Title, articleUpdateDto.Photo, ImageType.Post);
                Image image = new(imageUpload.FullName, articleUpdateDto.Photo.ContentType, userEmail);

                await unitOfWork.GetRepository<Image>().AddAsync(image);

                articleUpdateDto.Image = image;
                //article.ImageId = image.Id;
            }
            else
            {
                articleUpdateDto.Image = article.Image;
            }

            mapper.Map(articleUpdateDto, article);

            article.ModifiedDate = DateTime.Now;
            article.ModifiedBy = userEmail;

            await unitOfWork.GetRepository<Article>().UpdateAsync(article);
            await unitOfWork.SaveAsync();

            return articleTitle;
        }

        public async Task SafeDeleteArticleAsync(Guid articleId)
        {
            var article = await unitOfWork.GetRepository<Article>().GetByGuidAsync(articleId);
            var userEmail = user.GetLoggedInUserEmail();

            article.isDeleted = true;
            article.DeletedDate = DateTime.Now;
            article.DeletedBy = userEmail;

            await unitOfWork.GetRepository<Article>().UpdateAsync(article);
            await unitOfWork.SaveAsync();
        }

        public async Task<List<ArticleDto>> GetAllArticlesWithCategoryDeletedAsync()
        {
            var articles = await unitOfWork.GetRepository<Article>().GetAllAsync(x => x.isDeleted && !x.Category.isDeleted, y => y.Category);
            var map = mapper.Map<List<ArticleDto>>(articles);

            return map;
        }

        public async Task UndoDeleteArticleAsync(Guid articleId)
        {
            var article = await unitOfWork.GetRepository<Article>().GetByGuidAsync(articleId);

            article.isDeleted = false;
            article.DeletedDate = null;
            article.DeletedBy = null;

            await unitOfWork.GetRepository<Article>().UpdateAsync(article);
            await unitOfWork.SaveAsync();
        }

        public async Task<ArticleListDto> SearchAsync(string keyword, int currentPage = 1, int pageSize = 3, bool isAscending = false)
        {
            pageSize = pageSize > 20 ? 20 : pageSize;

            var articles = await unitOfWork.GetRepository<Article>().GetAllAsync(a => !a.isDeleted && (a.Title.Contains(keyword) || a.Content.Contains(keyword) || a.Category.Name.Contains(keyword)), c => c.Category, i => i.Image, u => u.User);

            var sortedArticles = isAscending
                ? articles.OrderBy(a => a.CreatedDate).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
                : articles.OrderByDescending(a => a.CreatedDate).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            return new ArticleListDto
            {
                Articles = sortedArticles,
                TotalCount = articles.Count,
                PageSize = pageSize,
                CurrentPage = currentPage,
                IsAscending = isAscending
            };
        }

        public async Task<ArticleDto> TriggerViewCount(Guid articleId)
        {
            var getIp = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var article = await unitOfWork.GetRepository<Article>().GetTAsync(x => x.Id == articleId);
            var visitor = await unitOfWork.GetRepository<Visitor>().GetTAsync(x => x.IpAddress == getIp);
            var articleVisitors = await unitOfWork.GetRepository<ArticleVisitor>().GetAllAsync(null, x => x.Visitor, y => y.Article);

            var result = await GetArticleWithCategoryNonDeletedAsync(articleId);

            var addArticleVisitor = new ArticleVisitor(article.Id, visitor.Id);

            if (articleVisitors.Any(x => x.VisitorId == addArticleVisitor.VisitorId && x.ArticleId == addArticleVisitor.ArticleId))
                return result;
            else
            {
                unitOfWork.GetRepository<ArticleVisitor>().AddAsync(addArticleVisitor);
                article.ViewCount++;
                unitOfWork.GetRepository<Article>().UpdateAsync(article);
                unitOfWork.SaveAsync();
            }
            return result;
        }

        public async Task<bool> DoesEntityExist(Guid articleId)
        {
            return await unitOfWork.GetRepository<Article>().AnyAsync(x=>x.Id==articleId);
        }
    }
}
