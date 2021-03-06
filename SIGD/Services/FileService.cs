using Microsoft.AspNetCore.Http;
using SIGD.Interfaces;
using SIGD.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;

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
                                var principalAccount = listOfPrincipals.Where(x => x.Email == principalEmail).FirstOrDefault();
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

        public List<FileModel> GetFilesByPrincipalUsername(string email)
        {
            try
            {
                ActivationAccount user = new ActivationAccount();
                if (IsValidEmail(email))
                {
                    user = databaseAccountService.GetActivationAccountByEmail(email);
                }
                else
                {
                    user = databaseAccountService.GetActivationAccountByUserName(email);
                }
                
                List<SupervisorFileModelView> supervisorFileModelView = new List<SupervisorFileModelView>();                
                List<PrincipalFileModelView> filesViewModel = fileViewModeldatabaseRepository.GetAllFilesViewModel();
                filesViewModel = filesViewModel.Where(x => x.SupervisorName == user.UserName).ToList();                
                List<FileModel> fileModels = new List<FileModel>();

                var allFiles = databaseService.GetAllFiles().ToList();
                List<PrincipalFileModelView> principalFilesData = new List<PrincipalFileModelView>();
                foreach (var file in allFiles)
                {
                    principalFilesData.AddRange(file.PrincipalsFiles);
                }

                principalFilesData = principalFilesData.Where(x => x.PrincipalName == user.UserName).ToList();
                foreach (var file in principalFilesData)
                {
                    fileModels.Add(file.FileModel);
                }

                return fileModels;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return null;
        }

        public List<PrincipalFileModelView> GetFilesByPrincipalUsername1(string email)
        {
            try
            {
                List<PrincipalFileModelView> filesViewModel = fileViewModeldatabaseRepository.GetAllFilesViewModel();
                if (IsValidEmail(email))
                {
                    filesViewModel = filesViewModel.Where(x => x.PrincipalEmail == email).ToList();
                }
                else
                {
                    filesViewModel = filesViewModel.Where(x => x.PrincipalName == email).ToList();
                }

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

        public List<Tuple<string, List<PrincipalFileModelView>>> GetDataChart(string username)
        {
            try
            {
                List<string> principalsName = new List<string>();
                List<SupervisorFileModelView> supervisorFileModelView = new List<SupervisorFileModelView>();
                ActivationAccount user = databaseAccountService.GetActivationAccountByUserName(username);
                List<PrincipalFileModelView> filesViewModel = fileViewModeldatabaseRepository.GetAllFilesViewModel();
                filesViewModel = filesViewModel.Where(x => x.SupervisorName == user.UserName).ToList();
                List<ActivationAccount> principalsAccounts = databaseAccountService.GetAllPrincipalsAccountsByAdmin(username);
                List<Tuple<string, List<PrincipalFileModelView>>> principalsViewModel = new List<Tuple<string, List<PrincipalFileModelView>>>();

                var allFiles = databaseService.GetAllFiles().ToList();
                List<PrincipalFileModelView> principalFilesData = new List<PrincipalFileModelView>();
                foreach (var file in allFiles)
                {
                    principalFilesData.AddRange(file.PrincipalsFiles);
                }
                

                foreach (var principal in principalsAccounts)
                {
                    principalsName.Add(principal.UserName);
                }
                //filesViewModel.Where(x => x.PrincipalName == name).ToList()
                foreach (var name in principalsName)
                {
                    principalsViewModel.Add(new Tuple<string, List<PrincipalFileModelView>>(name, principalFilesData.Where(x => x.PrincipalName == name).ToList()));
                }
                return principalsViewModel;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return null;
        }

        public List<Tuple<string, List<PrincipalFileModelView>>> GetDataChartPrincipal(string username)
        {
            try
            {
                List<SupervisorFileModelView> supervisorFileModelView = new List<SupervisorFileModelView>();
                ActivationAccount user = databaseAccountService.GetActivationAccountByUserName(username);
                List<PrincipalFileModelView> filesViewModel = fileViewModeldatabaseRepository.GetAllFilesViewModel();
                filesViewModel = filesViewModel.Where(x => x.PrincipalName == user.UserName).ToList();
                List<ActivationAccount> principalsAccounts = databaseAccountService.GetAllPrincipalsAccountsByAdmin(username);
                List<Tuple<string, List<PrincipalFileModelView>>> principalsViewModel = new List<Tuple<string, List<PrincipalFileModelView>>>();
                var allFiles = databaseService.GetAllFiles().ToList();

                var filesViewModelReaded = filesViewModel.Where(x => x.Status == true).ToList();
                var filesViewModelNotReaded = filesViewModel.Where(x => x.Status == false).ToList();

                principalsViewModel.Add(new Tuple<string, List<PrincipalFileModelView>>("Não lido", filesViewModelNotReaded));
                principalsViewModel.Add(new Tuple<string, List<PrincipalFileModelView>>("Lido", filesViewModelReaded));

                return principalsViewModel;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return null;
        }

        public bool ChangeReadingStatus(string filename)
        {
            bool saveStatus = false;
            DateTime dateTime = DateTime.Now;
            string fusoHorarioId = "E. South America Standard Time";
            TimeZoneInfo Standard_Time = TimeZoneInfo.FindSystemTimeZoneById(fusoHorarioId);
            DateTime dataHoraLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Standard_Time);
            try
            {
                List<PrincipalFileModelView> filesViewModel = fileViewModeldatabaseRepository.GetAllFilesViewModel();
                var test = databaseService.GetAllFiles().Where(x => x.FileName == filename).ToList();
                var fileModelView = filesViewModel.Where(x => x.FileModel.FileName == filename).FirstOrDefault();
                fileModelView.Status = true;
                fileModelView.LastTimeOpened = dataHoraLocal;
                saveStatus = fileViewModeldatabaseRepository.Save(fileModelView);
            }
            catch (Exception)
            {
                saveStatus = false;
            }
            return saveStatus;
        }

        public bool SaveFileQuestion(string filename, string message)
        {
            bool saveStatus = false;
            DateTime dateTime = DateTime.Now;
            try
            {
                List<PrincipalFileModelView> filesViewModel = fileViewModeldatabaseRepository.GetAllFilesViewModel();
                var test = databaseService.GetAllFiles().Where(x => x.FileName == filename).ToList();
                var fileModelView = filesViewModel.Where(x => x.FileModel.FileName == filename).FirstOrDefault();                
                fileModelView.Question = message;
                saveStatus = fileViewModeldatabaseRepository.Save(fileModelView);
            }
            catch (Exception)
            {
                saveStatus = false;
            }
            return saveStatus;
        }

        public bool SaveFileAnswer(string filename, string message)
        {
            bool saveStatus = false;
            DateTime dateTime = DateTime.Now;
            try
            {
                List<PrincipalFileModelView> filesViewModel = fileViewModeldatabaseRepository.GetAllFilesViewModel();
                var test = databaseService.GetAllFiles().Where(x => x.FileName == filename).ToList();
                var fileModelView = filesViewModel.Where(x => x.FileModel.FileName == filename).FirstOrDefault();
                fileModelView.Answer = message;
                saveStatus = fileViewModeldatabaseRepository.Save(fileModelView);
            }
            catch (Exception)
            {
                saveStatus = false;
            }
            return saveStatus;
        }

        public string GetContentType(string path)
        {
            try
            {
                var types = GetMimeTypes();
                var ext = Path.GetExtension(path).ToLowerInvariant();
                return !string.IsNullOrEmpty(ext) ? types[ext] : ext;
            }
            catch (Exception)
            {
                return "";
            }            
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

        private bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
