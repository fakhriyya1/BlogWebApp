using BlogCK.Entity.DTOs.Users;
using BlogCK.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogCK.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly RoleManager<AppRole> roleManager;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            if (ModelState.IsValid)
            {
                if (!userRegisterDto.AgreeToTerms)
                {
                    ModelState.AddModelError(nameof(userRegisterDto.AgreeToTerms), "You must agree to the terms and privacy policy.");
                    return View(userRegisterDto);
                }

                var user = new AppUser
                {
                    UserName = userRegisterDto.Email,
                    Email = userRegisterDto.Email,
                    FirstName="N/A",
                    LastName="N/A",
                    ImageId=Guid.Parse("07befd42-fb64-4c3f-a895-ace440aaa648")
                };

                var result = await userManager.CreateAsync(user, userRegisterDto.Password);

                if (result.Succeeded)
                {
                    var defaultRole = "User";
                    await userManager.AddToRoleAsync(user, defaultRole);

                    await signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home", new { Area = "" });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            return View(userRegisterDto);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(userLoginDto.Email);
                if (user != null)
                {
                    var result = await signInManager.PasswordSignInAsync(user, userLoginDto.Password, userLoginDto.RememberMe, false);
                    if (result.Succeeded)
                    {
                        var roles=await userManager.GetRolesAsync(user);

                        if (roles.Contains("User"))
                        {
                            return RedirectToAction("Index", "Home", new { Area = "" });
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home", new { Area = "Admin" });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Your email or password is incorrect");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Your email or password is incorrect");
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        [Authorize] //kiminse logout ola bilmese ucun evvelce hemin sisteme authorize olmus olmasi lazimdi
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { Area = "" });
        }

        [HttpGet]
        [Authorize] //kiminse logout ola bilmese ucun evvelce hemin sisteme authorize olmus olmasi lazimdi
        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }

    }
}
