namespace Starter.Services.Crypto
{
    public interface ICryptoContext
    {
        byte[] GenerateSalt();

        string GenerateSaltAsBase64();

        byte[] DeriveKey(string password, string salt_base64);

        byte[] DeriveKey(string password, byte[] salt);

        bool ArePasswordsEqual(string password, string encodedPassword, byte[] salt);

        bool ArePasswordsEqual(string password, string encodedPassword, string salt_base64);
    }
}