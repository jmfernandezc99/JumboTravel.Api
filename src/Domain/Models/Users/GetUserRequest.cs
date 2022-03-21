namespace JumboTravel.Api.src.Domain.Models.Users
{
    public class GetUserRequest
    {
        public string dni { get; set; } = string.Empty;
        public string pass { get;set; } = string.Empty;
    }
}
