using FinalProject.DAL;
using FinalProject.Models;
using FinalProject.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace FinalProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly Database _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IStringLocalizer<SharedResource> _localizer;
        public AccountController(Database context,UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer=localizer;
        }

        //register for members
        [HttpGet]
        public async Task<IActionResult> MemberRegister()
        {
            ViewData["Localizer"] = _localizer;
            return View();
        }
        [HttpPost]
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
                FullName=registerVM.Fullname,
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

        //register for stores
        [HttpGet]
        public async Task<IActionResult> StoreRegister()
        {
            ViewData["Localizer"] = _localizer;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> StoreRegister(StoreRegisterViewModel registerVM)
        {
            if (!ModelState.IsValid) return View();
            var storeAccount = await _userManager.FindByEmailAsync(registerVM.Email);
            if (storeAccount != null)
            {
                ModelState.AddModelError("Email", "Email has taken!");
                return View();
            }
            var existStoreName = _context.Stores.FirstOrDefault(sn => sn.StoreName == registerVM.Storename); 
            if (existStoreName != null)
            {
                ModelState.AddModelError("Storename", "Storename has taken!");
                return View();
            }
            storeAccount = new AppUser
            {
                Email = registerVM.Email,
                UserName = registerVM.Email,
                Address=registerVM.Address,
            };
            var passwordResult = await _userManager.CreateAsync(storeAccount, registerVM.Password);
            if (!passwordResult.Succeeded)
            {
                foreach (var err in passwordResult.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                    return View();
                }
            }
            var roleResult = await _userManager.AddToRoleAsync(storeAccount, "Store");
            foreach (var err in roleResult.Errors)
            {
                ModelState.AddModelError("", err.Description);
                return View();
            }
            Store store=new Store
            {
                Email=registerVM.Email,
                StoreName = registerVM.Storename,
                PhoneNumber1=registerVM.PhoneNumber,
                Address= registerVM.Address,
            };
            _context.Stores.Add(store);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            ViewData["Localizer"] = _localizer;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid) return View();
            var user = await _userManager.FindByEmailAsync(loginVM.Email);
            if(user == null)
            {
                ModelState.AddModelError("","Email or password incorrect!");
                return View();
            }
            var password = await _signInManager.PasswordSignInAsync(user, loginVM.Password,loginVM.RememberMe,false);
            if (!password.Succeeded)
            {
                ModelState.AddModelError("", "Email or password incorrect!");
                return View();
            }
            return RedirectToAction("Index","Home");
        }
        public async Task<IActionResult> LogOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                await _signInManager.SignOutAsync();
            }
            return RedirectToAction("Login","Account");
        }

    }
}
