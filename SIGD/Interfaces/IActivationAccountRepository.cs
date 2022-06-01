using SIGD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGD.Interfaces
{
    public interface IActivationAccountRepository
    {

        /// <summary>
        /// Save model data in data base
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isFirstAccess"></param>
        /// <returns>true if data saved</returns>
        /// <returns>false otherwise</returns>
        bool Save(ActivationAccount data, bool isFirstAccess);

        /// <summary>
        /// Get ActivationAccount data that contains email
        /// </summary>
        /// <param name="email">email saved</param>
        /// <returns>ActivationAccount data</returns>
        ActivationAccount GetActivationAccountByEmail(string email);

        /// <summary>
        /// Get ActivationAccount data that contains username
        /// </summary>
        /// <param name="username">username saved</param>
        /// <returns>ActivationAccount data</returns>
        ActivationAccount GetActivationAccountByUserName(string username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        ActivationAccount GetUser(ActivationAccount userModel);
    }
}
