using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static HomeFromRecords.Core.Data.Constants;

namespace HomeFromRecords.Core.Data.Entities {
    public class User(string userName) : IdentityUser<Guid>(userName) {
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        [Required]
        public string StreetAddress { get; set; } = null!;
        [Required]
        public string City { get; set; } = null!;
        [Required]
        public string Region { get; set; } = null!;
        [Required]
        public string PostalCode { get; set; } = null!;
        [Required]
        public string Country { get; set; } = null!;

        public string FullName => $"{FirstName} {LastName}";
    }
}
