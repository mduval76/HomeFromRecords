using HomeFromRecords.Core.Data.Entities;
using HomeFromRecords.Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HomeFromRecords.Core.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IConfiguration _configuration;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole<Guid>> roleManager, IConfiguration configuration) {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile() {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            if (user == null)
                return NotFound();

            var userDetails = new UserDto(
                user.Id,
                user.FirstName,
                user.LastName
            );

            return Ok(userDetails);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromForm] UserRegisterDto userDetails) {
            var user = new User(userDetails.UserName) {
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                Email = userDetails.Email,
                PhoneNumber = userDetails.Phone,
                StreetAddress = userDetails.StreetAddress,
                City = userDetails.City,
                Region = userDetails.Region,
                PostalCode = userDetails.PostalCode,
                Country = userDetails.Country
            };

            var result = await _userManager.CreateAsync(user, userDetails.Password);

            if (!result.Succeeded) {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "Failed to register user.", errors });
            }

            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var adminExists = admins.Any();
            var roleToAssign = adminExists ? "Member" : "Admin";

            await _userManager.AddToRoleAsync(user, roleToAssign);

            return Ok(userDetails);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUser([FromForm] UserLoginDto userDetails) {
            var user = await _userManager.FindByEmailAsync(userDetails.Email);

            if (user == null) {
                return BadRequest("Failed to login user.");
            }

            var result = await _userManager.CheckPasswordAsync(user, userDetails.Password);

            if (!result) {
                return BadRequest("Failed to login user.");
            }

            bool isInitialAdmin = false;
            if (userDetails.Email.Equals("admin@default.com")) {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                if (admins.Count == 1 && admins.First().Email == "admin@default.com") {
                    isInitialAdmin = true;
                }
            }

            var token = await GenerateJwtToken(user);

            return Ok(new {
                Token = token,
                Message = "Login successful",
                IsInitialAdmin = isInitialAdmin,
                UserId = isInitialAdmin ? user.Id : Guid.Empty
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> LogoutUser() {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromForm] UserUpdateDto userDetails) {
            if (!Guid.TryParse(userDetails.UserId, out Guid userId)) {
                return BadRequest("Invalid user ID.");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(userDetails.UserName)) {
                user.UserName = userDetails.UserName;
            }

            if (!string.IsNullOrWhiteSpace(userDetails.FirstName)) {
                user.FirstName = userDetails.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(userDetails.LastName)) {
                user.LastName = userDetails.LastName;
            }

            if (!string.IsNullOrWhiteSpace(userDetails.Email)) {
                user.Email = userDetails.Email;
            }

            if (!string.IsNullOrWhiteSpace(userDetails.Phone)) {
                user.PhoneNumber = userDetails.Phone;
            }

            if (!string.IsNullOrWhiteSpace(userDetails.StreetAddress)) {
                user.StreetAddress = userDetails.StreetAddress;
            }

            if (!string.IsNullOrWhiteSpace(userDetails.City)) {
                user.City = userDetails.City;
            }

            if (!string.IsNullOrWhiteSpace(userDetails.Region)) {
                user.Region = userDetails.Region;
            }

            if (!string.IsNullOrWhiteSpace(userDetails.PostalCode)) {
                user.PostalCode = userDetails.PostalCode;
            }

            if (!string.IsNullOrWhiteSpace(userDetails.Country)) {
                user.Country = userDetails.Country;
            }

            if (!string.IsNullOrWhiteSpace(userDetails.Password)) {
                var removeResult = await _userManager.RemovePasswordAsync(user);
                if (!removeResult.Succeeded) {
                    return BadRequest("Failed to reset password.");
                }

                var addResult = await _userManager.AddPasswordAsync(user, userDetails.Password);
                if (!addResult.Succeeded) {
                    return BadRequest(new { message = "Failed to update password.", errors = addResult.Errors.Select(e => e.Description) });
                }
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) {
                return BadRequest(new { message = "Failed to update user profile.", errors = result.Errors.Select(e => e.Description) });
            }

            return Ok(new {
                UserDetails = userDetails,
                IsInitialAdmin = false
            });
        }

        // Helper methods
        private async Task<string> GenerateJwtToken(User user) {
            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roleClaims = await _userManager.GetRolesAsync(user);
            foreach (var role in roleClaims) {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddHours(1);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
