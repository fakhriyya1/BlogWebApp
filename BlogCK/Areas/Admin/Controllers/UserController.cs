using AutoMapper;
using BlogCK.Data.UnitOfWorks;
using BlogCK.Entity.DTOs.Articles;
using BlogCK.Entity.DTOs.Users;
using BlogCK.Entity.Entities;
using BlogCK.Entity.Enums;
using BlogCK.Service.Extensions;
using BlogCK.Service.Helpers.Images;
using BlogCK.Service.Services.Abstractions;
using BlogCK.Web.ResultMessages;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using static BlogCK.Web.ResultMessages.Messages;

namespace BlogCK.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IUserService userService;
        private readonly IToastNotification toast;
        private readonly IValidator<AppUser> validator;
        private readonly IMapper mapper;

        public UserController(IUserService userService, IToastNotification toast, IValidator<AppUser> validator, IMapper mapper)
        {
            this.userService = userService;
            this.toast = toast;
            this.validator = validator;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var usersDto = await userService.GetAllUsersWithRoleAsync();

            return View(usersDto);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var roles = await userService.GetAllRolesAsync();
            return View(new UserAddDto { Roles = roles });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(UserAddDto userAddDto)
        {
            var appUser = mapper.Map<AppUser>(userAddDto);
            var validation = await validator.ValidateAsync(appUser);
            var roles = await userService.GetAllRolesAsync();

            if (validation.IsValid)
            {
                var result = await userService.CreateUserAsync(userAddDto);

                if (result.Succeeded)
                {
                    toast.AddSuccessToastMessage(Messages.User.Add(userAddDto.Email));
                    return RedirectToAction("Index", "User", new { Area = "Admin" });
                }
                else
                {
                    result.AddToIdentityModelState(this.ModelState);
                    validation.AddToModelStateLoop(this.ModelState);
                    return View(new UserAddDto { Roles = roles });
                }
            }
            else
                validation.AddToModelStateLoop(this.ModelState);

            return View(new UserAddDto { Roles = roles });
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid userId)
        {
            var user = await userService.GetAppUserByIdAsync(userId);
            var roles = await userService.GetAllRolesAsync();
            var roleId = await userService.GetUserRoleIdAsync(user);

            var userUpdateDto = mapper.Map<UserUpdateDto>(user);
            userUpdateDto.Roles = roles;
            userUpdateDto.RoleId = roleId;

            return View(userUpdateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserUpdateDto userUpdateDto)
        {
            var user = await userService.GetAppUserByIdAsync(userUpdateDto.Id);

            if (user != null)
            {
                var roles = await userService.GetAllRolesAsync();
                if (ModelState.IsValid)
                {
                    var map = mapper.Map(userUpdateDto, user);
                    var validation=validator.Validate(map);

                    if (validation.IsValid)
                    {
                        user.UserName = userUpdateDto.Email;
                        user.SecurityStamp = Guid.NewGuid().ToString();
                        var result = await userService.UpdateUserAsync(userUpdateDto);
                        if (result.Succeeded)
                        {
                            toast.AddSuccessToastMessage(Messages.User.Update(userUpdateDto.Email));
                            return RedirectToAction("Index", "User", new { Area = "Admin" });
                        }
                        else
                        {
                            result.AddToIdentityModelState(this.ModelState);
                            return View(new UserUpdateDto { Roles = roles });
                        }
                    }
                    else
                    {
                        validation.AddToModelStateLoop(this.ModelState);
                        return View(new UserUpdateDto { Roles = roles });
                    }
                }
            }

            return NotFound();
        }

        public async Task<IActionResult> Delete(Guid userId)
        {
            var result = await userService.DeleteUserAsync(userId);

            if (result.identityResult.Succeeded)
            {
                toast.AddSuccessToastMessage(Messages.User.Delete(result.email));
                return RedirectToAction("Index", "User", new { Area = "Admin" });
            }
            else
            {
                result.identityResult.AddToIdentityModelState(this.ModelState);
            }
               
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var profile = await userService.GetUserProfileAsync();

            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileDto userProfileDto)
        {
            if (userProfileDto.CurrentPassword == null)
            {
                //var profile = await userService.GetUserProfileAsync();
                //userProfileDto.Image = profile.Image;
                ModelState.AddModelError(nameof(userProfileDto.CurrentPassword), "Current Password is required.");
                return View(userProfileDto);
            }

            var result = await userService.UserProfileUpdateAsync(userProfileDto);

            if (ModelState.IsValid)
            {
                if (result)
                {
                    toast.AddSuccessToastMessage("Your information has been changed successfully");
                    return RedirectToAction("Index", "Home", new { Area = "Admin" });
                }
                else
                {
                    var profile = await userService.GetUserProfileAsync();
                    toast.AddErrorToastMessage("An error occured while trying to update your information.");
                    return View(profile);
                }
            }
            else
                return NotFound();
        }
    }
}

