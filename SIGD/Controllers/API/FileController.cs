using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SIGD.Helper;
using SIGD.Interfaces;
using SIGD.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace SIGD.Controllers.API
{
    [Route("api/filemanager")]
    [Produces("application/json")]
    [ApiController]
    public class FileController : Controller
    {
        private IFileService fileService;
        public FileController(IFileService fileService)
        {
            this.fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }


        [HttpPost("savefiles")]
        public ActionResult SaveFiles()
        {
            List<string> emails = new List<string>();
            try
            {
                IFormFileCollection files = HttpContext.Request?.Form?.Files;
                var dict = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
                var emailsJoined = dict.Last().Value;
                var emailArray = emailsJoined.Split(',');
                bool notPermitedType = false;
                foreach(var email in emailArray)
                {
                    emails.Add(email);
                }                
                
                if (files.Count() == 0)
                {
                    return BadRequest("Process Error: No file submitted");
                }
                else
                {
                    notPermitedType = GetPermitedTypes(files);

                    if (!notPermitedType)
                    {
                        List<string> message = new List<string>()
                        {
                            "Um ou mais arquivos com extensão não permitida, por favor tente novamente.<br>",
                            "Formatos permitidos: <br>",
                            ".txt, .pdf <br>"
                        };
                        return BadRequest(message);
                    }
                    
                    string userName = User.Identity.Name;
                    var listOfSavedFiles = fileService.SaveFile(files, userName, emails);
                    if(listOfSavedFiles.Where(x => x.Item2.Contains("File name already in DB.")).ToList().Count > 0)
                    {
                        var error = listOfSavedFiles.Where(x => x.Item2.Contains("File name already in DB.")).FirstOrDefault();
                        string[] fileNameError = error.Item2.Split(".");
                        return BadRequest($"Arquivo com nome salvo no banco, por favor mude o nome do arquivo: [ {fileNameError[1]} ] e tente novamente:");
                    }

                    bool filesWasSaved = listOfSavedFiles != null && listOfSavedFiles.Count > 0 ? true : false;
                    return Ok(filesWasSaved);
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Process Error: {e.Message}"); // Oops!
            }
        }

        [HttpGet("getfilesbyuser")]
        public ActionResult GetFiles()
        {
            try
            {
                string username = User.Identity.Name;
                var files = fileService.GetFilesBySupervisorUsername(username);
                var test = JsonConvert.SerializeObject(files, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                return Ok(test);
            }
            catch (Exception e)
            {
                return BadRequest($"Process Error: {e.Message}"); // Oops!
            }
        }

        [HttpPost("getfilesbyprincipal")]
        public ActionResult GetFilesbyPrincipal([FromBody] JObject input)
        {
            try
            {
                string useremail = input?["useremail"]?.ToString();
                var files = fileService.GetFilesByPrincipalUsername(useremail);
                return Ok(JsonConvert.SerializeObject(files, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }));
            }
            catch (Exception e)
            {
                return BadRequest($"Process Error: {e.Message}"); // Oops!
            }
        }

        [HttpPost("getfilesbyprincipalUsername")]
        public ActionResult GetFilesbyPrincipalUsername([FromBody] JObject input)
        {
            try
            {
                string useremail = input?["username"]?.ToString();
                var files = fileService.GetFilesByPrincipalUsername(useremail);
                return Ok(JsonConvert.SerializeObject(files, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }));
            }
            catch (Exception e)
            {
                return BadRequest($"Process Error: {e.Message}"); // Oops!
            }
        }

        [HttpPost("changeReadingStatus")]
        public ActionResult ChangeReadingStatus([FromBody] JObject input)
        {
            try
            {
                string filename = input?["filename"]?.ToString();
                var status = fileService.ChangeReadingStatus(filename);
                if (!status)
                    return BadRequest($"{SharedMessages.ERROR_SAVING_READING_STATUS}");
                else                    
                    return Ok(SharedMessages.READING_STATUS_SAVED);
            }
            catch (Exception)
            {
                return BadRequest($"{SharedMessages.ERROR_SAVING_READING_STATUS}");
            }
        }

        [HttpPost("sendFileQuestion")]
        public ActionResult SendFileQuestion([FromBody] JObject input)
        {
            try
            {
                string filename = input?["filename"]?.ToString();
                string message = input?["message"]?.ToString();
                var status = fileService.SaveFileQuestion(filename, message);
                if (!status)
                    return BadRequest($"{SharedMessages.ERROR_SAVING_READING_STATUS}");
                else
                    return Ok(SharedMessages.READING_STATUS_SAVED);
            }
            catch (Exception)
            {
                return BadRequest($"{SharedMessages.ERROR_SAVING_READING_STATUS}");
            }
        }

        [HttpGet("getDataChart")]
        public ActionResult GetDataChart()
        {
            try
            {
                string username = User.Identity.Name;
                var dataChart = fileService.GetDataChart(username);
                if(dataChart != null)
                {
                    return Ok(JsonConvert.SerializeObject(dataChart, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }));
                }
                else
                {
                    return BadRequest($"{SharedMessages.ERROR_GET_CHART_DATA}");
                }
                
            }
            catch (Exception)
            {
                return BadRequest($"{SharedMessages.ERROR_GET_CHART_DATA}");
            }
        }

        [HttpGet("getDataChartPrincipal")]
        public ActionResult GetDataChartPrincipal()
        {
            try
            {
                string username = User.Identity.Name;
                var dataChart = fileService.GetDataChartPrincipal(username);
                if (dataChart != null)
                {
                    return Ok(JsonConvert.SerializeObject(dataChart, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }));
                }
                else
                {
                    return BadRequest($"{SharedMessages.ERROR_GET_CHART_DATA}");
                }

            }
            catch (Exception)
            {
                return BadRequest($"{SharedMessages.ERROR_GET_CHART_DATA}");
            }
        }

        [HttpPost("sendFileAnswer")]
        public ActionResult SendFileAnswer([FromBody] JObject input)
        {
            try
            {
                string filename = input?["filename"]?.ToString();
                string message = input?["message"]?.ToString();
                var status = fileService.SaveFileAnswer(filename, message);
                if (!status)
                    return BadRequest($"{SharedMessages.ERROR_SAVING_READING_STATUS}");
                else
                    return Ok(SharedMessages.READING_STATUS_SAVED);
            }
            catch (Exception)
            {
                return BadRequest($"{SharedMessages.ERROR_SAVING_READING_STATUS}");
            }
        }

        [HttpPost("download")]
        public async Task<IActionResult> Download([FromBody] JObject input)
        {
            try
            {
                string filename = input?["filename"]?.ToString();
                if (!string.IsNullOrEmpty(filename))
                {
                    var path = Path.Combine(
                                   Directory.GetCurrentDirectory(),
                                   @"wwwroot\Files", filename);

                    var memory = new MemoryStream();
                    using (var stream = new FileStream(path, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    return File(memory, fileService.GetContentType(path), Path.GetFileName(path));
                }
                else
                {
                    return StatusCode(404);
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return BadRequest();
            }            
        }        

        private bool GetPermitedTypes(IFormFileCollection files)
        {
            bool isPermited = false;
            foreach (var file in files)
            {
                var fileType = fileService.GetContentType(file.FileName);
                foreach (var type in fileService.PermitedTypes())
                {
                    if (file.ContentType == type && file.ContentType == fileType)
                    {
                        isPermited = true;
                    }

                    if (isPermited)
                    {
                        break;
                    }
                }

                if (isPermited)
                {
                    break;
                }
            }

            return isPermited;
        }
    }
}
