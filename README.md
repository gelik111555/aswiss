
# ASWISS

This is a test project for ASWISS, implementing a web service for managing user accounts with the following functionality:

1. User creation
2. Retrieval of user by ID
3. Search for a user by one or more fields: last name, first name, middle name, phone, email

## User Attributes
- Id
- Last Name
- First Name
- Middle Name
- Date of Birth
- Passport Number (including series in the format XXXX XXXXXX)
- Place of Birth
- Phone (format 7XXXXXXXXXX)
- Email
- Registration Address
- Residential Address

Users can be created from different applications. Depending on the application, the validation logic for fields during account creation varies. The application type is determined by the mandatory HTTP header "x-Device".

### HTTP Header Values and Field Validation Rules
- **mail** - only First Name and Email are required
- **mobile** - only Phone Number is required
- **web** - all fields except Email and Residential Address are required

## Technology Stack
- .NET 8, ASP.NET Core, EF Core, MS SQL

## Additional Requirements
- Core business logic should be covered by integration tests
- The project should include docker-compose for setting up the local environment
- The project source code should be available on GitHub

## Project Setup and Usage

The project was generated using the [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/ASWISS) version 8.0.3.

### Build

Run `dotnet build -tl` to build the solution.

### Run

To run the web application:

```bash
cd .\src\Web\
dotnet watch run
```
### Code Styles & Formatting

The template includes [EditorConfig](https://editorconfig.org/) support to help maintain consistent coding styles for multiple developers working on the same project across various editors and IDEs. The **.editorconfig** file defines the coding styles applicable to this solution.

### Code Scaffolding

The template includes support to scaffold new commands and queries.

Start in the `.\src\Application\` folder.

Create a new command:

```bash
dotnet new ca-usecase --name CreateTodoList --feature-name TodoLists --usecase-type command --return-type int
```

Create a new query:

```bash
dotnet new ca-usecase -n GetTodos -fn TodoLists -ut query -rt TodosVm
```

If you encounter the error *"No templates or subcommands found matching: 'ca-usecase'."*, install the template and try again:

```bash
dotnet new install Clean.Architecture.Solution.Template::8.0.3
```

### Test

The solution contains unit, integration, and functional tests.

To run the tests:
```bash
dotnet test
```

### Help

To learn more about the template go to the [project website](https://github.com/jasontaylordev/CleanArchitecture). Here you can find additional guidance, request new features, report a bug, and discuss the template with other users.
