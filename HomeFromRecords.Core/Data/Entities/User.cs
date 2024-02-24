using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static HomeFromRecords.Core.Data.Constants;

namespace HomeFromRecords.Core.Data.Entities {
    public class User : IdentityUser<Guid> {
        public User(string userName) : base(userName) { }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Region { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string Country { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
