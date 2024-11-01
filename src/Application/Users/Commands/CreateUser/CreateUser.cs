using System.Text.RegularExpressions;
using ASWISS.Application.Common.Interfaces;
using ASWISS.Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace ASWISS.Application.Users.Commands.CreateUser;

public record CreateUserCommand : IRequest<string>
{
    public required string Password { get; init; }
    public string? LastName { get; init; }
    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public string? PassportNumber { get; init; }
    public string? PlaceOfBirth { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? RegistrationAddress { get; init; }
    public string? ResidentialAddress { get; init; }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
{
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler
        (
        IMapper mapper,
        IIdentityService identityService,
        ILogger<CreateUserCommandHandler> logger
        )
    {
        _mapper = mapper;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var options = _mapper.Map<CreateUserOptions>(request);

        var result =  await _identityService.CreateUserAsync(options);

         if(result.Result.Succeeded)
        {
            return result.UserId.ToString();
        }
        var errorMessages = string.Join("; ", result.Result.Errors.Select(e => e));

        _logger.LogError("Ошибка при создании пользователя: {ErrorMessages}", errorMessages);

        throw new Exception($"Ошибка при создании пользователя: {errorMessages}");
    }
}
