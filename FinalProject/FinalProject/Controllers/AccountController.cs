using FinalProject.Abstractions.MailService;
using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Http;
namespace FinalProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly Database _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMailService _mailService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly HttpContext _httpContext;
        public AccountController(Database context,UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager,IMailService mailService ,
            IStringLocalizer<SharedResource> localizer, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _mailService = mailService;
            _localizer=localizer;
            _httpContext = httpContextAccessor.HttpContext;
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
            ViewData["Localizer"] = _localizer;
            if (!ModelState.IsValid) return View();
            var user = await _userManager.FindByEmailAsync(registerVM.Email);
            //--------------------------------------------------------------
            string lang = GetCurrentLanguage.CurrentLanguage(_httpContext);
            string emailErrMessage = "";
            if (lang == "az")
            {
                emailErrMessage = "Eyni email adresini başqa bir hesap istifadə eliyir.";
            }
            else
            {
                emailErrMessage = "Another account is using the same email.";
            }
            //--------------------------------------------------------------
            if(user != null)
            {
                ModelState.AddModelError("Email",emailErrMessage);
                return View();
            }
            user= new AppUser 
            { 
                FullName=registerVM.Fullname,
                Email = registerVM.Email,
                UserName=registerVM.Email
            };
            //---------------------------------------------------------------------------
            string requireDigitErrorMessage = "";
            string requireLowercaseErrorMessage = "";
            string requireUppercaseErrorMessage = "";
            string requiredLengthErrorMessage = "";
            if (lang == "az")
            {
                requireDigitErrorMessage = "Şifrədə minimum 1 rəqəm olmalıdır!";
                requireLowercaseErrorMessage = "Şifrədə minimum 1 kiçik hərf olmalıdır!";
                requireUppercaseErrorMessage = "Şifrədə minimum 1 böyük hərf olmalıdır!";
                requiredLengthErrorMessage = "Şifrə minimum 8 simvoldan ibarət olmalıdır!";
            }
            else
            {
                requireDigitErrorMessage = "The password must contain at least 1 digit!";
                requireLowercaseErrorMessage = "The password must have at least 1 lowercase letter!";
                requireUppercaseErrorMessage = "The password must have at least 1 capital letter!";
                requiredLengthErrorMessage = "The password must contain at least 8 characters!";
            }
            //---------------------------------------------------------------------------

            var passwordResult = await _userManager.CreateAsync(user, registerVM.Password);
            if (!passwordResult.Succeeded) {
            foreach(var err in passwordResult.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                    return View();
                }
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, HttpContext.Request.Scheme);
            await _mailService.SendEmailAsync(
                new MailRequestViewModel { 
                    ToEmail=user.Email,
                    Subject="Confirm Email",
                    Body = $"<a href='{confirmationLink}'>{confirmationLink}</a>"}
                );

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            foreach (var err in roleResult.Errors)
             {
                  ModelState.AddModelError("", err.Description);
                  return View();
            }
            return RedirectToAction(nameof(CheckEmail));
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
            ViewData["Localizer"] = _localizer;
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
            ViewData["Localizer"] = _localizer;
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
            ViewData["Localizer"] = _localizer;
            if (User.Identity.IsAuthenticated)
            {
                await _signInManager.SignOutAsync();
            }
            return RedirectToAction("Login","Account");
        }

        public IActionResult ForgotPassword()
        {
            ViewData["Localizer"] = _localizer;
            return View();  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            ViewData["Localizer"] = _localizer;
            if (!ModelState.IsValid) return View(model); 
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                ModelState.AddModelError("Email","User is not found!");
                return View(model);
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            string link = Url.Action("ResetPassword","Account",new {userId=user.Id,token=token},HttpContext.Request.Scheme);

            await _mailService.SendEmailAsync(new MailRequestViewModel {ToEmail=model.Email,Subject="ResetPassword",Body=$"<a href='{link}'>Reset password</a>"});

            return RedirectToAction(nameof(CheckEmail));
        }

        public async Task<IActionResult> ResetPassword(string userId,string token)
        {
            ViewData["Localizer"] = _localizer;
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token)) return BadRequest();
            var user= await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model,string userId, string token)
        {
            ViewData["Localizer"] = _localizer;
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token)) return BadRequest();
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            var identityUser = await _userManager.ResetPasswordAsync(user, token,model.ConfirmPassword);

            return RedirectToAction(nameof(Login));
        }

        public IActionResult CheckEmail()
        {
            ViewData["Localizer"] = _localizer;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            ViewData["Localizer"] = _localizer;
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) NotFound();
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded ? nameof(ConfirmEmail) : NotFound());
        }
    }
}
