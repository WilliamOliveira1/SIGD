using System;
using SMTPConfig.Models;
using System.Collections.Generic;
using System.Text;
using LiteDB;
using System.IO;
using System.Security.AccessControl;

namespace SMTPConfig
{
    public class LiteDBHelper
    {
        public static string liteDBpath = Path.Combine((Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)), "SGID");
        public string liteDBFilepath = Path.Combine(liteDBpath, "SIGD.db");

        /// <summary>
        /// Save the STMPConfigData table
        /// </summary>
        /// <param name="config"></param>
        /// <returns>true if saved</returns>
        /// <returns>false otherwise</returns>
        public bool Save(SMTPConfigData config)
        {
            bool isSaved = false;
            bool isFolderCreated = false;

            if (Directory.Exists(liteDBpath))
            {
                isFolderCreated = true;
            }

            if (isFolderCreated)
            {
                using (var db = new LiteDatabase(liteDBFilepath))
                {
                    var collection = db.GetCollection<SMTPConfigData>("STMPConfigData");
                    collection.Insert(config);
                    isSaved = true;
                }
            }

            return isSaved;
        }

        /// <summary>
        /// Update the STMPConfigData table
        /// </summary>
        /// <param name="config"></param>
        /// <returns>true if updated</returns>
        /// <returns>false otherwise</returns>
        public bool UpdateSMPTConfigData(SMTPConfigData config)
        {
            bool isSaved = false;

            using (var db = new LiteDatabase(liteDBFilepath))
            {
                var collection = db.GetCollection<SMTPConfigData>("STMPConfigData");
                var author = collection.FindById(1);
                collection.Update(config);
                isSaved = true;
            }

            return isSaved;
        }

        /// <summary>
        /// Load data from liteDB
        /// </summary>
        /// <returns>SMTP Configuration Data</returns>
        public SMTPConfigData GetSMPTConfigData()
        {
            SMTPConfigData config = new SMTPConfigData();

            if(Directory.Exists(liteDBFilepath))
                using (var db = new LiteDatabase(liteDBFilepath))
                {
                    var collection = db.GetCollection<SMTPConfigData>("STMPConfigData");
                    config = collection.FindById(1);
                }

            return config;
        }        
    }
}
