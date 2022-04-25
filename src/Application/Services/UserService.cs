using Dapper;
using JumboTravel.Api.src.Application.Data;
using JumboTravel.Api.src.Application.Extensions;
using JumboTravel.Api.src.Domain.Enums;
using JumboTravel.Api.src.Domain.Interfaces.Services;
using JumboTravel.Api.src.Domain.Models.Providers;
using JumboTravel.Api.src.Domain.Models.Users;
using JumboTravel.Api.src.Domain.Models.Users.Requests;
using JumboTravel.Api.src.Domain.Models.Users.Responses;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JumboTravel.Api.src.Application.Services
{
    public class UserService : IUserService
    {
        private readonly DapperContext _context;
        private readonly IConfiguration _configuration;

        public UserService(DapperContext context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<LoginResponse> Login(LoginRequest rq)
        {
            using (var connection = _context.CreateConnection())
            {
                string query = $"SELECT * FROM Users";

                var response = await connection.QueryAsync<User>(query).ConfigureAwait(false);
                var users = response.ToList();

                User? user = users.Where(user => user.Nif == rq.UserName && user.Password == rq.Password).FirstOrDefault();
                if (user == null)
                    return new LoginResponse();

                Role userRole = Role.Unknown;
                List<string> origins = new List<string>();

                string queryProvider = $"SELECT * FROM Providers WHERE user_id = '{user.Id}'";
                var providers = await connection.QueryAsync<Provider>(queryProvider).ConfigureAwait(false);
                var provider = providers.FirstOrDefault();
                if (provider != null)
                {
                    userRole = Role.Provider;
                    origins.Add(provider.Base);
                }
                else
                {
                    userRole = Role.Attendant;
                    string getOrigins = $"select R.origin from attendants as A inner join planes as p on A.plane_id = P.id inner join routes as R on R.plane_id = P.id where A.user_id = {user.Id} order by origin desc";
                    var getOriginsResponse = await connection.QueryAsync<string>(getOrigins).ConfigureAwait(false);
                    origins = getOriginsResponse.ToList();
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, rq.UserName)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(10),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature),
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return new LoginResponse()
                {
                    JWTToken = tokenHandler.WriteToken(token),
                    Origins = origins,
                    Role = userRole
                };
            }
        }
    }
}
