using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIGD.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    }
}
