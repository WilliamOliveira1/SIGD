using SIGD.Data;
using SIGD.Helper;
using SIGD.Models;
using System;

namespace SIGD.Services
{
    public class RegisterLoginService
    {
        private readonly ApplicationDbContext _context;
        public RegisterLoginService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool CreateNewAdminUSer(ActivationAccount activationAccount)
        {

            EmailSender emailSender = new EmailSender();
            TokenService tokenService = new TokenService();

            try
            {
                activationAccount.role = Role.administrator;
                activationAccount.IsActivated = false;
                string tokenFirstAccess = tokenService.GetToken();
                activationAccount.password = tokenService.Hash(tokenFirstAccess);
                _context.Add(activationAccount);
                _context.SaveChangesAsync();
                emailSender.SendEmail(activationAccount.Email, tokenFirstAccess, $"Bem vindo ao SIGD {activationAccount.UserName},Sua senha de primeiro acesso: ");
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
