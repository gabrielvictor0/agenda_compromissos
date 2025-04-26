using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Threading;

namespace Agenda
{
    public partial class Register: Form
    {

        private string encryptionKey;
        private Connection con;
        private SqlDataReader dr;
        private string query; 

        public Register()
        {
            InitializeComponent();
        }

        private void Register_Load(object sender, EventArgs e)
        {
            encryptionKey = "SecretEncryptKey";
        }
        public static void OpenRegisterForm()
        {
            Application.Run(new Register());
        }

        private bool VerifyEmail()
        {

            try
            {
                con = new Connection();
                query = $"SELECT * FROM [user] WHERE email = '{emailTextBox.Text}'";
                dr = con.ExecuteReader(query);

                if (dr.Read())
                {
                    MessageBox.Show("Este email já está vinculado a uma conta!");
                    return true;
                }

                return false;
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

        private void RegisterUser()
        {
            try
            {
                if(!emailTextBox.Text.Contains("@"))
                {
                    throw new InvalidDataException("E-mail inválido!");
                }

                if(passwordTextBox.Text.Length <= 5 || passwordTextBox.Text.Length > 15)
                {
                    throw new InvalidDataException("A senha deve conter no mínimo 6 caracteres.");
                }

                if(passwordTextBox.Text != confirmPwdTextBox.Text)
                {
                    throw new InvalidDataException("Senhas não correspondem!");
                }

                con = new Connection();
                query = $" INSERT INTO [user] (email, password) VALUES ('{emailTextBox.Text}', '{Encrypt(passwordTextBox.Text, encryptionKey)}')";
                con.ExecuteNonQuery(query);

                MessageBox.Show("Usuário cadastrado com sucesso!");

                Thread th = new Thread(Login.OpenLoginForm);
                th.SetApartmentState(ApartmentState.STA);
                th.Start();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Aviso!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                Connection.con.Close();
            }
            
        }

        private static string Encrypt(string text, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[16];

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            bool registeredEmail = VerifyEmail();

            if (!registeredEmail)
            {
                RegisterUser();
            }
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Thread th = new Thread(Login.OpenLoginForm);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
            this.Close();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
