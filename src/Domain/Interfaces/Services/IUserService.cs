using JumboTravel.Api.src.Domain.Models.Users;

namespace JumboTravel.Api.src.Domain.Interfaces.Services
{
    public interface IUserService
    {
        bool UserExists(GetUserRequest rq);
    }
}
