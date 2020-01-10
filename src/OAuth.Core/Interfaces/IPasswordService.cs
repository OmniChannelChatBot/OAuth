namespace OAuth.Core.Interfaces
{
    public interface IPasswordService
    {
        void CreateHash(string password, out byte[] hash, out byte[] salt);

        bool Verify(string password, byte[] hash, byte[] salt);
    }
}
