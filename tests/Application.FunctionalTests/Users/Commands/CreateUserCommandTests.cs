using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using ASWISS.Application.Common.Exceptions;
using ASWISS.Application.Users.Commands.CreateUser;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ASWISS.Application.FunctionalTests.Users.Commands;

using static Testing;

internal class CreateUserCommandTests : BaseTestFixture
{
    [Test]
    public async Task ShouldCreateUser_WithMailDevice()
    {
        var password = GenerateRandomPassword(10);
        var createUserCommand = new CreateUserCommand { Password = password, Email = "testemail@mail.com", FirstName = "test" };

        var headers = new Dictionary<string, string>
            {
                { "x-Device", "mail" }
            };


        var result = await SendAsync(createUserCommand, headers);
        var guidUserId = Guid.Parse(result);
        Assert.That(guidUserId, Is.TypeOf<Guid>(), "Expected result to be of type Guid.");
        Assert.That(guidUserId, Is.Not.EqualTo(Guid.Empty), "Expected result to be a non-empty Guid.");

    }

    [Test]
    public async Task ShouldCreateUser_WithMobileDevice()
    {
        var password = GenerateRandomPassword(10);
        var createUserCommand = new CreateUserCommand { Password = password, PhoneNumber = "71234567890" };

        var headers = new Dictionary<string, string>
            {
                { "x-Device", "mobile" }
            };

        var result = await SendAsync(createUserCommand, headers);
        var guidUserId = Guid.Parse(result);
        Assert.That(guidUserId, Is.TypeOf<Guid>(), "Expected result to be of type Guid.");
        Assert.That(guidUserId, Is.Not.EqualTo(Guid.Empty), "Expected result to be a non-empty Guid.");

    }

    [Test]
    public async Task ShouldCreateUser_WithWebDevice()
    {
        var password = GenerateRandomPassword(10);
        var createUserCommand = new CreateUserCommand
        {
            Password = password,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            PassportNumber = "1234 567890",
            PlaceOfBirth = "New York",
            PhoneNumber = "71234567890",
            RegistrationAddress = "123 Main St"
        };

        var headers = new Dictionary<string, string>
            {
                { "x-Device", "web" }
            };


        var result = await SendAsync(createUserCommand, headers);

        var guidUserId = Guid.Parse(result);
        Assert.That(guidUserId, Is.TypeOf<Guid>(), "Expected result to be of type Guid.");
        Assert.That(guidUserId, Is.Not.EqualTo(Guid.Empty), "Expected result to be a non-empty Guid.");

    }

    [Test]
    public void ShouldFail_MissingEmailForMailDevice()
    {
        var password = GenerateRandomPassword(10);
        var createUserCommand = new CreateUserCommand { Password = password, FirstName = "test" };

        var headers = new Dictionary<string, string>
    {
        { "x-Device", "mail" }
    };

        var exception = Assert.ThrowsAsync<ValidationException>(async () =>
        {
            await SendAsync(createUserCommand, headers);
        });

        Assert.That(exception.Errors, Is.Not.Empty);
        Assert.That(exception.Errors.Any(e => e.Key == "Email"), Is.True, "'Email' должно быть заполнено.");
    }

    [Test]
    public void ShouldFail_ExtraFieldForMailDevice()
    {
        var password = GenerateRandomPassword(10);
        var createUserCommand = new CreateUserCommand
        {
            Password = password,
            Email = "testemail@mail.com",
            FirstName = "test",
            PhoneNumber = "71234567890" // Лишнее поле для устройства "mail"
        };

        var headers = new Dictionary<string, string>
    {
        { "x-Device", "mail" }
    };

        var exception = Assert.ThrowsAsync<ValidationException>(async () =>
        {
            await SendAsync(createUserCommand, headers);
        });

        Assert.That(exception.Errors, Is.Not.Empty);
        Assert.That(exception.Errors.Any(e => e.Key == "PhoneNumber"), Is.True, "Поле 'PhoneNumber' не разрешено для устройства 'mail'.");
    }

    [Test]
    public  void ShouldFail_MissingPhoneNumberForMobileDevice()
    {
        var password = GenerateRandomPassword(10);
        var createUserCommand = new CreateUserCommand { Password = password };

        var headers = new Dictionary<string, string>
    {
        { "x-Device", "mobile" }
    };

        var exception = Assert.ThrowsAsync<ValidationException>(async () =>
        {
            await SendAsync(createUserCommand, headers);
        });

        Assert.That(exception.Errors, Is.Not.Empty);
        Assert.That(exception.Errors.Any(e => e.Key == "PhoneNumber"), Is.True, "'Phone Number' должно быть заполнено.");
    }

    [Test]
    public void ShouldFail_ExtraFieldForMobileDevice()
    {
        var password = GenerateRandomPassword(10);
        var createUserCommand = new CreateUserCommand
        {
            Password = password,
            PhoneNumber = "71234567890",
            Email = "extra@mail.com" // Лишнее поле для устройства "mobile"
        };

        var headers = new Dictionary<string, string>
    {
        { "x-Device", "mobile" }
    };

        var exception = Assert.ThrowsAsync<ValidationException>(async () =>
        {
            await SendAsync(createUserCommand, headers);
        });

        Assert.That(exception.Errors, Is.Not.Empty);
        Assert.That(exception.Errors.Any(e => e.Key == "Email"), Is.True, "Поле 'Email' не разрешено для устройства 'mobile'.");
    }

    [Test]
    public void ShouldFail_MissingRequiredFieldsForWebDevice()
    {
        var password = GenerateRandomPassword(10);
        var createUserCommand = new CreateUserCommand
        {
            Password = password,
            FirstName = "John"
        };

        var headers = new Dictionary<string, string>
    {
        { "x-Device", "web" }
    };

        var exception = Assert.ThrowsAsync<ValidationException>(async () =>
        {
            await SendAsync(createUserCommand, headers);
        });

        Assert.That(exception.Errors, Is.Not.Empty);
        Assert.That(exception.Errors.Any(e => e.Key == "LastName"), Is.True, "Фамилия обязательна для 'web'.");
        Assert.That(exception.Errors.Any(e => e.Key == "DateOfBirth"), Is.True, "Дата рождения обязательна для 'web'.");
        Assert.That(exception.Errors.Any(e => e.Key == "PassportNumber"), Is.True, "'Passport Number' должно быть заполнено.");
        Assert.That(exception.Errors.Any(e => e.Key == "PlaceOfBirth"), Is.True, "Место рождения обязательно для 'web'.");
        Assert.That(exception.Errors.Any(e => e.Key == "PhoneNumber"), Is.True, "'Phone Number' должно быть заполнено.");
        Assert.That(exception.Errors.Any(e => e.Key == "RegistrationAddress"), Is.True, "Адрес регистрации обязателен для 'web'.");
    }

    [Test]
    public void ShouldFail_ExtraFieldForWebDevice()
    {
        var password = GenerateRandomPassword(10);
        var createUserCommand = new CreateUserCommand
        {
            Password = password,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            PassportNumber = "1234 567890",
            PlaceOfBirth = "New York",
            PhoneNumber = "71234567890",
            RegistrationAddress = "123 Main St",
            ResidentialAddress = "456 Another St", // Лишнее поле для устройства "web"
            Email = "extra@mail.com" // Лишнее поле для устройства "web"
        };

        var headers = new Dictionary<string, string>
    {
        { "x-Device", "web" }
    };

        var exception = Assert.ThrowsAsync<ValidationException>(async () =>
        {
            await SendAsync(createUserCommand, headers);
        });

        Assert.That(exception.Errors, Is.Not.Empty);
        Assert.That(exception.Errors.Any(e => e.Key == "Email"), Is.True, "Поле 'Email' не разрешено для устройства 'web'.");
        Assert.That(exception.Errors.Any(e => e.Key == "ResidentialAddress"), Is.True, "Поле 'ResidentialAddress' не разрешено для устройства 'web'.");
    }
   
    [Test]
    public void ShouldFail_WhenDeviceNotSpecified()
    {
        var password = GenerateRandomPassword(10);
        var createUserCommand = new CreateUserCommand { Password = password, Email = "testemail@mail.com" };

        var exception = Assert.ThrowsAsync<ValidationException>(async () =>
        {
            await SendAsync(createUserCommand);
        });

        Assert.That(exception, Is.Not.Null, "Expected validation exception due to invalid x-Device header value.");
        Assert.That(exception.Errors, Is.Not.Empty, "Expected validation errors in exception.");

        // Проверяем, что конкретное сообщение об ошибке содержит ожидаемый текст
        var deviceTypeError = exception.Errors.FirstOrDefault(e => e.Key == "FirstName").Value?.FirstOrDefault();
        Assert.That(deviceTypeError, Is.EqualTo("Тип устройства не указан. Укажите допустимое значение заголовка 'x-Device'."));

    }

    [Test]
    public  void ShouldFail_WhenDeviceIsInvalid()
    {
        var password = GenerateRandomPassword(10);
        var createUserCommand = new CreateUserCommand { Password = password, Email = "testemail@mail.com" };

        var headers = new Dictionary<string, string>
    {
        { "x-Device", "invalid-device" }
    };

        var exception = Assert.ThrowsAsync<ValidationException>(async () =>
        {
            await SendAsync(createUserCommand, headers);
        });

        Assert.That(exception, Is.Not.Null, "Expected validation exception due to invalid x-Device header value.");
        Assert.That(exception.Errors, Is.Not.Empty, "Expected validation errors in exception.");

        // Проверяем, что конкретное сообщение об ошибке содержит ожидаемый текст
        var deviceTypeError = exception.Errors.FirstOrDefault(e => e.Key == "FirstName").Value?.FirstOrDefault();
        Assert.That(deviceTypeError, Is.EqualTo("Тип устройства не указан. Укажите допустимое значение заголовка 'x-Device'."));
    }

    [Test]
    public async Task ShouldCreateUser_WithValidPassword_ForMailDevice()
    {
        var password = GenerateRandomPassword(10); // Содержит заглавные и строчные буквы, цифры и спецсимволы
        var createUserCommand = new CreateUserCommand { Password = password, Email = "testemail@mail.com", FirstName = "test" };

        var headers = new Dictionary<string, string>
        {
            { "x-Device", "mail" }
        };

        var result = await SendAsync(createUserCommand, headers);
        var guidUserId = Guid.Parse(result);
        Assert.That(guidUserId, Is.TypeOf<Guid>(), "Expected result to be of type Guid.");
        Assert.That(guidUserId, Is.Not.EqualTo(Guid.Empty), "Expected result to be a non-empty Guid.");
    }

    [Test]
    public void ShouldFail_WhenPasswordMissingSpecialCharacter()
    {
        var createUserCommand = new CreateUserCommand
        {
            Password = "ValidPass123", // Нет спецсимволов
            Email = "testemail@mail.com",
            FirstName = "test"
        };

        var headers = new Dictionary<string, string>
    {
        { "x-Device", "mail" }
    };

        var exception = Assert.ThrowsAsync<ValidationException>(async () =>
        {
            await SendAsync(createUserCommand, headers);
        });

        Assert.That(exception.Errors, Is.Not.Empty);
        Assert.That(exception.Errors.Any(e => e.Key == "Password"), Is.True, "Пароль должен содержать хотя бы один специальный символ.");
    }

    [Test]
    public void ShouldFail_WhenPasswordMissingDigit()
    {
        var createUserCommand = new CreateUserCommand
        {
            Password = "ValidPass!", // Нет цифр
            Email = "testemail@mail.com",
            FirstName = "test"
        };

        var headers = new Dictionary<string, string>
    {
        { "x-Device", "mail" }
    };

        var exception = Assert.ThrowsAsync<ValidationException>(async () =>
        {
            await SendAsync(createUserCommand, headers);
        });

        Assert.That(exception.Errors, Is.Not.Empty);
        Assert.That(exception.Errors.Any(e => e.Key == "Password"), Is.True, "Пароль должен содержать хотя бы одну цифру.");
    }
}
