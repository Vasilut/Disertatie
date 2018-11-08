using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeekCoding.Data.Models;
using GeekCoding.MainApplication.Utilities.DTO;
using GeekCoding.MainApplication.Utilities.Services;
using GeekCoding.MainApplication.ViewModels;
using GeekCoding.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GeekCoding.MainApplication.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private IUserInformationRepository _userInformationRepository;
        private UserManager<User> _userManager;
        private IUserRegistration _userRegistration;

        public AdminController(IUserInformationRepository userInformationRepository, UserManager<User> userManager,
                               IUserRegistration userRegistration)
        {
            _userInformationRepository = userInformationRepository;
            _userManager = userManager;
            _userRegistration = userRegistration;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var allUsers = _userManager.Users.ToList();
            var usersToShow = new List<UserDto>();
            foreach (var user in allUsers)
            {
                usersToShow.Add(new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName
                });
            }
            return View(usersToShow);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] UserInformationViewModel userInformation)
        {
            if (ModelState.IsValid)
            {
                var result = await _userRegistration.RegisterUser(userInformation);
                if (result == true)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Something bad happened.");
                return View();
            }
            return View();
        }

        [HttpGet]
        public IActionResult ImportUsers()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Delete(string id)
        {
            return View();
        }

    }
}