using JumboTravel.Api.src.Domain.Enums;

namespace JumboTravel.Api.src.Domain.Models.Users.Responses
{
    public class LoginResponse
    {
        public string? UserId { get; set; }
        public Role Role { get; set; }
        public List<string> Origins { get; set; } = new List<string>();
    }
}
