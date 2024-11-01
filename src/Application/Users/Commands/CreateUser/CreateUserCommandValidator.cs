using Microsoft.AspNetCore.Http;

namespace ASWISS.Application.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HashSet<string> _mailRequiredFields = ["FirstName", "Email"];
    private readonly HashSet<string> _mobileRequiredFields = ["PhoneNumber"];
    private readonly HashSet<string> _webRequiredFields = ["FirstName", "LastName", "DateOfBirth", "PassportNumber", "PlaceOfBirth", "PhoneNumber", "RegistrationAddress"];

    public CreateUserCommandValidator(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;

        var deviceType = GetDeviceType();

        if (IsValidDeviceType(deviceType))
        {
            ApplyPasswordRules();
        }

        if (deviceType == "mail")
        {
            ApplyRulesForMail();
            AddExtraFieldsCheck(_mailRequiredFields);
        }
        else if (deviceType == "mobile")
        {
            ApplyRulesForMobile();
            AddExtraFieldsCheck(_mobileRequiredFields);
        }
        else if (deviceType == "web")
        {
            ApplyRulesForWeb();
            AddExtraFieldsCheck(_webRequiredFields);
        }
        else
        {
            // Если заголовок "x-Device" не указан или некорректен
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("Тип устройства не указан. Укажите допустимое значение заголовка 'x-Device'.");
        }
    }
    private void ApplyPasswordRules()
    {
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен для всех устройств.")
            .Matches(@"[A-Z]").WithMessage("Пароль должен содержать хотя бы одну заглавную букву.")
            .Matches(@"[a-z]").WithMessage("Пароль должен содержать хотя бы одну строчную букву.")
            .Matches(@"\d").WithMessage("Пароль должен содержать хотя бы одну цифру.")
            .Matches(@"[!@#\$%\^&\*]").WithMessage("Пароль должен содержать хотя бы один специальный символ.");
    }
    private bool IsValidDeviceType(string? deviceType)
    {
        return deviceType == "mail" || deviceType == "mobile" || deviceType == "web";
    }
    private void ApplyRulesForMail()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Имя обязательно для 'mail'.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email обязателен и должен быть корректным для 'mail'.");
    }

    private void ApplyRulesForMobile()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^7\d{10}$")
            .WithMessage("Телефон обязателен для 'mobile' и должен быть в формате 7XXXXXXXXXX.");
    }

    private void ApplyRulesForWeb()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Имя обязательно для 'web'.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Фамилия обязательна для 'web'.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .WithMessage("Дата рождения обязательна для 'web'.");

        RuleFor(x => x.PassportNumber)
            .NotEmpty()
            .Matches(@"^\d{4} \d{6}$")
            .WithMessage("Номер паспорта обязателен для 'web' и должен быть в формате ХХХХ ХХХХХХ.");

        RuleFor(x => x.PlaceOfBirth)
            .NotEmpty()
            .WithMessage("Место рождения обязательно для 'web'.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^7\d{10}$")
            .WithMessage("Телефон обязателен для 'web' и должен быть в формате 7XXXXXXXXXX.");

        RuleFor(x => x.RegistrationAddress)
            .NotEmpty()
            .WithMessage("Адрес регистрации обязателен для 'web'.");
    }

    private void AddExtraFieldsCheck(HashSet<string> allowedFields)
    {
        RuleFor(x => x).Custom((command, context) =>
        {
            // Добавляем поле "Password" в разрешенные поля, так как оно обязательно для всех устройств
            allowedFields.Add("Password");

            // Получаем список свойств команды, которые были установлены (не равны null)
            var commandProperties = command.GetType()
                .GetProperties()
                .Where(p => p.GetValue(command) != null)
                .Select(p => p.Name)
                .ToHashSet();

            // Ищем лишние поля, которые не относятся к текущему типу устройства
            var extraFields = commandProperties.Except(allowedFields);

            // Если лишние поля есть, добавляем ошибки валидации
            foreach (var field in extraFields)
            {
                context.AddFailure(field, $"Поле '{field}' не разрешено для устройства '{GetDeviceType()}'.");
            }
        });
    }


    private string? GetDeviceType()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        // Пытаемся получить заголовок "x-Device"
        if (httpContext != null && httpContext.Request.Headers.TryGetValue("x-Device", out var device))
        {
            return device.ToString().Trim();
        }

        return null;
    }
}
