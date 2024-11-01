namespace ASWISS.Application.Users.Queries.GetUsers;

public class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        RuleFor(x => x)
            .Must(x => x.UserId != null || !string.IsNullOrEmpty(x.LastName) ||
                       !string.IsNullOrEmpty(x.FirstName) || !string.IsNullOrEmpty(x.MiddleName) ||
                       !string.IsNullOrEmpty(x.PhoneNumber) || !string.IsNullOrEmpty(x.Email))
            .WithMessage("Необходимо указать хотя бы один параметр для поиска.");

        RuleFor(x => x.LastName)
            .Matches(@"^[a-zA-Zа-яА-ЯёЁ]*$")
            .When(x => !string.IsNullOrEmpty(x.LastName))
            .WithMessage("Фамилия содержит недопустимые символы.");

        RuleFor(x => x.FirstName)
            .Matches(@"^[a-zA-Zа-яА-ЯёЁ]*$")
            .When(x => !string.IsNullOrEmpty(x.FirstName))
            .WithMessage("Имя содержит недопустимые символы.");

        RuleFor(x => x.MiddleName)
            .Matches(@"^[a-zA-Zа-яА-ЯёЁ]*$")
            .When(x => !string.IsNullOrEmpty(x.MiddleName))
            .WithMessage("Отчество содержит недопустимые символы.");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\d*$")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Номер телефона содержит недопустимые символы.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Некорректный формат email.");
    }
}
