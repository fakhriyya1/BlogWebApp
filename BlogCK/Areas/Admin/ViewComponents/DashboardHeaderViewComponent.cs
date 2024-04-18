using AutoMapper;
using BlogCK.Data.UnitOfWorks;
using BlogCK.Entity.DTOs.Users;
using BlogCK.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogCK.Web.Areas.Admin.ViewComponents
{
    public class DashboardHeaderViewComponent : ViewComponent
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public DashboardHeaderViewComponent(UserManager<AppUser> userManager, IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var loggedInUser = await userManager.GetUserAsync(HttpContext.User);
            var getUserWithImage = await unitOfWork.GetRepository<AppUser>().GetTAsync(x => x.Id == loggedInUser.Id, x => x.Image);
            var map=mapper.Map<UserDto>(getUserWithImage);
            map.Image.FileName = getUserWithImage.Image.FileName;


            var role = String.Join("", await userManager.GetRolesAsync(loggedInUser));
            map.Role = role;

            return View(map);
        }
    }
}
