using SIGD.Models;
using System.Collections.Generic;

namespace SIGD.Interfaces
{
    /// <summary>
    /// Interface implements UsersManagedService
    /// </summary>
    public interface IUsersManagedService
    {
        /// <summary>
        /// Get all principals accounts
        /// </summary>
        /// <returns>List of principals accounts</returns>
        List<ActivationAccount> GetAllPrincipalsAccounts();

        /// <summary>
        /// Get all principals accounts managed
        /// </summary>
        /// <param name="adminManager">admin thet created the account</param>
        /// <returns>List of principals accounts managed</returns>
        List<ActivationAccount> GetAllPrincipalsAccountsByAdmin(string adminManager);
    }
}
