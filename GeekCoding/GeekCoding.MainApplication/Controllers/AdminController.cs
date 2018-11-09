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
        string[] _rolesNames = {"Member", "Proponent" };
        private IUserInformationRepository _userInformationRepository;
        private UserManager<User> _userManager;
        private IUserInformationService _userInformationService;

        public AdminController(IUserInformationRepository userInformationRepository, UserManager<User> userManager,
                               IUserInformationService userRegistration)
        {
            _userInformationRepository = userInformationRepository;
            _userManager = userManager;
            _userInformationService = userRegistration;
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
                var result = await _userInformationService.RegisterUser(userInformation);
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
            var userInformation = _userInformationRepository.GetUserById(id);
            return View(userInformation);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Delete(string id)
        {
            var userInformation = _userInformationRepository.GetUserById(id);
            if(userInformation == null)
            {
                //that means we don't have any information in userinformation for this user
                //assign the id from the user
                return View(new UserInformation { IdUser = id, Username = User.Identity.Name });
            }
            return View(userInformation);
        }
        
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string idUser)
        {
            //if the role is admin, do not delete the user
            if(String.IsNullOrEmpty(idUser))
            {
                //it is not in the user information
                ModelState.AddModelError("", "You cannot delete an null user");
                return View();
            }

            var userToDelete = await _userManager.FindByIdAsync(idUser);
            var isAdmin = await _userManager.IsInRoleAsync(userToDelete, "Admin");
            if (isAdmin == true)
            {
                ModelState.AddModelError("", "You cannot delete an admin");
                return RedirectToAction(nameof(Delete), new { id = idUser});
            }

            //the user is not an admin
            foreach (var role in _rolesNames)
            {
                var isInRole = await _userManager.IsInRoleAsync(userToDelete, role);
                if(isInRole)
                {
                    await _userManager.RemoveFromRoleAsync(userToDelete, role);
                }
            }

            //delete from asp.net.users
            await _userManager.DeleteAsync(userToDelete);

            //delete userinformation
            var userInformationBasedOnId = _userInformationRepository.GetUserById(idUser);
            _userInformationRepository.Delete(userInformationBasedOnId);
            _userInformationRepository.Save();

            return RedirectToAction(nameof(Index));
        }

    }
}