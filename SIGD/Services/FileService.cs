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
            List<Tuple<byte[], string>> filesBytes = new List<Tuple<byte[], string>>();
            ActivationAccount accountUserUpload = new ActivationAccount();
            List<ActivationAccount> accountUsersToRead = new List<ActivationAccount>();
            List<Tuple<bool, string>> statusList = new List<Tuple<bool, string>>();
            try
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            filesBytes.Add(new Tuple<byte[], string>(ms.ToArray(), file.FileName));
                            //var fileBytes = ms.ToArray();
                            //string s = Convert.ToBase64String(fileBytes);
                            // act on the Base64 data
                        }
                    }
                }
                var listOfPrincipals = databaseAccountService.GetAllPrincipalsAccountsByAdmin(userUpload);
                var userUploadAccount = databaseAccountService.GetActivationAccountByUserName(userUpload);
                foreach(var user in usersToRead)
                {
                    accountUsersToRead.Add(listOfPrincipals.Where(x => x.Email == user).FirstOrDefault());
                }                

                List<FileModel> filesList = new List<FileModel>();
                
                foreach (var file in filesBytes)
                {
                    statusList.Add(new Tuple<bool, string>(databaseService.Save(new FileModel()
                    {
                        UsersToRead = Newtonsoft.Json.JsonConvert.SerializeObject(accountUsersToRead),
                        UserUpload = userUploadAccount,
                        FileName = file.Item2,
                        FileData = file.Item1
                    }), file.Item2));
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
