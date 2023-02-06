using FinalProject.DAL;
using FinalProject.Models;
using FinalProject.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly Database _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(Database context,UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        //register for members
        public async Task<IActionResult> MemberRegister(MemberRegisterViewModel registerVM)
        {
            if (!ModelState.IsValid) return View();
            var user = await _userManager.FindByEmailAsync(registerVM.Email);
            if(user != null)
            {
                ModelState.AddModelError("Email","Email has taken!");
                return View();
            }
            user= new AppUser 
            { 
                Fullname=registerVM.Fullname,
                Email = registerVM.Email,
                UserName=registerVM.Email
            };
            var passwordResult = await _userManager.CreateAsync(user, registerVM.Password);
            if (!passwordResult.Succeeded) {
            foreach(var err in passwordResult.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                    return View();
                }
            }
            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            foreach (var err in roleResult.Errors)
             {
                  ModelState.AddModelError("", err.Description);
                  return View();
            }
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> Login(LoginViewModel registerVM)
        {
            return Ok();
        }
        public async Task<IActionResult> LogOut()
        {
            return Ok();
        }

    }
}
