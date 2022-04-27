using Dapper;
using JumboTravel.Api.src.Application.Data;
using JumboTravel.Api.src.Application.Extensions;
using JumboTravel.Api.src.Domain.Enums;
using JumboTravel.Api.src.Domain.Interfaces.Services;
using JumboTravel.Api.src.Domain.Models.Providers;
using JumboTravel.Api.src.Domain.Models.Users;
using JumboTravel.Api.src.Domain.Models.Users.Requests;
using JumboTravel.Api.src.Domain.Models.Users.Responses;

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
            if (!string.IsNullOrEmpty(rq.JsonWebToken) && !JwtExtension.ValidateToken(rq.JsonWebToken))
                return new LoginResponse()
                {
                    Message = "Token Expired",
                    Exit = 1
                };

            using (var connection = _context.CreateConnection())
            {
                string query = $"SELECT * FROM Users";

                var response = await connection.QueryAsync<User>(query).ConfigureAwait(false);
                var users = response.ToList();
                
                User? user = !string.IsNullOrEmpty(rq.JsonWebToken) 
                            ? users.Where(user => user.Nif == JwtExtension.ReturnUserFromToken(rq.JsonWebToken).Nif).FirstOrDefault() 
                            : users.Where(user => user.Nif == rq.UserName && user.Password == rq.Password).FirstOrDefault();

                if (user == null)
                    return new LoginResponse() { Exit = 1};

                List<string> origins = new List<string>();

                string queryProvider = $"SELECT * FROM Providers WHERE user_id = '{user.Id}'";
                var providers = await connection.QueryAsync<Provider>(queryProvider).ConfigureAwait(false);

                if (providers.Count() > 0)
                {
                    origins.Add(providers.FirstOrDefault()!.Base);
                }
                else
                {
                    string getOrigins = $"select R.origin from attendants as A inner join planes as p on A.plane_id = P.id " +
                        $"inner join routes as R on R.plane_id = P.id where A.user_id = {user.Id} order by origin desc";
                    var getOriginsResponse = await connection.QueryAsync<string>(getOrigins).ConfigureAwait(false);
                    origins = getOriginsResponse.ToList();
                }

                return new LoginResponse()
                {
                    JWTToken = string.IsNullOrEmpty(rq.JsonWebToken) ? JwtExtension.CreateToken(_configuration, user.Nif!) : string.Empty,
                    Origins = origins,
                    Role = providers.Count() > 0 ? Role.Provider : Role.Attendant
                };
            }
        }
    }
}
