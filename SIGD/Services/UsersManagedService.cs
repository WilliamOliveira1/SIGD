using SIGD.Interfaces;
using SIGD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGD.Services
{
    /// <summary>
    /// Class service that support UsersManaged controller
    /// </summary>
    public class UsersManagedService : IUsersManagedService
    {
        private IActivationAccountRepository databaseService;

        public UsersManagedService(IActivationAccountRepository databaseService)
        {
            this.databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        }

        /// <summary>
        /// Get all principals accounts
        /// </summary>
        /// <returns>List of principals accounts</returns>
        public List<ActivationAccount> GetAllPrincipalsAccounts()
        {
            return databaseService.GetAllPrincipalsAccounts();
        }

        /// <summary>
        /// Get all principals accounts managed
        /// </summary>
        /// <param name="adminManager">admin thet created the account</param>
        /// <returns>List of principals accounts managed</returns>
        public List<ActivationAccount> GetAllPrincipalsAccountsByAdmin(string adminManager)
        {
            return databaseService.GetAllPrincipalsAccountsByAdmin(adminManager);
        }
    }
}
