using AutoMapper;
using BlogCK.Data.UnitOfWorks;
using BlogCK.Entity.DTOs.Users;
using BlogCK.Entity.Entities;
using BlogCK.Entity.Enums;
using BlogCK.Service.Extensions;
using BlogCK.Service.Helpers.Images;
using BlogCK.Service.Services.Abstractions;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogCK.Service.Services.Concrete
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly IImageHelper imageHelper;
        private readonly ClaimsPrincipal _user;

        public UserService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager, IHttpContextAccessor httpContextAccessor, IMapper mapper, IUnitOfWork unitOfWork, IImageHelper imageHelper)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.httpContextAccessor = httpContextAccessor;
            _user = httpContextAccessor.HttpContext.User;
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.imageHelper = imageHelper;
        }

        public async Task<List<UserDto>> GetAllUsersWithRoleAsync()
        {
            var users = await userManager.Users.ToListAsync();
            var userDto = mapper.Map<List<UserDto>>(users);

            foreach (var user in userDto)
            {
                var findUser = await userManager.FindByIdAsync(user.Id.ToString());
                var role = String.Join("", await userManager.GetRolesAsync(findUser));
                // burada join istifade elemeyimizin sebebi: bir userin bir nece rolu ola biler, ancaq bizim sistemde ele deyil ona gore de bir deyeri almaq ucun istifade edirik.

                user.Role = role;
            }

            return userDto;
        }

        public async Task<List<AppRole>> GetAllRolesAsync()
        {
            return await roleManager.Roles.ToListAsync();
        }

        public async Task<IdentityResult> CreateUserAsync(UserAddDto userAddDto)
        {
            var appUser = mapper.Map<AppUser>(userAddDto);   //niye bunu edirik?
                                                             //This is typically done to transfer data between layers of your application, such as from the presentation layer (DTO) to the data layer (Entity). Yeni ki database'e yazmaq ucunnn

            appUser.UserName = userAddDto.Email;
            var result = await userManager.CreateAsync(appUser, string.IsNullOrEmpty(userAddDto.Password) ? "" : userAddDto.Password);

            if (result.Succeeded)
            {
                var findRole = await roleManager.FindByIdAsync(userAddDto.RoleId.ToString());
                await userManager.AddToRoleAsync(appUser, findRole.ToString());
                return result;
            }
            else
                return result;
        }

        public async Task<AppUser> GetAppUserByIdAsync(Guid userId)
        {
            return await userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<string> GetUserRoleAsync(AppUser user)
        {
            return string.Join("", await userManager.GetRolesAsync(user));
        }

        public async Task<Guid> GetUserRoleIdAsync(AppUser user)
        {
            var roleId=await unitOfWork.GetRepository<AppUserRole>().GetTAsync(x=>x.UserId==user.Id);

            return roleId.RoleId;
        }

        public async Task<IdentityResult> UpdateUserAsync(UserUpdateDto userUpdateDto)
        {
            var user = await GetAppUserByIdAsync(userUpdateDto.Id);
            var userRole = await GetUserRoleAsync(user);
            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                await userManager.RemoveFromRoleAsync(user, userRole);
                var findRole = await roleManager.FindByIdAsync(userUpdateDto.RoleId.ToString());
                await userManager.AddToRoleAsync(user, findRole.Name);
                return result;
            }
            else
                return result;
        }

        public async Task<(IdentityResult identityResult, string? email)> DeleteUserAsync(Guid userId)
        {
            var user = await GetAppUserByIdAsync(userId);
            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
                return (result, user.Email);
            else
                return (result, null);
        }

        public async Task<UserProfileDto> GetUserProfileAsync()
        {
            var userId = _user.GetLoggedInUserId();
            var getUserWithImage = await unitOfWork.GetRepository<AppUser>().GetTAsync(x=>x.Id == userId, x=>x.Image);
            var map = mapper.Map<UserProfileDto>(getUserWithImage);
            map.Image.FileName=getUserWithImage.Image.FileName;

            return map;
        }

        private async Task<Guid> UploadImageForUser(UserProfileDto userProfileDto)
        {
            var userEmail = _user.GetLoggedInUserEmail();

            var imageUpload = await imageHelper.Upload($"{userProfileDto.FirstName}{userProfileDto.LastName}", userProfileDto.Photo, ImageType.User);
            Image image = new(imageUpload.FullName, userProfileDto.Photo.ContentType, userEmail);
            await unitOfWork.GetRepository<Image>().AddAsync(image);

            return image.Id;
        }

        public async Task<bool> UserProfileUpdateAsync(UserProfileDto userProfileDto)
        {
            var userId = _user.GetLoggedInUserId();
            //var getUserWithImage = await unitOfWork.GetRepository<AppUser>().GetTAsync(x => x.Id == userId, x => x.Image);
            var user = await GetAppUserByIdAsync(userId);

            var isVerified = await userManager.CheckPasswordAsync(user, userProfileDto.CurrentPassword);

            if (isVerified && userProfileDto.NewPassword != null)
            {
                var result = await userManager.ChangePasswordAsync(user, userProfileDto.CurrentPassword, userProfileDto.NewPassword);

                if (result.Succeeded)
                {
                    await userManager.UpdateSecurityStampAsync(user);
                    await signInManager.SignOutAsync();
                    await signInManager.PasswordSignInAsync(user, userProfileDto.NewPassword, true, false);

                    mapper.Map(userProfileDto, user);

                    if (userProfileDto.Photo != null)
                    {
                        imageHelper.Delete(user.Image.FileName);
                        user.ImageId = await UploadImageForUser(userProfileDto);
                    }

                    await userManager.UpdateAsync(user);
                    await unitOfWork.SaveAsync();

                    return true;
                }
                else
                    return false;

            }
            else if (isVerified)
            {
                await userManager.UpdateSecurityStampAsync(user);
                mapper.Map(userProfileDto, user);

                if (userProfileDto.Photo != null)
                {
                    imageHelper.Delete(user.Image.FileName);
                    user.ImageId = await UploadImageForUser(userProfileDto);
                }

                await userManager.UpdateAsync(user);
                await unitOfWork.SaveAsync();

                return true;
            }
            else
                return false;
            
        }
    }
}
