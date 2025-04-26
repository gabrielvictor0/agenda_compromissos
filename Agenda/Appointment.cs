using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Agenda
{
    public partial class Appointment: Form
    {
        public Appointment()
        {
            InitializeComponent();
        }

        public static void OpenAppointmentForm()
        {
            Application.Run(new Appointment());
        }

        private void GetAppointmentByUserId()
        {
            try
            {
                Connection con = new Connection();

                string query = $"SELECT id, title as 'Título', description as 'Descrição', [date] as 'Data', [status] as 'Status' FROM appointment WHERE id_user = {Properties.Settings.Default.idUser}";

                Connection.con.Open();
                SqlCommand cmd = new SqlCommand(query, Connection.con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView.DataSource = dt;
                dataGridView.Columns["id"].Visible = false;

                for(int i = 0; i < dataGridView.Rows.Count - 1; i++)
                {
                    string status = dataGridView.Rows[i].Cells[4].Value.ToString();

                    switch (status)
                    {
                        case "Concluído":
                            dataGridView.Rows[i].Cells[4].Style.BackColor = Color.LightGreen;
                            break;
                        case "Cancelado":
                            dataGridView.Rows[i].Cells[4].Style.BackColor = Color.Orange;
                            break;
                        case "Pendente":
                            dataGridView.Rows[i].Cells[4].Style.BackColor = Color.Yellow;
                            break;
                        case "Em andamento":
                            dataGridView.Rows[i].Cells[4].Style.BackColor = Color.LightSkyBlue;
                            break;
                        default:
                            break;
                    }
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

        private void Appointment_Load(object sender, EventArgs e)
        {
            GetAppointmentByUserId();
           
        }

        private void AddAppointBtn_Click(object sender, EventArgs e)
        {
            Thread th = new Thread(RegisterAppointment.OpenRegisterAppointmentForm);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();

            this.Close();
        }

        private void DataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectedAppointment selectedAppointment = new SelectedAppointment((int) dataGridView.CurrentRow.Cells[0].Value, dataGridView.CurrentRow.Cells[1].Value.ToString(), dataGridView.CurrentRow.Cells[2].Value.ToString(), dataGridView.CurrentRow.Cells[3].Value.ToString(), dataGridView.CurrentRow.Cells[4].Value.ToString());
            Thread th = new Thread(selectedAppointment.OpenSelectedAppointmentForm);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();

            this.Close();

        }

        private void Logout()
        {
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();

            Thread th = new Thread(Login.OpenLoginForm);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();

            this.Close();
        }
        
        private void LogoutButton_Click(object sender, EventArgs e)
        {
            Logout();
        }

    }
}
