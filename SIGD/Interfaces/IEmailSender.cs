using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace SIGD.Interfaces
{
    public interface IEmailSender
    {
        /// <summary>
        /// Send email with first access password
        /// </summary>
        /// <param name="emailAdressTo">email address</param>
        /// <param name="mailBody">HTML with the email body</param>
        bool SendEmailFirstAccess(string emailAdressTo, SecureString tokenFirstAccess, string mailBody);

        /// <summary>
        /// Send email with first access password
        /// </summary>
        /// <param name="emailAdressTo">email address</param>
        /// <param name="mailBody">HTML with the email body</param>
        bool SendEmailPasswordChanged(string emailAdressTo, string mailBody);
    }
}
