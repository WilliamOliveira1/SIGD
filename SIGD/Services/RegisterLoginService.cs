using SIGD.Helper;
using SIGD.Interfaces;
using SIGD.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security;

namespace SIGD.Services
{
    public class RegisterLoginService : IRegisterLoginService
    {
        private IEmailSender emailSender;
        private ITokenService tokenService;
        private IActivationAccountRepository databaseService;

        public RegisterLoginService(IEmailSender emailSender, ITokenService tokenService, IActivationAccountRepository databaseService)
        {
            this.emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            this.tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            this.databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activationAccount"></param>
        /// <returns></returns>
        public ActivationAccount CreateNewUser(ActivationAccount activationAccount, Role accountType, string adminManager = null)
        {
            string emailHtml = string.Empty;
            try
            {
                activationAccount.adminManager = adminManager;
                activationAccount.role = accountType;
                activationAccount.IsActivated = false;
                SecureString tokenFirstAccess = tokenService.GetToken();
                activationAccount.Password = tokenService.Hash(tokenFirstAccess);
                activationAccount.passwordExpiration = DateTime.Now.AddMonths(3);
                if(string.IsNullOrEmpty(activationAccount.adminManager))
                {
                    emailHtml = File.ReadAllText(SharedPaths.FIRST_PASS_ACCESS_EMAIL_ADMIN);
                    emailHtml = emailHtml.Replace("senhaprimeiroacesso", new NetworkCredential(string.Empty, tokenFirstAccess).Password).Replace("nomedeusuario ", activationAccount.UserName + " ");
                }
                else
                {
                    emailHtml = File.ReadAllText(SharedPaths.FIRST_PASS_ACCESS_EMAIL_PRINCIPAL);
                    emailHtml = emailHtml.Replace("senhaprimeiroacesso", 
                        new NetworkCredential(string.Empty, tokenFirstAccess).Password).Replace("nomedeusuario ", activationAccount.UserName)
                        .Replace("nomedosupervisor", activationAccount.adminManager);
                }                
                bool isSended = emailSender.SendEmailFirstAccess(activationAccount.Email, tokenFirstAccess, emailHtml);
                if(!isSended)
                {
                    return null;
                }

                return activationAccount;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public bool SaveAccount(ActivationAccount activationAccount, bool isFirstAccess)
        {
            return databaseService.Save(activationAccount, isFirstAccess);
        }

        public ActivationAccount ChangePassword(ActivationAccount activationAccount, SecureString newPassword)
        {
            try
            {
                activationAccount.IsActivated = true;
                activationAccount.Password = new NetworkCredential(string.Empty, tokenService.Hash(newPassword)).Password;                

                return activationAccount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public bool ChangePasswordSendEmail(ActivationAccount activationAccount)
        {
            try
            {
                string emailHtml = File.ReadAllText(SharedPaths.ACCOUNT_ACTIVATED_EMAIL_TEMPLATE);
                emailHtml = emailHtml.Replace("nomedeusuario ", activationAccount.UserName + " ");
                return emailSender.SendEmailPasswordChanged(activationAccount.Email, emailHtml);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="dbPassword"></param>
        /// <returns></returns>
        public bool TokenMatch(SecureString oldPassword, SecureString dbPassword)
        {
            return tokenService.Verify(oldPassword, dbPassword);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public ActivationAccount GetUserByUsername(string username, bool cleanPass = true)
        {
            ActivationAccount user = databaseService.GetActivationAccountByUserName(username);
            if(user != null && cleanPass)
            {
                user.Password = null;
            }            
            return user;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public ActivationAccount GetUserByEmail(string email, bool cleanPass = true)
        {
            ActivationAccount user = databaseService.GetActivationAccountByEmail(email);
            if (user != null && cleanPass)
            {
                user.Password = null;
            }
            return user;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adminManag"></param>
        /// <returns></returns>
        public List<ActivationAccount> getAllPrincipalsByAdmin(string adminManag, bool cleanPass = true)
        {
            try
            {
                List<ActivationAccount> users = databaseService.GetAllPrincipalsAccountsByAdmin(adminManag);
                if (cleanPass)
                {
                    foreach (var user in users)
                    {
                        user.Password = null;
                    }
                }

                return users;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="issuer"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public string GetJWTToken(string Key, string issuer, ActivationAccount account)
        {
            return tokenService.BuildToken(Key, issuer.ToString(), account);
        }
    }
}
