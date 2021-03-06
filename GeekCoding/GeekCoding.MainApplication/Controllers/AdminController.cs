﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GeekCoding.Common.Helpers;
using GeekCoding.Data.Models;
using GeekCoding.MainApplication.Pagination;
using GeekCoding.MainApplication.Utilities.DTO;
using GeekCoding.MainApplication.Utilities.Services;
using GeekCoding.MainApplication.ViewModels;
using GeekCoding.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GeekCoding.MainApplication.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        string[] _rolesNames = { "Admin", "Member", "Proponent" };
        private IUserInformationRepository _userInformationRepository;
        private UserManager<User> _userManager;
        private IUserInformationService _userInformationService;
        private List<SelectListItem> _roles = new List<SelectListItem>();

        public AdminController(IUserInformationRepository userInformationRepository, UserManager<User> userManager,
                               IUserInformationService userRegistration)
        {
            _userInformationRepository = userInformationRepository;
            _userManager = userManager;
            _userInformationService = userRegistration;


            _roles = Membership.Roles;
        }


        [HttpGet]
        public IActionResult Index(string searchString, int? page)
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
            if (!String.IsNullOrEmpty(searchString))
            {
                usersToShow = usersToShow.Where(usr => usr.Username.Contains(searchString)).ToList();
            }

            int pageSize = 15;
            return View(PaginatedList<UserDto>.CreateAsync(usersToShow, page ?? 1, pageSize));
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = new UserInformationViewModel();
            model.SelectListItems = _roles;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] UserInformationViewModel userInformation)
        {
            if (ModelState.IsValid)
            {
                var result = await _userInformationService.RegisterUser(userInformation,userInformation.Role);
                if (result == true)
                {
                    return RedirectToAction("Index", new { searchString = string.Empty });
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

        [HttpPost]
        public async Task<IActionResult> ImportFileExecute([FromForm]FileExecutionViewModel file)
        {
            var reader = new StreamReader(file.File.OpenReadStream());
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var userToBeRegistered = line.Split(';');
                if (userToBeRegistered.Length < 7)
                {
                    continue;
                }
                var userToRegister = new UserInformationViewModel
                {
                    Nume = userToBeRegistered[0],
                    Prenume = userToBeRegistered[1],
                    Profesor = userToBeRegistered[2],
                    Clasa = userToBeRegistered[3],
                    Scoala = userToBeRegistered[4],
                    Username = userToBeRegistered[5],
                    Password = userToBeRegistered[6]
                };

                var result = await _userInformationService.RegisterUser(userToRegister,string.Empty);
                if (result == false)
                {
                    //log the errors
                    ModelState.AddModelError("", $"Something bad happened for user + {userToRegister.Nume}");
                }

            }

            return RedirectToAction(nameof(Index), new { searchString = string.Empty });
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
            var usrInformation = _userInformationRepository.GetUserById(id);
            if (usrInformation == null)
            {
                return View(new UserInformationDto());
            }
            return View(new UserInformationDto
            {
                IdUser = usrInformation.IdUser,
                Clasa = usrInformation.Clasa,
                Nume = usrInformation.Nume,
                Password = string.Empty,
                Prenume = usrInformation.Prenume,
                Profesor = usrInformation.Profesor,
                Scoala = usrInformation.Scoala,
                Username = usrInformation.Username,
                SelectListItems = _roles
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] UserInformationViewModel userInformationViewModel)
        {
            if (ModelState.IsValid)
            {
                var userToUpdate = _userInformationRepository.GetUserById(userInformationViewModel.IdUser);
                if (userToUpdate == null)
                {
                    return BadRequest("No entity to update");
                }

                userToUpdate.Clasa = userInformationViewModel.Clasa;
                userToUpdate.Nume = userInformationViewModel.Nume;
                userToUpdate.Prenume = userInformationViewModel.Prenume;
                userToUpdate.Profesor = userInformationViewModel.Profesor;
                userToUpdate.Scoala = userInformationViewModel.Scoala;
                userToUpdate.Username = userInformationViewModel.Username;

                //see if we need to update the password
                if (!string.IsNullOrEmpty(userInformationViewModel.Password))
                {
                    var userToUpdatePassword = await _userManager.FindByIdAsync(userInformationViewModel.IdUser);
                    if (userToUpdatePassword == null)
                    {
                        return BadRequest("No user to update");
                    }

                    //update password
                    var newPasswordHash = _userManager.PasswordHasher.HashPassword(userToUpdatePassword, userInformationViewModel.Password);
                    userToUpdatePassword.PasswordHash = newPasswordHash;
                    var resultPasswordUpdated = await _userManager.UpdateAsync(userToUpdatePassword);
                    if (!resultPasswordUpdated.Succeeded)
                    {
                        return BadRequest("Failed to update password");
                    }
                }

                //update role
                var userToUpdateRole = await _userManager.FindByIdAsync(userInformationViewModel.IdUser);
                //delete user from all the roles
                foreach (var role in _rolesNames)
                {
                    var isInRole = await _userManager.IsInRoleAsync(userToUpdateRole, role);
                    if (isInRole)
                    {
                        await _userManager.RemoveFromRoleAsync(userToUpdateRole, role);
                    }
                }

                //asign the new role
                await _userManager.AddToRoleAsync(userToUpdateRole, userInformationViewModel.Role);


                _userInformationRepository.Update(userToUpdate);
                _userInformationRepository.Save();

                return RedirectToAction(nameof(Index), new { searchString = string.Empty });
            }
            return RedirectToAction(nameof(Edit), new { id = userInformationViewModel.IdUser });
        }


        [HttpGet]
        public IActionResult Delete(string id)
        {
            var userInformation = _userInformationRepository.GetUserById(id);
            if (userInformation == null)
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
            if (String.IsNullOrEmpty(idUser))
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
                return RedirectToAction(nameof(Delete), new { id = idUser });
            }

            //the user is not an admin
            foreach (var role in _rolesNames)
            {
                var isInRole = await _userManager.IsInRoleAsync(userToDelete, role);
                if (isInRole)
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

            return RedirectToAction(nameof(Index), new { searchString = string.Empty });
        }

    }
}