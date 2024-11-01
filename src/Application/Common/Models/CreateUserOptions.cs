using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASWISS.Application.Users.Commands.CreateUser;

namespace ASWISS.Application.Common.Models;
public class CreateUserOptions
{
    public required string Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? PassportNumber { get; set; }
    public string? PlaceOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? RegistrationAddress { get; set; }
    public string? ResidentialAddress { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateUserCommand, CreateUserOptions>();
        }
    }
}
