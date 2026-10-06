using AuthDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace AuthDB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]


    public class AuthController : ControllerBase
    {
        private readonly string _connectionString;

            public AuthController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            string username =
                request.FirstName.Substring(0, 2).ToLower() + request.LastName.Substring(0, 2).ToLower() + request.DateOfBirth.Year;

            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = @" INSERT INTO Users
                (FirstName, LastName, DateOfBirth, Username, Password)
                VALUES
                (@FirstName, @LastName, @DateOfBirth, @Username, @Password)
            ";

            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@FirstName", request.FirstName);
            command.Parameters.AddWithValue("@LastName", request.LastName);
            command.Parameters.AddWithValue("@DateOfBirth", request.DateOfBirth);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", request.Password);

            command.ExecuteNonQuery();

            return Ok(new { message = "User registered successfully", username = username });
        }


        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            using SqlConnection connection =
                new SqlConnection(_connectionString);

            connection.Open();

            string query = @"
                SELECT COUNT(*)
                FROM Users
                WHERE Username = @Username
                AND Password = @Password
            ";

            using SqlCommand command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Username", request.Username);
            command.Parameters.AddWithValue("@Password", request.Password);

            int count = (int)command.ExecuteScalar();

            if (count > 0)
            {
                return Ok(new
                {
                    message = "Login successful"
                });
            }

            return Unauthorized(new
            {
                message = "Invalid username or password"
            });
        }
    }
}
    

