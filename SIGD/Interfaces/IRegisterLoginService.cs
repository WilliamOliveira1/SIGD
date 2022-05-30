using SIGD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace SIGD.Interfaces
{
    public interface IRegisterLoginService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="activationAccount"></param>
        /// <returns></returns>
        ActivationAccount CreateNewAdminUser(ActivationAccount activationAccount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activationAccount"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        ActivationAccount ChangePassword(ActivationAccount activationAccount, SecureString newPassword);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="dbPassword"></param>
        /// <returns></returns>
        bool TokenMatch(SecureString oldPassword, SecureString dbPassword);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activationAccount"></param>
        bool ChangePasswordSendEmail(ActivationAccount activationAccount);
    }
}
