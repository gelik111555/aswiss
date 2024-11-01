using ASWISS.Application.Common.Interfaces;

namespace ASWISS.Application.Users.Queries.GetUsers;

public record GetUsersQuery : IRequest<List<UserDto>>
{
    public Guid? UserId { get; init; } // Поиск по ID
    public string? LastName { get; init; } // Поиск по фамилии
    public string? FirstName { get; init; } // Поиск по имени
    public string? MiddleName { get; init; } // Поиск по отчеству
    public string? PhoneNumber { get; init; } // Поиск по телефону
    public string? Email { get; init; } // Поиск по email
}

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly IIdentityService _identityService;

    public GetUsersQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
       return await _identityService.FindUsersAsync(
            request.UserId,
            request.LastName,
            request.FirstName,
            request.MiddleName,
            request.PhoneNumber,
            request.Email);
    }
}
