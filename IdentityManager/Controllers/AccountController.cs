using IdentityManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index() 
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register() 
        {
            var registerViewModel = new RegisterViewModel();
            return View(registerViewModel);
        }
        [HttpGet]
        public IActionResult Login()
        {
            var loginViewModel = new LoginViewModel();
            return View(loginViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel) 
        {
            if (ModelState.IsValid) 
            {
                var user = new ApplicationUser() { UserName = registerViewModel.Email, Email = registerViewModel.Email, Name = registerViewModel.Name };
                var result = await _userManager.CreateAsync(user, registerViewModel.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false).ConfigureAwait(true);
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            return View(registerViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {                
                var result = await _signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password,loginViewModel.RememberMe, lockoutOnFailure:false);
                if (result.Succeeded)
                {                    
                    return RedirectToAction("Index", "Home");
                }
                else 
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(loginViewModel);
                }
            }

            return View(loginViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff() 
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        private void AddErrors(IdentityResult identityResult) 
        {
            foreach (var error in identityResult.Errors)             
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
