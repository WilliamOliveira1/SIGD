using Microsoft.Extensions.Configuration;
using SIGD.Data;
using SIGD.Interfaces;
using SIGD.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SIGD.Services
{
    public class FilesRepository : IFilesRepository
    {
        private ApplicationDbContext _context;
        public IConfiguration Configuration { get; }
        public FilesRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Save model data in data base
        /// </summary>
        /// <param name="data"></param>
        /// <returns>true if data saved</returns>
        /// <returns>false otherwise</returns>
        public bool Save(FileModel data)
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
        public bool SaveList(List<FileModel> data)
        {
            bool haveInDb = false;
            try
            {
                foreach (var row in data)
                {
                    var modelData = GetFileById(row.Id);
                    if(modelData != null)
                    {
                        haveInDb = true;
                    }
                }

                if (!haveInDb)
                {
                    foreach (var row in data)
                    {
                        _context.AddAsync(row);                        
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
        public FileModel GetFileByFileName(string fileName)
        {
            return _context.FilesContext.Where(x => x.FileName == fileName).FirstOrDefault();
        }

        /// <summary>
        /// Get ActivationAccount data that contains username
        /// </summary>
        /// <param name="username">username saved</param>
        /// <returns>ActivationAccount data</returns>
        public FileModel GetFileById(Guid fileId)
        {
            return _context.FilesContext.Where(x => x.Id == fileId).FirstOrDefault();
        }

        public List<FileModel> GetAllFiles()
        {
           return _context.FilesContext.ToList();
        }       
    }
}
