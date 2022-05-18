using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGD.Helper
{
    public class TokenGenerator
    {
        private const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LowerChars = "abcdefghijklmnopqrstuvwxyz";
        private const string specialChars = "!”#$%&'()*+,-./:;<=>?@[\\]^_`{|}~.";
        private const string numbers = "0123456789";

        /// <summary>
        /// Create a random token for first connection password
        /// </summary>
        /// <returns>random token</returns>
        public string GetToken()
        {
            string allChar = string.Join(upperChars, LowerChars, specialChars, numbers);
            Random random = new Random();

            return new string(Enumerable.Repeat(allChar, 25).Select(token => token[random.Next(token.Length)]).ToArray());
        }
    }
}
