using SMTPConfig.Helpers;
using SMTPConfig.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SMTPConfig
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LiteDBHelper liteDBHelper = new LiteDBHelper();
            string host = Host.Text.ToString();
            string email = Email.Text.ToString();
            string port = Port.Text;
            string certName = "SIGD";
            SecureString securePassword = new SecureString();
            byte[] data = Encoding.ASCII.GetBytes(Password.Password);
            byte[] certPassData = Encoding.ASCII.GetBytes(CertPassword.Password);
            CreateCert createCert = new CreateCert();
            CertPassword.Password.ToString().ToCharArray().ToList().ForEach(p => securePassword.AppendChar(p));
            bool isCertCreated = createCert.CreateCertificate(certName, securePassword);
            X509Certificate2 cert = LoadCert(certName, securePassword);
            byte[] encryptPass = EncryptDataOaepSha1(cert, data);            
            

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(port) || securePassword.Length == 0)
            {
                MessageBox.Show($"Todos os campos são necessários", "Erro");
            }
            else
            {
                if (!Int32.TryParse(port, out int portNumber))
                {
                    MessageBox.Show($"A porta deve ser um número", "Erro");
                }

                SMTPConfigData config = new SMTPConfigData()
                {
                    Email = email,
                    Host = host,
                    Port = portNumber,
                    Password = encryptPass,
                    CertName = certName,
                    CertPassword = certPassData
                };
                try
                {
                    SMTPConfigData configFromDB = liteDBHelper.GetSMPTConfigData();
                    bool isDataSaved = false;

                    if (configFromDB?.Email == null || configFromDB?.Host == null)
                    {
                        isDataSaved = liteDBHelper.Save(config);
                    }
                    else if (configFromDB != null)
                    {
                        isDataSaved = liteDBHelper.UpdateSMPTConfigData(config);
                    }
                    

                    if (!isDataSaved)
                    {
                        MessageBox.Show($"Ocorreu um erro ao salvar os dados! Por favor tente novamente.", "Erro");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocorreu um erro ao salvar os dados: {ex.Message}", "Erro");
                }                
            }
        }

        /// <summary>
        /// Load a certificate
        /// </summary>
        /// <param name="certName">certificate file name</param>
        /// <param name="pass">secure string password</param>
        /// <returns>X509Certificate2 certificate</returns>
        private X509Certificate2 LoadCert(string certName, SecureString pass)
        {
            X509Certificate2 x509 = new X509Certificate2();
            try
            {
                string certPath = System.IO.Path.Combine((Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)), "SGID");
                x509 = new X509Certificate2(File.ReadAllBytes(System.IO.Path.Combine(certPath, $"{certName}.pfx")), pass);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro ao salvar os dados: {ex.Message}", "Erro");
            }
            return x509;
        }

        /// <summary>
        /// Encrypt a byte array using an certificate
        /// </summary>
        /// <param name="cert">certificate</param>
        /// <param name="data">data bayte array</param>
        /// <returns>Encrypted byte array</returns>
        public byte[] EncryptDataOaepSha1(X509Certificate2 cert, byte[] data)
        {            
            try
            {
                // GetRSAPublicKey returns an object with an independent lifetime, so it should be
                // handled via a using statement.
                using (RSA rsa = cert.GetRSAPublicKey())
                {
                    // OAEP allows for multiple hashing algorithms, what was formermly just "OAEP" is
                    // now OAEP-SHA1.
                    return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro ao salvar os dados: {ex.Message}", "Erro");
                throw ex;
            }            
        }       
    }
}
