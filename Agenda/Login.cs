using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Agenda
{
    public partial class Login: Form
    {

        public Login()
        {
            InitializeComponent();
        }

        public static void OpenLoginForm()
        {
            Application.Run(new Login());
        }

        private string Decrypt(string encryptedText, string key)
        {
            using(Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[16];

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using(MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using(StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        public Dictionary<string, string> FindUserDataByEmail()
        {
            try
            {
                Connection con = new Connection();

                string query = $"SELECT id, [password] FROM [user] WHERE email = '{emailTextBox.Text}'";

                SqlDataReader dr = con.ExecuteReader(query);

                if (!dr.Read())
                {
                    MessageBox.Show("E-mail ou senha inválidos!", "Aviso!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }
                else
                {
                    Dictionary<string, string> userData = new Dictionary<string, string>();

                    userData.Add("id", dr[0].ToString());
                    userData.Add("encryptPassword", dr[1].ToString());

                    return userData;
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                Connection.con.Close();
            }
           

        }

        private bool VerifyPassword(Dictionary<string, string> userData)
        {
            try
            {
                string decryptPassword = Decrypt(userData["encryptPassword"], "SecretEncryptKey");

                if (decryptPassword == passwordTextBox.Text)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("E-mail ou senha inválidos!", "Aviso!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
           
        }  

        private void SaveUserData(Dictionary<string, string> userData)
        {
            Properties.Settings.Default.idUser = int.Parse(userData["id"]);
            Properties.Settings.Default.Save();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {

            Dictionary<string, string> userData = FindUserDataByEmail();

            if(userData != null)
            {
                bool validUser = VerifyPassword(userData);

                if (validUser)
                {
                    emailTextBox.Text = "";
                    passwordTextBox.Text = "";

                    SaveUserData(userData);

                    Thread th = new Thread(Appointment.OpenAppointmentForm);
                    th.SetApartmentState(ApartmentState.STA);
                    th.Start();
                    this.Close();
                }
            }
        }
    }
}
