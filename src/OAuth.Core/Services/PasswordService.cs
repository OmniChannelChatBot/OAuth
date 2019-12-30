using OAuth.Core.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;

namespace OAuth.Core.Services
{
    public class PasswordService : IPasswordService
    {
        public void CreateHash(string password, out byte[] hash, out byte[] salt)
        {
            using (var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public bool Verify(string password, byte[] hash, byte[] salt)
        {
            if (hash.Length != 64)
            {
                throw new ArgumentException($"Invalid length of {nameof(hash)} (64 bytes expected)", nameof(hash));
            }

            if (salt.Length != 128)
            {
                throw new ArgumentException($"Invalid length of {nameof(salt)} (128 bytes expected)", nameof(salt));
            }

            using (var hmac = new HMACSHA512(salt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != hash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
