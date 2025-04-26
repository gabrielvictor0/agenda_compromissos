using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Agenda
{
    public partial class RegisterAppointment: Form
    {
        public RegisterAppointment()
        {
            InitializeComponent();
        }

        public static void OpenRegisterAppointmentForm()
        {
            Application.Run(new RegisterAppointment());
        }

        private void AddAppointment()
        {
            try
            {
                var result = MessageBox.Show("Deseja confirmar o cadastro do compromisso?", "Aviso!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if(result == DialogResult.Yes)
                {
                    Connection con = new Connection();

                    string query = $"INSERT INTO appointment (title, description, date, id_user) VALUES ('{titleTextBox.Text}', '{descriptionTextBox.Text}', '{dateTimePicker.Value.ToString("yyyy-MM-dd")}', {Properties.Settings.Default.idUser})";

                    con.ExecuteNonQuery(query);

                    MessageBox.Show("Compromisso cadastrado com sucesso!");
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

        private void AddApointBtn_Click(object sender, EventArgs e)
        {
            AddAppointment();

            Thread th = new Thread(Appointment.OpenAppointmentForm);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();

            this.Close();
        }

        private void voltarLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Thread th = new Thread(Appointment.OpenAppointmentForm);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
            this.Close();
        }
    }
}
