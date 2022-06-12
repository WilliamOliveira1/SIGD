﻿using Microsoft.AspNetCore.Http;
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
    }
}
