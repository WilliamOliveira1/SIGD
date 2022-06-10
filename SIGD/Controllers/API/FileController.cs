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
        public ActionResult SaveFiles(List<string> usersToRead)
        {            
            try
            {
                IFormFileCollection files = HttpContext.Request?.Form?.Files;
                if (files.Count() == 0)
                {
                    return BadRequest("Process Error: No file submitted");
                }
                else
                {
                    string userName = User.Identity.Name;
                    var listOfSavedFiles = fileService.SaveFile(files, userName, usersToRead);
                    return Ok(listOfSavedFiles);
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Process Error: {e.Message}"); // Oops!
            }
        }
    }
}
