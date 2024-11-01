using ASWISS.Application.Users.Commands.CreateUser;
using ASWISS.Application.Users.Queries.GetUsers;

namespace ASWISS.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateUser)
            .MapGet(GetUsers);
    }

    public async Task<List<UserDto>> GetUsers(ISender sender, [AsParameters] GetUsersQuery query)
    {
        return await sender.Send(query);
    }

    public Task<string> CreateUser(ISender sender, CreateUserCommand command)
    {
        return sender.Send(command);
    }
}
