using ASWISS.Application.Common.Interfaces;
using ASWISS.Application.Common.Models;
using ASWISS.Application.Users.Queries.GetUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASWISS.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
    }

    public async Task<List<UserDto>> FindUsersAsync(Guid? userId = null, string? lastName = null, string? firstName = null,
        string? middleName = null, string? phoneNumber = null, string? email = null)
    {
        var query = _userManager.Users.AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(u => u.Id == userId.Value.ToString());
        }
        if (!string.IsNullOrEmpty(lastName))
        {
            query = query.Where(u => u.LastName != null && u.LastName.Contains(lastName));
        }

        if (!string.IsNullOrEmpty(firstName))
        {
            query = query.Where(u => u.FirstName != null && u.FirstName.Contains(firstName));
        }

        if (!string.IsNullOrEmpty(middleName))
        {
            query = query.Where(u => u.MiddleName != null && u.MiddleName.Contains(middleName));
        }

        if (!string.IsNullOrEmpty(phoneNumber))
        {
            query = query.Where(u => u.PhoneNumber != null && u.PhoneNumber.Contains(phoneNumber));
        }

        if (!string.IsNullOrEmpty(email))
        {
            query = query.Where(u => u.Email != null && u.Email.Contains(email));
        }

        var users = await query
            .Select(u => new UserDto
            {
                Id = Guid.Parse(u.Id),
                FirstName = u.FirstName,
                LastName = u.LastName,
                MiddleName = u.MiddleName,
                PhoneNumber = u.PhoneNumber,
                Email = u.Email
            })
            .ToListAsync();

        return users;
    }
    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

        return user.UserName;
    }
    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }
    public async Task<(Result Result, string UserId)> CreateUserAsync(CreateUserOptions options)
    {
        // Заполняем данные пользователя на основе переданных параметров
        var user = new ApplicationUser
        {
            UserName = GenerateUserName(options),
            Email = GenerateEmail(options),
            FirstName = options.FirstName,
            LastName = options.LastName,
            MiddleName = options.MiddleName,
            DateOfBirth = options.DateOfBirth,
            PassportNumber = options.PassportNumber,
            PlaceOfBirth = options.PlaceOfBirth,
            PhoneNumber = options.PhoneNumber,
            RegistrationAddress = options.RegistrationAddress,
            ResidentialAddress = options.ResidentialAddress
        };


        var result = await _userManager.CreateAsync(user, options.Password);

        return (result.ToApplicationResult(), user.Id);
    }
    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }
    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }
    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }
    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }
    private string GenerateUserName(CreateUserOptions options)
    {
        // Если `Email` задан, используем часть перед `@` в качестве `UserName`
        if (!string.IsNullOrEmpty(options.Email))
        {
            if (options.Email.Contains('@'))
                return options.Email.Split('@')[0];

            else return options.Email;
        }


        // Если `PhoneNumber` задан, создаем `UserName` из номера телефона
        if (!string.IsNullOrEmpty(options.PhoneNumber))
        {
            return "user_" + options.PhoneNumber;
        }

        // В крайнем случае генерируем случайное имя пользователя
        return "user_" + Guid.NewGuid().ToString("N").Substring(0, 8);
    }
    private string GenerateEmail(CreateUserOptions options)
    {
        // Если `Email` задан, используем его
        if (!string.IsNullOrEmpty(options.Email))
        {
            return options.Email;
        }

        // Если `PhoneNumber` задан, создаем `Email` на основе номера телефона
        if (!string.IsNullOrEmpty(options.PhoneNumber))
        {
            return $"{options.PhoneNumber}@example.com"; // Пример: 71234567890@example.com
        }

        // В крайнем случае генерируем временный email
        return $"user_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
    }

}
