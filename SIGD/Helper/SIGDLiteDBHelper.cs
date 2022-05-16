﻿using LiteDB;
using SMTPConfig.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SIGD.Helper
{
    public class SIGDLiteDBHelper
    {
        public static string liteDBpath = Path.Combine((Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)), "SGID");
        public string liteDBFilepath = Path.Combine(liteDBpath, "SIGD.db");

        public SMTPConfigData GetSMPTConfigData()
        {
            SMTPConfigData config = new SMTPConfigData();

            if (Directory.Exists(liteDBpath))
                using (var db = new LiteDatabase(liteDBFilepath))
                {
                    var collection = db.GetCollection<SMTPConfigData>("STMPConfigData");
                    config = collection.FindById(1);
                }

            return config;
        }
    }
}
