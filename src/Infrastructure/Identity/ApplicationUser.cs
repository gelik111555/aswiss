using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ASWISS.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(50)]
    public string? LastName { get; set; }

    [Required]
    [MaxLength(50)]
    public string? FirstName { get; set; }

    [MaxLength(50)]
    public string? MiddleName { get; set; }

    [Required]
    public DateTime? DateOfBirth { get; set; }

    [Required]
    [RegularExpression(@"\d{4} \d{6}", ErrorMessage = "Номер паспорта должен быть в формате ХХХХ ХХХХХХ")]
    public string? PassportNumber { get; set; }

    [Required]
    [MaxLength(100)]
    public string? PlaceOfBirth { get; set; }

    [Required]
    [Phone]
    [RegularExpression(@"7\d{10}", ErrorMessage = "Телефон должен быть в формате 7XXXXXXXXXX")]
    public override string? PhoneNumber { get; set; }

    [Required]
    [MaxLength(200)]
    public string? RegistrationAddress { get; set; }

    [Required]
    [MaxLength(200)]
    public string? ResidentialAddress { get; set; }
}
