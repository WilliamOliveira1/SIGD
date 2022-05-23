using SIGD.Data;
using SIGD.Helper;
using SIGD.Models;
using System;
using System.Security;

namespace SIGD.Services
{
    public class RegisterLoginService
    {
        private EmailSender emailSender = new EmailSender();
        private TokenService tokenService = new TokenService();
        public ActivationAccount CreateNewAdminUser(ActivationAccount activationAccount)
        {           
            try
            {
                activationAccount.role = Role.administrator;
                activationAccount.IsActivated = false;
                string tokenFirstAccess = tokenService.GetToken();
                activationAccount.password = tokenService.Hash(tokenFirstAccess);
                activationAccount.passwordExpiration = DateTime.Now.AddMonths(3);
                emailSender.SendEmail(activationAccount.Email, tokenFirstAccess, $"Bem vindo ao SIGD {activationAccount.UserName},Sua senha de primeiro acesso: ");
                return activationAccount;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public bool TokenMatch(string oldPassword, string dbPassword)
        {
            return tokenService.Verify(oldPassword, dbPassword);
        }
    }
}
