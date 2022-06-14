using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SIGD.Interfaces;
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
                    string userName = User.Identity.Name;
                    var listOfSavedFiles = fileService.SaveFile(files, userName, emails);
                    return Ok(listOfSavedFiles);
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
                var files = fileService.GetFiles();
                return Ok(files);
            }
            catch (Exception e)
            {
                return BadRequest($"Process Error: {e.Message}"); // Oops!
            }
        }

        [HttpPost("download")]
        public async Task<IActionResult> Download([FromBody] JObject input)
        {
            try
            {
                //filename = "_leiame.txt";
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
                    return File(memory, GetContentType(path), Path.GetFileName(path));
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

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},  
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}
