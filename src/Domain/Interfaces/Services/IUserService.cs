using JumboTravel.Api.src.Domain.Models.Users.Requests;
using JumboTravel.Api.src.Domain.Models.Users.Responses;

namespace JumboTravel.Api.src.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<LoginResponse?> Login(LoginRequest rq);
    }
}
