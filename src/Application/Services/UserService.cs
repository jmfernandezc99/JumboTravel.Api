using JumboTravel.Api.src.Domain.Interfaces.Services;
using JumboTravel.Api.src.Domain.Models.Users;
using MySql.Data.MySqlClient;

namespace JumboTravel.Api.src.Application.Services
{
    public class UserService : IUserService
    {
        public const string _connectionString = "server=localhost;user=root;database=JUMBOTRAVEL;port=3306;password=root";

        public bool UserExists(GetUserRequest rq)
        {
            bool result = false;
            string query = "SELECT dni, pass FROM Personal;";

            MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (rq.dni == reader.GetString(0) && rq.pass == reader.GetString(1))
                {
                    result = true;
                }
            }
            reader.Close();
            command.Dispose();
            connection.Close();
            return result;
        }
    }
}
