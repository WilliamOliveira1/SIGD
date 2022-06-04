using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SIGD.Helper
{
    public class SharedPaths
    {
        public static string FIRST_PASS_ACCESS_EMAIL_ADMIN = Path.GetFullPath("EmailsTemplates/AdminFirstPassAccess.html");
        public static string FIRST_PASS_ACCESS_EMAIL_PRINCIPAL = Path.GetFullPath("EmailsTemplates/PrincipalFirstPassAccess.html");
        public static string ACCOUNT_ACTIVATED_EMAIL_TEMPLATE = Path.GetFullPath("EmailsTemplates/AccountActivated.html");
    }
}
