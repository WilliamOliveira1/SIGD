using SMTPConfig.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGD.Interfaces
{
    public interface ISIGDLiteDBHelper
    {
        /// <summary>
        /// Load data from liteDB
        /// </summary>
        /// <returns>SMTP Configuration Data</returns>
        SMTPConfigData GetSMPTConfigData();
    }
}
