using Dapper;
using JumboTravel.Api.src.Application.Data;
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
        public UserService(DapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<LoginResponse?> Login(LoginRequest rq)
        {
            string query = $"SELECT * FROM Users WHERE nif = '{rq.UserName}' AND password = '{rq.Password}'";
            using (var connection = _context.CreateConnection())
            {
                var response = await connection.QueryAsync<User>(query).ConfigureAwait(false);
                var user = response.FirstOrDefault();

                if (user != null)
                {
                    string queryProvider = $"SELECT * FROM Providers WHERE user_id = '{user.Id}'";
                    var providers = await connection.QueryAsync<Provider>(queryProvider).ConfigureAwait(false);
                    var provider = providers.FirstOrDefault();
                    Role userRole = provider != null ? Role.Provider : Role.Attendant;

                    return new LoginResponse()
                    {
                        UserId = Encrypt(user.Id),
                        Role = userRole
                    };
                }
                return null;
            }
        }

        private string Encrypt(int id)
        {
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(id.ToString());
            return Convert.ToBase64String(encryted);
        }
        private int Decrypt(string id)
        {
            byte[] decryted = Convert.FromBase64String(id);
            string decryptedId = System.Text.Encoding.Unicode.GetString(decryted);
            return int.TryParse(decryptedId, out int result) ? result : 0;
        }
    }
}
