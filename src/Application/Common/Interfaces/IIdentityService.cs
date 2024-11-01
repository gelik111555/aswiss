using ASWISS.Application.Common.Models;
using ASWISS.Application.Users.Queries.GetUsers;

namespace ASWISS.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

    Task<(Result Result, string UserId)> CreateUserAsync(CreateUserOptions options);
    public Task<List<UserDto>> FindUsersAsync(Guid? userId = null, string? lastName = null, string? firstName = null,
        string? middleName = null, string? phoneNumber = null, string? email = null);
    Task<Result> DeleteUserAsync(string userId);
}
