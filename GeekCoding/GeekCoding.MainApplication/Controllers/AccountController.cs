using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeekCoding.Common.EmailGenerator;
using GeekCoding.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GeekCoding.MainApplication.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private IMessageBuilder _emailSender;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
                                 IMessageBuilder emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        #region Register

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user == null)
                {
                    user = new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = model.UserName,
                        Email = model.UserName
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationEmail = Url.Action("ConfirmEmailAddress", "Account",
                            new { token, email = user.Email }, Request.Scheme);
                        _emailSender.AddReceiver(user.Email)
                                    .AddSubject("Confirmation to GeekCoding Site")
                                    .AddBody("Please confirm the mail at " + confirmationEmail)
                                    .BuildAndSend();
                        //send token via mail
                        return View("Success");
                    }
                    else
                    {
                        ModelState.AddModelError("", GetErrors(result.Errors));
                    }
                }

                return View();
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmailAddress(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    return View("MailConfirmed");
                }
            }

            return View("Error");
        }

        #endregion

        private string GetErrors(IEnumerable<IdentityError> errors)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in errors)
            {
                sb.Append(item.Description);
            }

            return sb.ToString();
        }

        #region Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user != null)
                {
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("", "Email is not confirmed");
                        return View();
                    }

                    var signInResult = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                    if (signInResult.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError("", "Invalid UserName or Password");
                }

                ModelState.AddModelError("", "Invalid UserName or Password");
            }

            return View();

            //login with _signInManager
            //if (ModelState.IsValid)
            //{

            //    var signInResult = await _signInManager.PasswordSignInAsync(model.UserName, model.Password,
            //        false, false);

            //    if (signInResult.Succeeded)
            //    {
            //        return RedirectToAction("Index", "Home");
            //    }

            //    ModelState.AddModelError("", "Invalid UserName or Password");
            //}

            //return View();
        }

        #endregion

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetUrl = Url.Action("ResetPassword", "Account",
                        new { token, email = user.Email }, Request.Scheme);
                    _emailSender.AddReceiver(user.Email)
                                    .AddSubject("Reset your password for GeekCoding Site")
                                    .AddBody("Reset password link: " + resetUrl)
                                    .BuildAndSend();
                    //send resetUrl via mail
                }
                else
                {
                    // email user and inform them that they do not have an account
                    _emailSender.AddReceiver(model.Email)
                                    .AddSubject("Create account on GeekCoding Site")
                                    .AddBody("You don't have any account on our site. Please register!")
                                    .BuildAndSend();
                }

                return View("ResetPasswordSent");
            }
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            return View(new ResetPasswordModel { Token = token, Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View();
                    }
                    return View("PasswordResetSuccess");
                }
                ModelState.AddModelError("", "Invalid Request");
            }
            return View();
        }
    }
}