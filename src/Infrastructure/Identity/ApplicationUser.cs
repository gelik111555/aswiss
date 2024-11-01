using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ASWISS.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    [MaxLength(50)]
    public string? LastName { get; set; }

    [MaxLength(50)]
    public string? FirstName { get; set; }

    [MaxLength(50)]
    public string? MiddleName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [RegularExpression(@"\d{4} \d{6}", ErrorMessage = "Номер паспорта должен быть в формате ХХХХ ХХХХХХ")]
    public string? PassportNumber { get; set; }

    [MaxLength(100)]
    public string? PlaceOfBirth { get; set; }

    [Phone]
    [RegularExpression(@"7\d{10}", ErrorMessage = "Телефон должен быть в формате 7XXXXXXXXXX")]
    public override string? PhoneNumber { get; set; }

    [MaxLength(200)]
    public string? RegistrationAddress { get; set; }

    [MaxLength(200)]
    public string? ResidentialAddress { get; set; }
}
