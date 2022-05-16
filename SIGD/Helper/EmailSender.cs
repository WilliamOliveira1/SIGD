﻿using SMTPConfig.Models;
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
    public class EmailSender
    {
        public void SendEmail(string emailAdressTo, string mailBody)
        {
            
            SIGDLiteDBHelper liteDBHelper = new SIGDLiteDBHelper();
            SMTPConfigData config = liteDBHelper.GetSMPTConfigData();
            SecureString secureCertPass = new SecureString();
            CovertByteToString(config.CertPassword).ToCharArray().ToList().ForEach(p => secureCertPass.AppendChar(p));
            X509Certificate2 certificate = LoadCert($"{config.CertName}.pfx", secureCertPass);

            SmtpClient smtpClient = new SmtpClient()
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

            string emailAdressFrom = config.Email;
            string displayNameFrom = "Projeto IntegradorII";

            MailAddress emailFrom = new MailAddress(emailAdressFrom, displayNameFrom);
            MailAddress emailTo = new MailAddress(emailAdressTo);

            TokenGenerator tokenGenerator = new TokenGenerator();
            mailBody = "<h1>" + mailBody + " " +  tokenGenerator.GetToken() + "</h1>";

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
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
            }
        }

        static string CovertByteToString(byte[] value)
        {
            return Encoding.ASCII.GetString(value);
        }

        public byte[] DecryptDataOaepSha1(X509Certificate2 cert, byte[] data)
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
    }
}
