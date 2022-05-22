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
        /// <summary>
        /// Export certificate file
        /// </summary>
        /// <param name="certName">X509Certificate2 certificate</param>
        /// <param name="pass">password</param>
        /// <returns>true if certificate was created</returns>
        /// <returns>false otherwise</returns>
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

        /// <summary>
        /// Create a certificate
        /// </summary>
        /// <param name="certName"></param>
        /// <returns>X509Certificate2 certificate</returns>
        private static X509Certificate2 GenerateRsaCertificate(string certName)
        {
            var hashAlgorithm = HashAlgorithmName.SHA256;
            var rsaKey = RSA.Create(2048);
            var subject = new X500DistinguishedName($"CN={certName}");
            var request = new CertificateRequest(subject, rsaKey, hashAlgorithm, RSASignaturePadding.Pkcs1);
            var certificate = request.CreateSelfSigned(DateTime.Now - TimeSpan.FromDays(5), DateTime.Now + TimeSpan.FromDays(365));
            return certificate;
        }

        /// <summary>
        /// Create the certificate and liteDB file folder
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns>True if the folder was created</returns>
        /// <returns>False otherwise</returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="Account"></param>
        /// <param name="Rights"></param>
        /// <param name="ControlType"></param>
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

        /// <summary>
        /// Remove 
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="Account"></param>
        /// <param name="Rights"></param>
        /// <param name="ControlType"></param>
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
