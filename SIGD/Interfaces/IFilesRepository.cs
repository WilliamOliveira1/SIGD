using SIGD.Models;
using System;
using System.Collections.Generic;

namespace SIGD.Interfaces
{
    public interface IFilesRepository
    {
        bool Save(FileModel data);

        FileModel GetFileByFileName(string fileName);

        FileModel GetFileById(Guid fileId);

        List<FileModel> GetAllFiles();
    }
}
