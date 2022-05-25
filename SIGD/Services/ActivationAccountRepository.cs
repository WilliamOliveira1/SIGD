using Microsoft.EntityFrameworkCore;
using SIGD.Data;
using SIGD.Interfaces;
using SIGD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGD.Services
{
    // TODO create methods to use ApplicationDbContext outside controllers
    public class ActivationAccountRepository : IActivationAccountRepository
    {
        private ApplicationDbContext _context;

        public ActivationAccountRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Save model data in data base
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="data"></param>
        /// <returns>true if data saved</returns>
        /// <returns>false otherwise</returns>
        public bool Save(ActivationAccount data)
        {
            bool isEmailInDB = IsEmailSaved(data.Email);
            bool isUserNameInDB = IsUserNameSaved(data.UserName);

            if(isEmailInDB || isUserNameInDB)
            {
                return false;
            }

            try
            {
                var modelData = GetModelById(data.Id);
                if(modelData == null)
                {
                    _context.Add(data);
                }
                else
                {
                    _context.Update(data);                    
                }

                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get ActivationAccount data that contains email
        /// </summary>
        /// <param name="email">email saved</param>
        /// <returns>ActivationAccount data</returns>
        public ActivationAccount GetActivationAccountByEmail(string email)
        {
            return _context.ActivationAccount.Where(x => x.Email == email).FirstOrDefault();
        }
       
        /// <summary>
        /// Get ActivationAccount data that contains username
        /// </summary>
        /// <param name="username">username saved</param>
        /// <returns>ActivationAccount data</returns>
        public ActivationAccount GetActivationAccountByUserName(string username)
        {
            return _context.ActivationAccount.Where(x => x.UserName == username).FirstOrDefault();
        }

        /// <summary>
        /// Get ActivationAccount data that contains id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private ActivationAccount GetModelById(Guid id)
        {
            return _context.ActivationAccount.Where(x => x.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Check if exist an ActivationAccount saved with email
        /// </summary>
        /// <param name="email">email saved</param>
        /// <returns>true if exist in DB</returns>
        /// <returns>false otherwise</returns>
        private bool IsEmailSaved(string email)
        {
            var data = _context.ActivationAccount.Where(x => x.Email == email).FirstOrDefault();
            bool isInDB = false;
            if (data != null)
            {
                isInDB = true;
            }

            return isInDB;
        }

        /// <summary>
        /// Check if exist an ActivationAccount saved with username
        /// </summary>
        /// <param name="username">username saved</param>
        /// <returns>true if exist in DB</returns>
        /// <returns>false otherwise</returns>
        private bool IsUserNameSaved(string username)
        {
            var data = _context.ActivationAccount.Where(x => x.UserName == username).FirstOrDefault();
            bool isInDB = false;
            if (data != null)
            {
                isInDB = true;
            }

            return isInDB;
        }
    }
}
