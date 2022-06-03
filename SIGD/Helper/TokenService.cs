using Microsoft.IdentityModel.Tokens;
using SIGD.Interfaces;
using SIGD.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SIGD.Helper
{
    public class TokenService : ITokenService
    {
        private const double EXPIRY_DURATION_MINUTES = 30;
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
        public SecureString GetToken()
        {
            string allChar = string.Join(upperChars, lowerChars, specialChars, numbers);
            Random random = new Random();

            return new NetworkCredential("", 
                new string(Enumerable.Repeat(allChar, 25).Select(token => token[random.Next(token.Length)]).ToArray())).SecurePassword;
        }        

        /// <summary>
        /// Creates a hash from a password
        /// </summary>
        /// <param name="password">password</param>
        /// <returns>The base64 token</returns>
        public string Hash(SecureString password)
        {
            return Hash(new NetworkCredential(string.Empty, password).Password, interactions);
        }

        /// <summary>
        /// Verifies a password against a hash.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="hashedPassword">The hash.</param>
        /// <returns>True if is equal</returns>
        /// <returns>False otherwise</returns>
        public bool Verify(SecureString password, SecureString hashedPassword)
        {
            // Get hash bytes
            byte[] hashBytes = Convert.FromBase64String(new NetworkCredential(string.Empty, hashedPassword).Password);
            // Get salt
            byte[] salt = new byte[saltSize];
            Array.Copy(hashBytes, 0, salt, 0, saltSize);

            // Create hash with given salt
            var pbkdf2 = new Rfc2898DeriveBytes(new NetworkCredential(string.Empty, password).Password, salt, interactions);
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

        public string BuildToken(string key, string issuer, ActivationAccount user)
        {
            var claims = new[] {
            new Claim(ClaimTypes.Name, user.UserName),
            //new Claim(ClaimTypes.Role, user.role),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims,
                expires: DateTime.Now.AddMinutes(EXPIRY_DURATION_MINUTES), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public bool IsTokenValid(string key, string issuer, string token)
        {
            var mySecret = Encoding.UTF8.GetBytes(key);
            var mySecurityKey = new SymmetricSecurityKey(mySecret);
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = issuer,
                    ValidAudience = issuer,
                    IssuerSigningKey = mySecurityKey,
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
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
