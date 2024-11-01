namespace ASWISS.Application.FunctionalTests.Users.Queries;

using ASWISS.Application.Common.Exceptions;
using ASWISS.Application.Users.Commands.CreateUser;
using ASWISS.Application.Users.Queries.GetUsers;
using static Testing;
internal class GetUsersQueryTests : BaseTestFixture
{
    [Test]
    public void ShouldReturnValidationError_WhenNoSearchParameterProvided()
    {
        // Arrange
        var query = new GetUsersQuery();

        // Act
        var exception = Assert.ThrowsAsync<ValidationException>(async () =>
        {
            await SendAsync(query);
        });

        // Assert
        Assert.That(exception, Is.Not.Null, "Expected validation exception due to missing search parameters.");
        Assert.That(exception.Errors, Is.Not.Empty, "Expected validation errors in exception.");
        Assert.That(exception.Errors.Any(e => e.Value.Contains("Необходимо указать хотя бы один параметр для поиска.")), Is.True);
    }

    [Test]
    public void ShouldReturnValidationError_ForInvalidLastNameFormat()
    {
        // Arrange
        var query = new GetUsersQuery { LastName = "123InvalidName" };

        // Act
        var exception = Assert.ThrowsAsync<ValidationException>(async () =>
        {
            await SendAsync(query);
        });

        // Assert
        Assert.That(exception, Is.Not.Null, "Expected validation exception for invalid last name format.");
        Assert.That(exception.Errors, Is.Not.Empty, "Expected validation errors in exception.");
        Assert.That(exception.Errors.Any(e => e.Key == "LastName" && e.Value.Contains("Фамилия содержит недопустимые символы.")), Is.True);
    }

    [Test]
    public void ShouldReturnValidationError_ForInvalidFirstNameFormat()
    {
        // Arrange
        var query = new GetUsersQuery { FirstName = "123InvalidName" };

        // Act
        var exception = Assert.ThrowsAsync<ValidationException>(async () =>
        {
            await SendAsync(query);
        });

        // Assert
        Assert.That(exception, Is.Not.Null, "Expected validation exception for invalid first name format.");
        Assert.That(exception.Errors, Is.Not.Empty, "Expected validation errors in exception.");
        Assert.That(exception.Errors.Any(e => e.Key == "FirstName" && e.Value.Contains("Имя содержит недопустимые символы.")), Is.True);
    }

    [Test]
    public void ShouldReturnValidationError_ForInvalidMiddleNameFormat()
    {
        // Arrange
        var query = new GetUsersQuery { MiddleName = "123InvalidName" };

        // Act
        var exception = Assert.ThrowsAsync<ValidationException>(async () =>
        {
            await SendAsync(query);
        });

        // Assert
        Assert.That(exception, Is.Not.Null, "Expected validation exception for invalid middle name format.");
        Assert.That(exception.Errors, Is.Not.Empty, "Expected validation errors in exception.");
        Assert.That(exception.Errors.Any(e => e.Key == "MiddleName" && e.Value.Contains("Отчество содержит недопустимые символы.")), Is.True);
    }

    [Test]
    public void ShouldReturnValidationError_ForInvalidPhoneNumberFormat()
    {
        // Arrange
        var query = new GetUsersQuery { PhoneNumber = "invalidPhoneNumber" };

        // Act
        var exception = Assert.ThrowsAsync<ValidationException>(async () =>
        {
            await SendAsync(query);
        });

        // Assert
        Assert.That(exception, Is.Not.Null, "Expected validation exception for invalid phone number format.");
        Assert.That(exception.Errors, Is.Not.Empty, "Expected validation errors in exception.");
        Assert.That(exception.Errors.Any(e => e.Key == "PhoneNumber" && e.Value.Contains("Номер телефона содержит недопустимые символы.")), Is.True);
    }

    [Test]
    public void ShouldReturnValidationError_ForInvalidEmailFormat()
    {
        // Arrange
        var query = new GetUsersQuery { Email = "invalidEmailFormat" };

        // Act
        var exception = Assert.ThrowsAsync<ValidationException>(async () =>
        {
            await SendAsync(query);
        });

        // Assert
        Assert.That(exception, Is.Not.Null, "Expected validation exception for invalid email format.");
        Assert.That(exception.Errors, Is.Not.Empty, "Expected validation errors in exception.");
        Assert.That(exception.Errors.Any(e => e.Key == "Email" && e.Value.Contains("Некорректный формат email.")), Is.True);
    }

    [Test]
    public async Task ShouldPassValidation_WhenValidParametersProvided()
    {
        // Arrange
        var query = new GetUsersQuery
        {
            LastName = "ValidLastName",
            FirstName = "ValidFirstName",
            MiddleName = "ValidMiddleName",
            PhoneNumber = "71234567890",
            Email = "test@example.com"
        };

        // Act
        var result = await FluentActions.Invoking(() => SendAsync(query)).Invoke();

        // Assert
        Assert.That(result, Is.Not.Null, "Expected non-null result for valid query.");
    }

    [Test]
    public async Task ShouldRetrieveUserByEmail_WhenUserExists()
    {
        // Arrange
        var password = GenerateRandomPassword(10);
        var createUserCommand = new CreateUserCommand
        {
            Password = password,
            Email = "testemail@mail.com",
            FirstName = "test"
        };

        var headers = new Dictionary<string, string>
        {
            { "x-Device", "mail" }
        };

        // Создаем пользователя
        await SendAsync(createUserCommand, headers);

        var query = new GetUsersQuery
        {
            Email = "testemail@mail.com"
        };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null, "Expected to retrieve a non-null collection of users.");
        Assert.That(result, Is.Not.Empty, "Expected to retrieve at least one user for the given email.");
        Assert.That(result.Any(u => u.Email == "testemail@mail.com"), Is.True, "Expected at least one user with the specified email.");
        Assert.That(result.Any(u => u.FirstName == "test"), Is.True, "Expected at least one user with the specified first name.");

    }

    [Test]
    public async Task ShouldRetrieveMultipleUsersByFirstName_WhenUsersExist()
    {
        // Arrange
        var password1 = GenerateRandomPassword(10);
        var password2 = GenerateRandomPassword(10);

        var createUserCommand1 = new CreateUserCommand
        {
            Password = password1,
            Email = "user1@mail.com",
            FirstName = "test"
        };

        var createUserCommand2 = new CreateUserCommand
        {
            Password = password2,
            Email = "user2@mail.com",
            FirstName = "test"
        };

        var headers = new Dictionary<string, string>
    {
        { "x-Device", "mail" }
    };

        // Создаем двух пользователей с одинаковым FirstName
        await SendAsync(createUserCommand1, headers);
        await SendAsync(createUserCommand2, headers);

        var query = new GetUsersQuery
        {
            FirstName = "test"
        };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null, "Expected to retrieve a non-null collection of users.");
        Assert.That(result.Count, Is.GreaterThanOrEqualTo(2), "Expected to retrieve at least two users with the specified first name.");
        Assert.That(result.Any(u => u.Email == "user1@mail.com"), Is.True, "Expected at least one user with the email 'user1@mail.com'.");
        Assert.That(result.Any(u => u.Email == "user2@mail.com"), Is.True, "Expected at least one user with the email 'user2@mail.com'.");
        Assert.That(result.All(u => u.FirstName == "test"), Is.True, "Expected all retrieved users to have the specified first name.");
    }

    [Test]
    public async Task ShouldRetrieveSingleUserByFirstNameAndLastName_WhenExactMatchExists()
    {
        // Arrange
        var password1 = GenerateRandomPassword(10);
        var password2 = GenerateRandomPassword(10);

        var createUserCommand1 = new CreateUserCommand
        {
            Password = password1,
            Email = "user1@mail.com",
            FirstName = "John",
        };

        var createUserCommand2 = new CreateUserCommand
        {
            Password = password2,
            Email = "user2@mail.com",
            FirstName = "John",
        };

        var headers = new Dictionary<string, string>
    {
        { "x-Device", "mail" }
    };

        // Создаем двух пользователей с одинаковым FirstName, но разными LastName
        await SendAsync(createUserCommand1, headers);
        await SendAsync(createUserCommand2, headers);

        var query = new GetUsersQuery
        {
            FirstName = "John",
            Email = "user1@mail.com"
        };

        // Act
        var result = await SendAsync(query);

        // Assert
        Assert.That(result, Is.Not.Null, "Expected to retrieve a non-null collection of users.");
        Assert.That(result.Count, Is.EqualTo(1), "Expected to retrieve exactly one user.");
        Assert.That(result.Any(u => u.Email == "user1@mail.com" && u.FirstName == "John"), Is.True, "Expected to retrieve the user with email 'user1@mail.com' and first name 'John'.");
    }


    [Test]
    public async Task ShouldReturnEmptyResult_WhenNoUsersMatchQueryParameters()
    {
        var query = new GetUsersQuery { FirstName = "NonExistentName" };

        var result = await SendAsync(query);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty, "Expected empty result when no users match the query parameters.");
    }

    [Test]
    public async Task ShouldRetrieveUserByPhoneNumber_WhenUserExists()
    {
        var password = GenerateRandomPassword(10);
        var phoneNumber = "71234567890";

        var createUserCommand = new CreateUserCommand { Password = password, PhoneNumber = phoneNumber };
        var headers = new Dictionary<string, string> { { "x-Device", "mobile" } };

        await SendAsync(createUserCommand, headers);

        var query = new GetUsersQuery { PhoneNumber = phoneNumber };

        var result = await SendAsync(query);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1), "Expected to retrieve exactly one user by phone number.");
        Assert.That(result.First().PhoneNumber, Is.EqualTo(phoneNumber), "Expected retrieved user's phone number to match the query.");
    }

    [Test]
    public async Task ShouldRetrieveUserById_WhenUserExists()
    {
        var password = GenerateRandomPassword(10);
        var createUserCommand = new CreateUserCommand { Password = password, Email = "testuser@mail.com", FirstName = "John" };
        var headers = new Dictionary<string, string> { { "x-Device", "mail" } };

        var userId = await SendAsync(createUserCommand, headers);

        var query = new GetUsersQuery { UserId = Guid.Parse(userId) };

        var result = await SendAsync(query);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1), "Expected to retrieve exactly one user by ID.");
        Assert.That(result.First().Id, Is.EqualTo(Guid.Parse(userId)), "Expected retrieved user's ID to match the query.");
    }

    [Test]
    public async Task ShouldRetrieveUsersByPartialMatch_WhenUsingNameField()
    {
        var password = GenerateRandomPassword(10);

        var createUserCommand1 = new CreateUserCommand { Password = password, Email = "user1@mail.com", FirstName = "John" };
        var createUserCommand2 = new CreateUserCommand { Password = password, Email = "user2@mail.com", FirstName = "Johnny" };
        var headers = new Dictionary<string, string> { { "x-Device", "mail" } };

        await SendAsync(createUserCommand1, headers);
        await SendAsync(createUserCommand2, headers);

        var query = new GetUsersQuery { FirstName = "Jo" };

        var result = await SendAsync(query);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2), "Expected to retrieve two users with partial name match 'Jo'.");
    }

    [Test]
    public void ShouldFail_WhenInvalidPhoneNumberFormatProvided()
    {
        var query = new GetUsersQuery { PhoneNumber = "InvalidPhoneNumber" };

        var exception = Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(query));
        Assert.That(exception, Is.Not.Null, "Expected validation exception due to invalid phone number format.");
        Assert.That(exception.Errors.Any(e => e.Key == "PhoneNumber"), Is.True, "Expected validation error for invalid phone number format.");
    }

    [Test]
    public void ShouldFail_WhenInvalidEmailFormatProvided()
    {
        var query = new GetUsersQuery { Email = "invalid-email" };

        var exception = Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(query));
        Assert.That(exception, Is.Not.Null, "Expected validation exception due to invalid email format.");
        Assert.That(exception.Errors.Any(e => e.Key == "Email"), Is.True, "Expected validation error for invalid email format.");
    }

    [Test]
    public async Task ShouldRetrieveUsersByMultipleFields_WhenAllFieldsMatch()
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


        await SendAsync(createUserCommand, headers);

        var query = new GetUsersQuery
        {
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "71234567890"
        };

        var result = await SendAsync(query);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1), "Expected to retrieve exactly one user when all fields match.");
    }

    [Test]
    public async Task ShouldRetrieveMultipleUsers_WhenDuplicateRecordsExist()
    {
        var password = GenerateRandomPassword(10);

        var createUserCommand1 = new CreateUserCommand { Password = password, Email = "user1@mail.com", FirstName = "John" };
        var createUserCommand2 = new CreateUserCommand { Password = password, Email = "user2@mail.com", FirstName = "John" };
        var headers = new Dictionary<string, string> { { "x-Device", "mail" } };

        await SendAsync(createUserCommand1, headers);
        await SendAsync(createUserCommand2, headers);

        var query = new GetUsersQuery { FirstName = "John" };

        var result = await SendAsync(query);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2), "Expected to retrieve two users with the same first name.");
    }
}
