using Microsoft.AspNetCore.Http;
using SIGD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGD.Interfaces
{
    public interface IFileService
    {
        List<Tuple<bool, string>> SaveFile(IFormFileCollection files, string userUpload, List<string> usersToRead);

        List<FileModel> GetFiles();

        List<PrincipalFileModelView> GetFilesByPrincipalUsername1(string username);
        List<FileModel> GetFilesByPrincipalUsername(string username);

        List<FileModel> GetFilesBySupervisorUsername(string username);
        bool ChangeReadingStatus(string filename);
        bool SaveFileQuestion(string filename, string message);
        bool SaveFileAnswer(string filename, string message);

        List<Tuple<string, List<PrincipalFileModelView>>> GetDataChart(string username);

        List<Tuple<string, List<PrincipalFileModelView>>> GetDataChartPrincipal(string username);

        string GetContentType(string path);

        Dictionary<string, string> GetMimeTypes();

        List<string> PermitedTypes();
    }
}
