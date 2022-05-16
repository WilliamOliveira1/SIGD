using System;
using System.IO;
using System.Security;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SMTPConfig.Helpers
{
    public class CreateCert
    {
        public bool CreateCertificate(string certName, SecureString pass)
        {
            try
            {
                string certPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SGID");
                string certFilePath = Path.Combine(certPath, $"{certName}.pfx");
                if(!Directory.Exists(certPath))
                {
                    CreateLiteDBFolder(certPath);
                }

                if(Directory.Exists(certPath))
                {
                    if (Directory.Exists(certFilePath))
                    {
                        return true;
                    }
                    else
                    {
                        //generate a selfSigned cert and obtain the privateKey
                        X509Certificate2 MyRootCAcert = GenerateRsaCertificate(certName);
                        //Export as pfx with privatekey
                        byte[] certData = MyRootCAcert.Export(X509ContentType.Pfx, pass);
                        File.WriteAllBytes(certFilePath, certData);
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return false;
            }
        }

        private static X509Certificate2 GenerateRsaCertificate(string certName)
        {
            var hashAlgorithm = HashAlgorithmName.SHA256;
            var rsaKey = RSA.Create(2048);
            var subject = new X500DistinguishedName($"CN={certName}");
            var request = new CertificateRequest(subject, rsaKey, hashAlgorithm, RSASignaturePadding.Pkcs1);
            var certificate = request.CreateSelfSigned(DateTime.Now - TimeSpan.FromDays(5), DateTime.Now + TimeSpan.FromDays(365));
            return certificate;
        }

        public bool CreateLiteDBFolder(string folderPath)
        {
            bool isCreated = false;

            // If directory does not exist, create it
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                // Add the access control entry to the directory.
                var test = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                var cUser = "IIS_IUSRS";
                AddDirectorySecurity(folderPath, cUser, FileSystemRights.Write, AccessControlType.Allow);
                // Remove the access control entry from the directory.
                //RemoveDirectorySecurity(DirectoryName, @"IIS AppPool\myAppPool", FileSystemRights.Write, AccessControlType.Allow);
                isCreated = true;
            }
            else
            {
                isCreated = true;
            }

            return isCreated;
        }

        public static void AddDirectorySecurity(string FileName, string Account, FileSystemRights Rights, AccessControlType ControlType)
        {
            // Create a new DirectoryInfo object.
            DirectoryInfo dInfo = new DirectoryInfo(FileName);

            // Get a DirectorySecurity object that represents the 
            // current security settings.
            DirectorySecurity dSecurity = dInfo.GetAccessControl();

            // Add the FileSystemAccessRule to the security settings. 
            dSecurity.AddAccessRule(new FileSystemAccessRule(Account,
            Rights,
            ControlType));

            // Set the new access settings.
            dInfo.SetAccessControl(dSecurity);
        }

        public static void RemoveDirectorySecurity(string FileName, string Account, FileSystemRights Rights, AccessControlType ControlType)
        {
            // Create a new DirectoryInfo object.
            DirectoryInfo dInfo = new DirectoryInfo(FileName);

            // Get a DirectorySecurity object that represents the 
            // current security settings.
            DirectorySecurity dSecurity = dInfo.GetAccessControl();

            // Add the FileSystemAccessRule to the security settings. 
            dSecurity.RemoveAccessRule(new FileSystemAccessRule(Account,
            Rights,
            ControlType));

            // Set the new access settings.
            dInfo.SetAccessControl(dSecurity);
        }
    }
}
