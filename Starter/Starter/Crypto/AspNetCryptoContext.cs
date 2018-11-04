using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using OtpNet;
using Starter.Services.Crypto;

namespace Starter.API.Crypto
{
    public class AspNetCryptoContext : ICryptoContext
    {
        private readonly CryptoOptions _options;

        public AspNetCryptoContext(IOptions<CryptoOptions> options)
        {
            _options = options.Value;

            if (_options == null) { throw new ArgumentNullException(); }
            if (_options.SaltSizeBits < 512 || _options.SaltSizeBits % 8 != 0)
            {
                throw new ArgumentException($"Salt size cannot be less than {_options.SaltSizeBits} bits and has to be dividable by 8");
            }
            if (_options.DerivedKeySizeBits < 512 || _options.DerivedKeySizeBits % 8 != 0)
            {
                throw new ArgumentException($"Derived key size cannot be less than {_options.DerivedKeySizeBits} bits and has to be dividable by 8");
            }
        }

        public byte[] DeriveKey(string password, string salt_base64)
        {
            return DeriveKey(password, Convert.FromBase64String(salt_base64));
        }

        public byte[] DeriveKey(string password, byte[] salt)
        {
            byte[] key = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: _options.DerivedKeyIterations,
                numBytesRequested: _options.DerivedKeySizeBits
                );
            return key;
        }

        public string GenerateSaltAsBase64() => Convert.ToBase64String(GenerateSalt());

        public byte[] GenerateSalt()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] salt = new byte[_options.SaltSizeBits];
                rng.GetBytes(salt);
                return salt;
            }
        }

        public bool ArePasswordsEqual(string password, string encodedPassword, string salt_base64)
        {
            return ArePasswordsEqual(password, encodedPassword, Convert.FromBase64String(salt_base64));
        }

        public bool ArePasswordsEqual(string password, string encodedPassword, byte[] salt)
        {
            byte[] storedKey = Convert.FromBase64String(encodedPassword);

            byte[] derivedKey = DeriveKey(password, salt);

            return storedKey.SequenceEqual(derivedKey);
        }

        public string GeneratePasswordAsBase32()
        {
            char[] chars = new char[62];
            chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[64];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(64);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return Base32Encoding.ToString(Encoding.ASCII.GetBytes(result.ToString())).TrimEnd('=');
        }
    }
}