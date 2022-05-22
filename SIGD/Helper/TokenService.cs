using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SIGD.Helper
{
    public class TokenService
    {
        private const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
        private const string specialChars = "!”#$%&'()*+,-./:;<=>?@[\\]^_`{|}~.";
        private const string numbers = "0123456789";
        private const int interactions = 10000;
        private const int saltSize = 14;
        private const int hashSize = 25;

        /// <summary>
        /// Create a random token for first connection password
        /// </summary>
        /// <returns>random token</returns>
        public string GetToken()
        {
            string allChar = string.Join(upperChars, lowerChars, specialChars, numbers);
            Random random = new Random();

            return new string(Enumerable.Repeat(allChar, 25).Select(token => token[random.Next(token.Length)]).ToArray());
        }        

        /// <summary>
        /// Creates a hash from a password
        /// </summary>
        /// <param name="password">password</param>
        /// <returns>The base64 token</returns>
        public string Hash(string password)
        {
            return Hash(password, interactions);
        }

        /// <summary>
        /// Verifies a password against a hash.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="hashedPassword">The hash.</param>
        /// <returns>True if is equal</returns>
        /// <returns>False otherwise</returns>
        public bool Verify(string password, string hashedPassword)
        {
            // Get hash bytes
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            // Get salt
            byte[] salt = new byte[saltSize];
            Array.Copy(hashBytes, 0, salt, 0, saltSize);

            // Create hash with given salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, interactions);
            byte[] hash = pbkdf2.GetBytes(hashSize);

            // Get result
            for (var i = 0; i < hashSize; i++)
            {
                if (hashBytes[i + saltSize] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Creates a hash from a password.
        /// </summary>
        /// <param name="password">The password</param>
        /// <param name="iterations">Number of iterations</param>
        /// <returns>The base64 token</returns>
        private static string Hash(string password, int iterations)
        {
            // Create salt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[saltSize]);

            // Create hash
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            var hash = pbkdf2.GetBytes(hashSize);

            // Combine salt and hash
            var hashBytes = new byte[saltSize + hashSize];
            Array.Copy(salt, 0, hashBytes, 0, saltSize);
            Array.Copy(hash, 0, hashBytes, saltSize, hashSize);

            // Convert to base64
            return Convert.ToBase64String(hashBytes);
        }
    }
}
