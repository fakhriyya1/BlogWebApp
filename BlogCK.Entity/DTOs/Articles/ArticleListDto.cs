﻿using BlogCK.Entity.Entities;

namespace BlogCK.Entity.DTOs.Articles
{
    public class ArticleListDto
    {
        public IList<Article> Articles { get; set; }
        public Guid? CategoryId { get; set; }
        public virtual int CurrentPage { get; set; } = 1;
        public virtual int PageSize { get; set; } = 3;   //bir sehifede nece blog gorunurse onu gosterir
        public virtual int TotalCount { get; set; }
        public virtual int TotalPages => (int)Math.Ceiling(decimal.Divide(TotalCount, PageSize));
        public virtual bool ShowPrevious => CurrentPage > 1;
        public virtual bool ShowNext => CurrentPage < TotalPages;
        public virtual bool IsAscending { get; set; } = false;

    }
}
