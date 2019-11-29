using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OAuth.Entity;
using OAuth.Exceptions;
using OAuth.Helpers;
using OAuth.Models;
using OAuth.Models.DBApi;
using OAuth.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OAuth.Services
{
    public interface IUserService
    {
        Task<User> AuthenticateAsync(string username, string password);

        Task<User> CreateAsync(User user);

        Task<bool> CheckUserAsync(string userName, string password);

        Task<bool> CheckUserNameAsync(string userName);

        Task<User> GetUserAsync(string userName, string password);

        Task<User> GetAsync(int id);
    }

    public class UserService : IUserService
    {
        private readonly AppOptions _appSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DBApiOptions _dbApiSettings;

        public UserService(IOptions<AppOptions> appSettings,
            IHttpClientFactory httpClientFactory,
            IOptions<DBApiOptions> dbApiSettings)
        {
            _appSettings = appSettings.Value;
            _httpClientFactory = httpClientFactory;
            _dbApiSettings = dbApiSettings.Value;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var exists = await CheckUserAsync(username, password);

            // return null if user not found
            if (exists == false)
            {
                return null;
            }

            var user = await GetUserAsync(username, password);

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_appSettings.Secret);
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

        public async Task<bool> CheckUserNameAsync(string userName)
        {
            var DBApiUrl = Client.DBApiUrl.GetDBApiFullUrl(_dbApiSettings.Url, Client.DBApiUrl.CheckUserName);

            bool responseBool;

            var checkUserNameModel = new CheckUserNameModel()
            {
                UserName = userName
            };

            using (var client = _httpClientFactory.CreateClient())
            {
                var response = await client.PostAsync(DBApiUrl,
                    JsonSerializerHelper.Serialize(checkUserNameModel));

                responseBool = JsonSerializerHelper.Deserialize<bool>(response);
            }

            return responseBool;
        }

        public async Task<bool> CheckUserAsync(string userName, string password)
        {
            var DBApiUrl = Client.DBApiUrl.GetDBApiFullUrl(_dbApiSettings.Url, Client.DBApiUrl.CheckUser);

            bool responseBool;

            var checkUserModel = new CheckUserModel()
            {
                UserName = userName,
                Password = password
            };

            using (var client = _httpClientFactory.CreateClient())
            {
                var response = await client.PostAsync(DBApiUrl,
                    JsonSerializerHelper.Serialize(checkUserModel));

                responseBool = JsonSerializerHelper.Deserialize<bool>(response);
            }

            return responseBool;
        }

        public async Task<User> GetUserAsync(string userName, string password)
        {
            var DBApiUrl = Client.DBApiUrl.GetDBApiFullUrl(_dbApiSettings.Url, Client.DBApiUrl.GetUser);

            User user;

            var checkUserModel = new CheckUserModel()
            {
                UserName = userName,
                Password = password
            };

            using (var client = _httpClientFactory.CreateClient())
            {
                var response = await client.PostAsync(DBApiUrl,
                    JsonSerializerHelper.Serialize(checkUserModel));

                user = JsonSerializerHelper.Deserialize<User>(response);
            }

            return user;
        }

        public async Task<User> CreateAsync(User user)
        {
            await Validate(user);

            var registerUser = new RegisterUserModel()
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
                    JsonSerializerHelper.Serialize(registerUser));

                deserializeUser = JsonSerializerHelper.Deserialize<User>(response);
            }

            //byte[] passwordHash, passwordSalt;
            //CreatePasswordHash(user.Password, out passwordHash, out passwordSalt);

            //user.PasswordHash = passwordHash;
            //user.PasswordSalt = passwordSalt;

            return deserializeUser;
        }

        private async Task Validate(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                throw new OAuthException("Password is required");
            }

            // validate user in the end

            var exists = await CheckUserNameAsync(user.Username);
            if (exists)
            {
                throw new OAuthException($"Username {user.Username} is already taken");
            }
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

        public async Task<User> GetAsync(int id)
        {
            var DBApiUrl = Client.DBApiUrl.GetDBApiFullUrl(_dbApiSettings.Url, id.ToString());

            User deserializeUser = null;

            using (var client = _httpClientFactory.CreateClient())
            {
                var response = await client.GetAsync(DBApiUrl);

                deserializeUser = JsonSerializerHelper.Deserialize<User>(response);
            }

            return deserializeUser;
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
