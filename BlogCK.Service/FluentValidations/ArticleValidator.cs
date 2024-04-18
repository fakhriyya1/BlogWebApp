using BlogCK.Entity.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCK.Service.FluentValidations
{
    public class ArticleValidator:AbstractValidator<Article>
    {
        public ArticleValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(150);
            RuleFor(x => x.Content)
                .NotEmpty()
                .MinimumLength(100)
                .MaximumLength(1500);
        }
    }
}
