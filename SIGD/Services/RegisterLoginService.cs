using SIGD.Helper;
using SIGD.Interfaces;
using SIGD.Models;
using System;
using System.Net;
using System.Security;

namespace SIGD.Services
{
    public class RegisterLoginService : IRegisterLoginService
    {
        private IEmailSender emailSender;
        private ITokenService tokenService;

        public RegisterLoginService(IEmailSender emailSender, ITokenService tokenService)
        {
            this.emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            this.tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activationAccount"></param>
        /// <returns></returns>
        public ActivationAccount CreateNewAdminUser(ActivationAccount activationAccount)
        {           
            try
            {
                activationAccount.role = Role.administrator;
                activationAccount.IsActivated = false;
                SecureString tokenFirstAccess = tokenService.GetToken();
                activationAccount.Password = tokenService.Hash(tokenFirstAccess);
                activationAccount.passwordExpiration = DateTime.Now.AddMonths(3);
                bool isSended = emailSender.SendEmailFirstAccess(activationAccount.Email, tokenFirstAccess, $"Bem vindo ao SIGD {activationAccount.UserName},Sua senha de primeiro acesso: ");
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
                return emailSender.SendEmailPasswordChanged(activationAccount.Email, $"Bem vindo ao SIGD {activationAccount.UserName} sua conta foi ativada. <br>se não  foi  você  entreem contato com o IT");
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
    }
}
