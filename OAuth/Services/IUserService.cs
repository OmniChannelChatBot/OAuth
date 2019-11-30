using OAuth.Entity;
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
}
