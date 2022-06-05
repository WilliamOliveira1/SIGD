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
        ActivationAccount CreateNewUser(ActivationAccount activationAccount, Role accountType, string adminManager = null);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activationAccount"></param>
        /// <param name="isFirstAccess"></param>
        /// <returns></returns>
        bool SaveAccount(ActivationAccount activationAccount, bool isFirstAccess);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        ActivationAccount GetUser(ActivationAccount userModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        ActivationAccount GetUserByUsername(string username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        ActivationAccount GetUserByEmail(string email);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adminManag"></param>
        /// <returns></returns>
        List<ActivationAccount> getAllPrincipalsByAdmin(string adminManag);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="issuer"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        string GetJWTToken(string Key, string issuer, ActivationAccount account);
    }
}
