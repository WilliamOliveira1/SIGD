using Microsoft.Extensions.Configuration;
using SIGD.Data;
using SIGD.Interfaces;
using SIGD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGD.Services
{
    public class PrincipalFileModelViewRepository : IPrincipalFileModelViewRepository
    {
        private ApplicationDbContext _context;
        public PrincipalFileModelViewRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Save model data in data base
        /// </summary>
        /// <param name="data"></param>
        /// <returns>true if data saved</returns>
        /// <returns>false otherwise</returns>
        public bool Save(PrincipalFileModelView data)
        {
            try
            {
                var modelData = GetFileById(data.Id);
                if (modelData == null)
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
        /// Save model data in data base
        /// </summary>
        /// <param name="data"></param>
        /// <returns>true if data saved</returns>
        /// <returns>false otherwise</returns>
        public bool SaveList(List<PrincipalFileModelView> data)
        {
            bool haveInDb = false;
            try
            {
                foreach (var row in data)
                {
                    var modelData = GetFileById(row.Id);
                    if (modelData != null)
                    {
                        haveInDb = true;
                    }
                }

                if (!haveInDb)
                {
                    foreach (var row in data)
                    {
                        _context.Add(row);
                        _context.SaveChanges();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get File by file name
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>FilesModel data</returns>
        public PrincipalFileModelView GetDatabyPrincipal(string principal)
        {
            return _context.FilesViewContext.Where(x => x.PrincipalName == principal).FirstOrDefault();
        }        

        public List<PrincipalFileModelView> GetAllFilesViewModel()
        {
            return _context.FilesViewContext.ToList();
        }

        /// <summary>
        /// Get PrincipalFileModelView data that contains fileViewId
        /// </summary>
        /// <param name="fileViewId">fileviewwModel Id</param>
        /// <returns>PrincipalFileModelView data</returns>
        private PrincipalFileModelView GetFileById(Guid fileViewId)
        {
            return _context.FilesViewContext.Where(x => x.Id == fileViewId).FirstOrDefault();
        }
    }
}
