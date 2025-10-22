using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;


namespace CNPM
{
    public class DatabaseConnection
    {
        private static string connectionString =
        //"Data Source=.;Initial Catalog = BanVeTau; Integrated Security = True;";
        //"Data Source=DESKTOP-G8H86K3\\SQLEXPRESS;Initial Catalog = BanVeTau; Integrated Security = True;";
        "Data Source=LAPTOP-MKNGM2HG;Initial Catalog = BanVeTau; Integrated Security = True;";
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
