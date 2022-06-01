using SIGD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace SIGD.Interfaces
{
    public interface ITokenService
    {
        /// <summary>
        /// Create a random token for first connection password
        /// </summary>
        /// <returns>random token</returns>
        SecureString GetToken();


        /// <summary>
        /// Creates a hash from a password
        /// </summary>
        /// <param name="password">password</param>
        /// <returns>The base64 token</returns>
        string Hash(SecureString password);


        /// <summary>
        /// Verifies a password against a hash.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="hashedPassword">The hash.</param>
        /// <returns>True if is equal</returns>
        /// <returns>False otherwise</returns>
        bool Verify(SecureString password, SecureString hashedPassword);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="issuer"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        string BuildToken(string key, string issuer, ActivationAccount user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="issuer"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        bool IsTokenValid(string key, string issuer, string token);
    }
}
