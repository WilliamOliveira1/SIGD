using SIGD.Interfaces;
using SMTPConfig.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SIGD.Helper
{
    public class EmailSender : IEmailSender
    {
        private ISIGDLiteDBHelper liteDBHelper;
        public EmailSender(ISIGDLiteDBHelper liteDBHelper)
        {
            this.liteDBHelper = liteDBHelper ?? throw new ArgumentNullException(nameof(liteDBHelper));
        }

        /// <summary>
        /// Send email with first access password
        /// </summary>
        /// <param name="emailAdressTo">email address</param>
        /// <param name="mailBody">HTML with the email body</param>
        public bool SendEmailFirstAccess(string emailAdressTo, SecureString tokenFirstAccess,string mailBody)
        {
            SMTPConfigData config = liteDBHelper.GetSMPTConfigData();

            string emailAdressFrom = config.Email;
            string displayNameFrom = "Projeto IntegradorII";

            MailAddress emailFrom = new MailAddress(emailAdressFrom, displayNameFrom);
            MailAddress emailTo = new MailAddress(emailAdressTo);

            MailMessage mailMessage = new MailMessage()
            {
                From = emailFrom,
                IsBodyHtml = true,
                Subject = "SIGD first password access",
                Body = mailBody
            };

            mailMessage.To.Add(emailTo);

            try
            {
                GetSTMPData().Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Send email with first access password
        /// </summary>
        /// <param name="emailAdressTo">email address</param>
        /// <param name="mailBody">HTML with the email body</param>
        public bool SendEmailPasswordChanged(string emailAdressTo, string mailBody)
        {
            SMTPConfigData config = liteDBHelper.GetSMPTConfigData();

            string emailAdressFrom = config.Email;
            string displayNameFrom = "Projeto IntegradorII";

            MailAddress emailFrom = new MailAddress(emailAdressFrom, displayNameFrom);
            MailAddress emailTo = new MailAddress(emailAdressTo);

            mailBody = "<h1>" + mailBody + "</h1>";

            MailMessage mailMessage = new MailMessage()
            {
                From = emailFrom,
                IsBodyHtml = true,
                Subject = "SIGD first password access",
                Body = mailBody
            };

            mailMessage.To.Add(emailTo);

            try
            {
                GetSTMPData().Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Convert byte array to string
        /// </summary>
        /// <param name="value">byte array data</param>
        /// <returns>String</returns>
        private static string CovertByteToString(byte[] value)
        {
            return Encoding.ASCII.GetString(value);
        }

        /// <summary>
        /// Decripty an array of bytes
        /// </summary>
        /// <param name="cert">X509Certificate2 certificate</param>
        /// <param name="data">encripted data</param>
        /// <returns>decripted data</returns>
        private byte[] DecryptDataOaepSha1(X509Certificate2 cert, byte[] data)
        {
            try
            {
                // GetRSAPrivateKey returns an object with an independent lifetime, so it should be
                // handled via a using statement.
                using (RSA rsa = cert.GetRSAPrivateKey())
                {
                    return rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA1);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        /// <summary>
        /// Load certificate data
        /// </summary>
        /// <param name="certName">certificate path</param>
        /// <param name="certPass">certificate password</param>
        /// <returns>X509Certificate2 certificate</returns>
        private X509Certificate2 LoadCert(string certName, SecureString certPass)
        {
            X509Certificate2 certificate = new X509Certificate2();
            try
            {
                string certPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SGID");
                return new X509Certificate2(File.ReadAllBytes(System.IO.Path.Combine(certPath, certName)), certPass);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }

            return certificate;
        }

        private SmtpClient GetSTMPData()
        {
            SMTPConfigData config = liteDBHelper.GetSMPTConfigData();
            SecureString secureCertPass = new SecureString();
            CovertByteToString(config.CertPassword).ToCharArray().ToList().ForEach(p => secureCertPass.AppendChar(p));
            X509Certificate2 certificate = LoadCert($"{config.CertName}.pfx", secureCertPass);

            return new SmtpClient()
            {
                Host = config.Host,
                Port = config.Port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential()
                {
                    UserName = config.Email,
                    Password = CovertByteToString(DecryptDataOaepSha1(certificate, config.Password))
                }
            };
        }
    }
}
