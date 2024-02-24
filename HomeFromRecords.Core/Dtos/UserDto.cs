using static HomeFromRecords.Core.Data.Constants;

namespace HomeFromRecords.Core.Dtos {
    public record UserDto(
        Guid UserId,
        string FirstName,
        string LastName
        ) {
        public string FullName => $"{FirstName} {LastName}";
    };

    public record UserLoginDto(
        string Email,
        string Password
    );

    public record UserRegisterDto(
        string UserName,
        string Password,
        string FirstName,
        string LastName,
        string Email,
        string Phone,
        string StreetAddress,
        string City,
        string Region,
        string PostalCode,
        string Country
    );

    public record UserUpdateDto(
        string UserId,
        string? UserName,
        string? Password,
        string? FirstName,
        string? LastName,
        string? Email,
        string? Phone,
        string? StreetAddress,
        string? City,
        string? Region,
        string? PostalCode,
        string? Country
    );
}
