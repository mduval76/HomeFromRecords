using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.IO;

namespace HomeFromRecords.Core.Data.Entities {
    public static class Seed {
        public static readonly PasswordHasher<User> PASSWORD_HASHER = new();
        private static readonly List<string> Roles = new List<string> {
            "Admin",
            "Member"
        };

        public static void SeedAdmin(this ModelBuilder builder) {
            foreach (var role in Roles) {
                AddRole(builder, role);
            }

            var adminUser = AddUser(builder,
                "default",
                "default",
                "default",
                "admin@default.com",
                "default",
                "default",
                "default",
                "default",
                "default",
                "default",
                "Admin123!"
            );

            AddUserToRole(builder, adminUser, "Admin");
        }

        private static void AddRole(ModelBuilder builder, string roleName) {
            var role = builder.Model.FindEntityType(typeof(IdentityRole<Guid>))
                .GetSeedData().FirstOrDefault(sd => sd.Values.Contains(roleName.ToUpper()));

            if (role == null) {
                builder.Entity<IdentityRole<Guid>>().HasData(new IdentityRole<Guid> {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                });
            }
        }

        private static User AddUser(
                ModelBuilder builder,
                string firstName,
                string lastName,
                string username,
                string email,
                string phone,
                string streetAddress,
                string city,
                string region,
                string postalCode,
                string country,
                string password
            ) {
            var newUser = new User(username) {
                Id = Guid.NewGuid(),
                UserName = username,
                NormalizedUserName = username.ToUpper(),
                Email = email,
                NormalizedEmail = email.ToUpper(),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phone,
                StreetAddress = streetAddress,
                City = city,
                Region = region,
                PostalCode = postalCode,
                Country = country
            };

            newUser.PasswordHash = PASSWORD_HASHER.HashPassword(newUser, password);
            builder.Entity<User>().HasData(newUser);

            return newUser;
        }

        private static void AddUserToRole(ModelBuilder builder, User user, string roleName) {
            var role = builder.Model.FindEntityType(typeof(IdentityRole<Guid>))
                .GetSeedData().FirstOrDefault(sd => sd.Values.Contains(roleName.ToUpper()));

            if (role != null) {
                builder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid> {
                    UserId = user.Id,
                    RoleId = (Guid)role["Id"]
                });
            }
        }
    }
}
