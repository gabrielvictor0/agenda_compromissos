using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Agenda
{
    class Connection
    {

        public static SqlConnection con = new SqlConnection(Properties.Settings.Default.stringConnection);
        SqlCommand cmd;

        public void ExecuteNonQuery(string query)
        {
            con.Open();
            cmd = new SqlCommand(query, con);
            cmd.ExecuteNonQuery();
        }

        public SqlDataReader ExecuteReader(string query)
        {
            con.Open();
            cmd = new SqlCommand(query, con);
            return cmd.ExecuteReader();
        }
    }
}
