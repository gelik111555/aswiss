namespace ASWISS.Application.FunctionalTests;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Testing;

[TestFixture]
public abstract class BaseTestFixture
{
    protected static string GenerateRandomPassword(int length)
    {
        var random = new Random();
        const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string specialChars = "!@#%^&*";

        var password = new StringBuilder();
        password.Append(upperChars[random.Next(upperChars.Length)]);
        password.Append(lowerChars[random.Next(lowerChars.Length)]);
        password.Append(digits[random.Next(digits.Length)]);
        password.Append(specialChars[random.Next(specialChars.Length)]);

        var allChars = upperChars + lowerChars + digits + specialChars;
        for (int i = password.Length; i < length; i++)
        {
            password.Append(allChars[random.Next(allChars.Length)]);
        }

        return new string(password.ToString().OrderBy(_ => random.Next()).ToArray());
    }
    [SetUp]
    public async Task TestSetUp()
    {
        await ResetState();
    }
}
