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
                    accountUsersToRead.Add(listOfPrincipals.Where(x => x.UserName == user).FirstOrDefault());
                }

                FileModel fileModel = new FileModel() 
                { 
                    UsersToRead = accountUsersToRead,
                    UserUpload = userUploadAccount
                };

                foreach (var file in filesBytes)
                {
                    fileModel.FileName = file.Item2;
                    fileModel.FileData = file.Item1;
                    statusList.Add(new Tuple<bool, string>(databaseService.Save(fileModel), file.Item2));
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }            

            return statusList;
        }
    }
}
