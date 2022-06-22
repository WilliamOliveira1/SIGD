using Microsoft.AspNetCore.Http;
using SIGD.Interfaces;
using SIGD.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SIGD.Services
{
    public class FileService : IFileService
    {
        private IFilesRepository databaseService;
        private IActivationAccountRepository databaseAccountService;
        private IPrincipalFileModelViewRepository fileViewModeldatabaseRepository;

        public FileService(IFilesRepository databaseService, IActivationAccountRepository databaseAccountService, IPrincipalFileModelViewRepository fileViewModeldatabaseRepository)
        {
            this.databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            this.databaseAccountService = databaseAccountService ?? throw new ArgumentNullException(nameof(databaseAccountService));
            this.fileViewModeldatabaseRepository = fileViewModeldatabaseRepository ?? throw new ArgumentNullException(nameof(fileViewModeldatabaseRepository));
        }

        public List<Tuple<bool, string>> SaveFile(IFormFileCollection files, string userUpload, List<string> usersToRead)
        {            
            ActivationAccount accountUserUpload = new ActivationAccount();
            List<PrincipalFileModelView> accountUsersToRead = new List<PrincipalFileModelView>();
            List<Tuple<bool, string>> statusList = new List<Tuple<bool, string>>();
            List<Tuple<bool, string>> listCheck = new List<Tuple<bool, string>>();
            try
            {                
                var listOfPrincipals = databaseAccountService.GetAllPrincipalsAccountsByAdmin(userUpload);
                var userUploadAccount = databaseAccountService.GetActivationAccountByUserName(userUpload);                
                
                List<string> filesList = new List<string>();

                var filesInDB = databaseService.GetAllFiles();
                var filesSavedBySupervisor = filesInDB.Where(x => x.UserUpload == userUploadAccount).ToList();

                foreach (var file in files)
                {
                    if (filesSavedBySupervisor.Where(y => y.FileName == file.FileName).ToList().Count() > 0)
                    {
                        statusList.Add(new Tuple<bool, string>(false, $"File name already in DB.{file.FileName}"));
                        return statusList;
                    }

                    string filePath = string.Empty;
                    if (file.Length > 0)
                    {
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Files", file.FileName);
                        using (var ms = new FileStream(filePath, FileMode.Create))
                        {                            
                            file.CopyTo(ms);
                            var fileSaved = databaseService.Save(new FileModel()
                            {
                                UserUpload = userUploadAccount,
                                FileName = file.FileName,
                                FilePath = filePath
                            });
                            bool isSaved = fileSaved != null ? true : false;                            
                            filesList.Add(filePath);

                            foreach (var principalEmail in usersToRead)
                            {
                                ActivationAccount principalAccount = listOfPrincipals.Where(x => x.Email == principalEmail).FirstOrDefault();
                                PrincipalFileModelView fileModelView = new PrincipalFileModelView()
                                {
                                    PrincipalName = principalAccount.UserName,
                                    PrincipalEmail = principalAccount.Email,
                                    Status = false,
                                    FileModel = fileSaved,
                                    SupervisorName = userUploadAccount.UserName
                                };

                                accountUsersToRead.Add(fileModelView);
                            }
                        }
                    }
                }

                List<string> getRepetead = new List<string>();
                foreach (var viewModel in accountUsersToRead)
                {
                    bool saveViewModel = fileViewModeldatabaseRepository.Save(viewModel);
                    getRepetead.Add(viewModel.FileModel.FileName);
                    if(getRepetead.Where(x => x.Contains(viewModel.FileModel.FileName)).ToList().Count() == 1 || !saveViewModel)
                    {
                        statusList.Add(new Tuple<bool, string>(saveViewModel, viewModel.FileModel.FileName));
                    }                    
                }
                var getListNotSaved = statusList.Where(x => x.Item1 == false).ToList();
                if(getListNotSaved.Count > 0)
                {
                    foreach (var itemNotSaved in accountUsersToRead)
                    {
                        databaseService.DeleteFileById(itemNotSaved.FileModel.Id);
                    }
                    return null;
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
                return databaseService.GetAllFiles();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return null;
        }

        public List<PrincipalFileModelView> GetFilesByPrincipalUsername(string email)
        {
            try
            {        
                List<PrincipalFileModelView> filesViewModel = fileViewModeldatabaseRepository.GetAllFilesViewModel();
                filesViewModel = filesViewModel.Where(x => x.PrincipalEmail == email).ToList();
                var test = databaseService.GetAllFiles().ToList();
                List<PrincipalFileModelView> test1 = new List<PrincipalFileModelView>();
                foreach (var t in test)
                {
                    test1.AddRange(t.PrincipalsFiles);
                }
                return test1.Where(x => x.PrincipalEmail == email).ToList();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return null;
        }

        public List<FileModel> GetFilesBySupervisorUsername(string username)
        {
            try
            {
                List<SupervisorFileModelView> supervisorFileModelView = new List<SupervisorFileModelView>();
                ActivationAccount user = databaseAccountService.GetActivationAccountByUserName(username);
                List<PrincipalFileModelView> filesViewModel = fileViewModeldatabaseRepository.GetAllFilesViewModel();
                filesViewModel = filesViewModel.Where(x => x.SupervisorName == user.UserName).ToList();
                var test = databaseService.GetAllFiles().Where(x => x.UserUpload == user).ToList();
                return test;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return null;
        }

        public string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return !string.IsNullOrEmpty(ext) ? types[ext] : ext;
        }

        public Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/octet-stream"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".cs", "text/plain.cs"}
            };
        }

        public List<string> PermitedTypes()
        {
            return new List<string>
            {
                "text/plain",
                "application/pdf",
                "application/octet-stream",
            };
        }     
    }
}
