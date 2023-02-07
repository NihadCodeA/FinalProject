﻿using FinalProject.DAL;
using FinalProject.Models;
using FinalProject.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        //register for members
        [HttpGet]
        public async Task<IActionResult> MemberRegister()
        {
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
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> StoreRegister(StoreRegisterViewModel registerVM)
        {
            if (!ModelState.IsValid) return View();
            var store = await _userManager.FindByEmailAsync(registerVM.Email);
            if (store != null)
            {
                ModelState.AddModelError("Email", "Email has taken!");
                return View();
            }
            store = new AppUser
            {
                StoreName = registerVM.Storename,
                Email = registerVM.Email,
                UserName = registerVM.Email,
                Address=registerVM.Address,
                PhoneNumber1= registerVM.PhoneNumber,
            };
            var passwordResult = await _userManager.CreateAsync(store, registerVM.Password);
            if (!passwordResult.Succeeded)
            {
                foreach (var err in passwordResult.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                    return View();
                }
            }
            var roleResult = await _userManager.AddToRoleAsync(store, "Store");
            foreach (var err in roleResult.Errors)
            {
                ModelState.AddModelError("", err.Description);
                return View();
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
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
