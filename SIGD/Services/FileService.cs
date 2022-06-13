using Microsoft.AspNetCore.Http;
using SIGD.Interfaces;
using SIGD.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SIGD.Services
{
    public class FileService : IFileService
    {
        private IFilesRepository databaseService;
        private IActivationAccountRepository databaseAccountService;

        public FileService(IFilesRepository databaseService, IActivationAccountRepository databaseAccountService)
        {
            this.databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            this.databaseAccountService = databaseAccountService ?? throw new ArgumentNullException(nameof(databaseAccountService));
        }

        public List<Tuple<bool, string>> SaveFile(IFormFileCollection files, string userUpload, List<string> usersToRead)
        {            
            ActivationAccount accountUserUpload = new ActivationAccount();
            List<ActivationAccount> accountUsersToRead = new List<ActivationAccount>();
            List<Tuple<bool, string>> statusList = new List<Tuple<bool, string>>();            
            try
            {                
                var listOfPrincipals = databaseAccountService.GetAllPrincipalsAccountsByAdmin(userUpload);
                var userUploadAccount = databaseAccountService.GetActivationAccountByUserName(userUpload);
                foreach(var user in usersToRead)
                {
                    accountUsersToRead.Add(listOfPrincipals.Where(x => x.Email == user).FirstOrDefault());
                }                

                List<string> filesList = new List<string>();

                foreach (var file in files)
                {
                    string filePath = string.Empty;
                    if (file.Length > 0)
                    {
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Files", file.FileName);                        
                        using (var ms = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(ms);                            
                            statusList.Add(new Tuple<bool, string>(databaseService.Save(new FileModel()
                            {
                                UsersToRead = Newtonsoft.Json.JsonConvert.SerializeObject(accountUsersToRead),
                                UserUpload = userUploadAccount,
                                FileName = file.FileName,
                                FilePath = filePath
                            }), file.FileName));
                            filesList.Add(filePath);
                            //var fileBytes = ms.ToArray();
                            //string s = Convert.ToBase64String(fileBytes);
                            // act on the Base64 data
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }            

            return statusList;
        }

        public List<FileModel> GetFiles()
        {
            try
            {
                List<FileModel> files = new List<FileModel>();
                files = databaseService.GetAllFiles();
                return files;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return null;
        }       
    }
}
