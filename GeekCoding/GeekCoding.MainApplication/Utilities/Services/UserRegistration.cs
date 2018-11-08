using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeekCoding.Data.Models;
using GeekCoding.MainApplication.ViewModels;
using GeekCoding.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace GeekCoding.MainApplication.Utilities.Services
{
    public class UserRegistration : IUserRegistration
    {
        private IUserInformationRepository _userInformationRepository;
        private UserManager<User> _userManager;

        public UserRegistration(IUserInformationRepository userInformationRepository, UserManager<User> userManager)
        {
            _userInformationRepository = userInformationRepository;
            _userManager = userManager;
        }
        public async Task<bool> RegisterUser(UserInformationViewModel userInformation)
        {
            var user = await _userManager.FindByNameAsync(userInformation.Username);
            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = userInformation.Username,
                    Email = userInformation.Username
                };

                var result = await _userManager.CreateAsync(user, userInformation.Password);
                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var userToConfirm = await _userManager.FindByEmailAsync(user.Email);

                    if (userToConfirm != null)
                    {
                        var resultFromConfirmation = await _userManager.ConfirmEmailAsync(user, token);

                        if (resultFromConfirmation.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(user, "Member");

                            //add user information
                            var userInformationToCurrentUser = new UserInformation
                            {
                                IdUser = userToConfirm.Id,
                                Clasa = userInformation.Clasa,
                                Nume = userInformation.Nume,
                                Prenume = userInformation.Prenume,
                                Profesor = userInformation.Profesor,
                                Scoala = userInformation.Scoala,
                                Username = userInformation.Username
                            };
                            await _userInformationRepository.AddAsync(userInformationToCurrentUser);
                            return true;
                        }
                    }
                    else
                    {
                        //error
                        return false;
                    }

                }
                else
                {
                    //error
                    return false;
                }
            }
            return false;

        }
    }
}
