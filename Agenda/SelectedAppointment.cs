using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Agenda
{
    public partial class SelectedAppointment: Form
    {
        Connection con;
        int id;
        string title;
        string description;
        string date;
        string status;

        public SelectedAppointment(int id, string title, string description, string date, string status)
        {
            InitializeComponent();

            this.id = id;
            this.title = title;
            this.description = description;
            this.date = date;
            this.status = status;
        }

        public void OpenSelectedAppointmentForm()
        {
            SelectedAppointment selectedAppointment = new SelectedAppointment(id, title, description, date, status);

            selectedAppointment.titleTextBox.Text = title;
            selectedAppointment.descriptionTextBox.Text =  description;
            selectedAppointment.dateTimePicker.Value = DateTime.Parse(date);

            Application.Run(selectedAppointment);
        }

        private void DeleteAppointment()
        {
            try
            {

                var result = MessageBox.Show("Deseja confirmar a exclusão?", "Aviso!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    con = new Connection();
                    string query = $"DELETE FROM appointment WHERE id = {id}";
                    con.ExecuteNonQuery(query);
                    MessageBox.Show("Compromisso deletado com êxito!", "Sucesso!");

                    Thread th = new Thread(Appointment.OpenAppointmentForm);
                    th.SetApartmentState(ApartmentState.STA);
                    th.Start();

                    this.Close();
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

        private void deleteButton_Click(object sender, EventArgs e)
        {
            DeleteAppointment();   
        }

        private void UpdateAppointment()
        {
            try
            {

                if(titleTextBox.Text == string.Empty)
                {
                    throw new InvalidDataException("Título não pode ser vazio!");
                }

                var result = MessageBox.Show("Deseja confirmar a alteração?", "Aviso!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if(result == DialogResult.Yes)
                {
                    con = new Connection();
                    string query = $"UPDATE appointment SET title = '{titleTextBox.Text}', description = '{descriptionTextBox.Text}', date = '{dateTimePicker.Value}', [status] = '{statusComboBox.Text}' WHERE id = {id}";
                    con.ExecuteNonQuery(query);

                    MessageBox.Show("O compromisso foi editado com êxito!", "Sucesso!");

                    Thread th = new Thread(Appointment.OpenAppointmentForm);
                    th.SetApartmentState(ApartmentState.STA);
                    th.Start();

                    this.Close();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                Connection.con.Close();
            }

        }
        private void editButton_Click(object sender, EventArgs e)
        {
            UpdateAppointment();
        }

        private void voltarLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Thread th = new Thread(Appointment.OpenAppointmentForm);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
            this.Close();
        }

        private void SelectedAppointment_Load(object sender, EventArgs e)
        {
            statusComboBox.Items.Add("Pendente");
            statusComboBox.Items.Add("Concluído");
            statusComboBox.Items.Add("Cancelado");
            statusComboBox.Items.Add("Em andamento");

            for (int i = 0; i < statusComboBox.Items.Count; i++)
            {
                if (statusComboBox.Items[i].ToString() == status)
                {
                    statusComboBox.SelectedItem = statusComboBox.Items[i];
                }
            }
        }
    }
}
