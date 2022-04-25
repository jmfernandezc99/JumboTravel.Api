using JumboTravel.Api.src.Domain.Enums;

namespace JumboTravel.Api.src.Domain.Models.Users.Responses
{
    public class LoginResponse
    {
        public string JWTToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public Role Role { get; set; }
        public List<string> Origins { get; set; } = new List<string>();
    }
}
