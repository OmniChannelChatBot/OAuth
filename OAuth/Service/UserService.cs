using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OAuth.Client;
using OAuth.Entity;
using OAuth.Exception;
using OAuth.Helper;
using OAuth.Model.DBApi;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OAuth.Service
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        Task<User> Create(User user, string password);
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<User> _users = new List<User>
        {
            new User { Id = 1, FirstName = "Test", LastName = "User", Username = "test", Password = "test" }
        };

        private readonly AppSettings _appSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DBApiSettings _dbApiSettings;

        public UserService(IOptions<AppSettings> appSettings,
            IHttpClientFactory httpClientFactory,
            IOptions<DBApiSettings> dbApiSettings)
        {
            _appSettings = appSettings.Value;
            _httpClientFactory = httpClientFactory;
            _dbApiSettings = dbApiSettings.Value;
        }

        public User Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (user == null)
            {
                return null;
            }

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return user.WithoutPassword();
        }

        public async Task<User> Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new OAuthException("Password is required");
            }

            var registerUser = new DBApiRegisterModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Password = user.Password,
                Email = user.Email
            };

            var DBApiUrl = Client.DBApiUrl.GetDBApiFullUrl(_dbApiSettings.Url, Client.DBApiUrl.Create);

            User deserializeUser = null;

            using (var client = _httpClientFactory.CreateClient())
            {
                var response = await client.PostAsync(DBApiUrl,
                    new StringContent(
                        JsonSerializer.Serialize(registerUser), Encoding.UTF8, "application/json"));

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                deserializeUser = JsonSerializer.Deserialize<User>
                    (response.Content.ReadAsStringAsync().Result, options);
            }

            //if (_context.Users.Any(x => x.Username == user.Username))
            //{
            //    throw new OAuthException($"Username {user.Username} is already taken");
            //}

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            //user.PasswordHash = passwordHash;
            //user.PasswordSalt = passwordSalt;

            return deserializeUser;
        }

        public void Update(User userParam, string password = null)
        {
            //var user = _context.Users.Find(userParam.Id);

            //if (user == null)
            //{
            //    throw new OAuthException("User not found");
            //}

            //// update username if it has changed
            //if (!string.IsNullOrWhiteSpace(userParam.Username) && userParam.Username != user.Username)
            //{
            //    // throw error if the new username is already taken
            //    if (_context.Users.Any(x => x.Username == userParam.Username))
            //    {
            //        throw new OAuthException("Username " + userParam.Username + " is already taken");
            //    }

            //    user.Username = userParam.Username;
            //}

            //// update user properties if provided
            //if (!string.IsNullOrWhiteSpace(userParam.FirstName))
            //    user.FirstName = userParam.FirstName;

            //if (!string.IsNullOrWhiteSpace(userParam.LastName))
            //    user.LastName = userParam.LastName;

            //// update password if provided
            //if (!string.IsNullOrWhiteSpace(password))
            //{
            //    byte[] passwordHash, passwordSalt;
            //    CreatePasswordHash(password, out passwordHash, out passwordSalt);

            //    user.PasswordHash = passwordHash;
            //    user.PasswordSalt = passwordSalt;
            //}

            //_context.Users.Update(user);
            //_context.SaveChanges();
        }

        public User GetById(int id)
        {
            //return _context.Users.Find(id);

            return new User();
        }

        public IEnumerable<User> GetAll()
        {
            return _users.WithoutPasswords();
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            }

            if (storedHash.Length != 64)
            {
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            }

            if (storedSalt.Length != 128)
            {
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");
            }

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}
