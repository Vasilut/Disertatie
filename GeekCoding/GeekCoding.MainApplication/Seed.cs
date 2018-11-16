using GeekCoding.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication
{
    public static class Seed
    {
        public static async Task CreateRoles(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var _userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            string[] _rolesNames = { "Admin", "Member", "Proponent" };
            foreach (var roleName in _rolesNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
                }
            }

            //prepare admin user
            string userName = configuration.GetSection("UserSettings")["UserName"];
            string password = configuration.GetSection("UserSettings")["Password"];
            var user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, _rolesNames[0]);
            }
            else
            {
                user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = userName,
                    Email = userName
                };

                var isCreated = await _userManager.CreateAsync(user, password);
                if(isCreated.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var userToConfirm = await _userManager.FindByEmailAsync(user.Email);

                    if (userToConfirm != null)
                    {
                        var resultFromConfirmation = await _userManager.ConfirmEmailAsync(user, token);

                        if (resultFromConfirmation.Succeeded)
                        {
                            //add admin role
                            await _userManager.AddToRoleAsync(user, _rolesNames[0]);
                        }
                    }
                }
            }

        }
    }
}
