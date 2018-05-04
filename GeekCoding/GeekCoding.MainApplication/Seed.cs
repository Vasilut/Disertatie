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
        public static async Task CreateRoles(IServiceProvider serviceProvider)
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
            const string userName = "lucian.vasilut10@gmail.com";
            var user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, _rolesNames[0]);
            }

        }
    }
}
