using SIGD.Models;
using System;
using System.Collections.Generic;

namespace SIGD.Interfaces
{
    public interface IFilesRepository
    {
        FileModel Save(FileModel data);

        FileModel GetFileByFileName(string fileName);

        FileModel GetFileById(Guid fileId);

        List<FileModel> GetAllFiles();

        bool SaveList(List<FileModel> data);

        /// <summary>
        /// Remove file model from database
        /// </summary>
        /// <param name="filename">name and extension of file</param>
        /// <returns>true if removed and false otherwise</returns>
        bool DeleteByFileName(string filename);

        /// <summary>
        /// Remove file model from database
        /// </summary>
        /// <param name="id">id of file model</param>
        /// <returns>true if removed and false otherwise</returns>
        bool DeleteFileById(Guid id);
    }
}
