using JumboTravel.Api.src.Domain.Models.Users;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JumboTravel.Api.src.Application.Extensions
{
    public static class JwtExtension
    {
        public static string CreateToken(IConfiguration _configuration, string data)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, data!)
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static User ReturnUserFromToken(string token)
        {
            var tokenConverted = new JwtSecurityToken(token.Replace("Bearer ", string.Empty));

            return new User()
            {
                Nif = tokenConverted.Claims.FirstOrDefault()!.ToString().Replace("unique_name: ", string.Empty)
            };
        }

        public static bool ValidateToken(string token) => DateTime.Now < new JwtSecurityToken(token.Replace("Bearer ", string.Empty)).ValidTo;
    }
}
