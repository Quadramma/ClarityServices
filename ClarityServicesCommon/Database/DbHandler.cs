using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClarityServices.System;

namespace ClarityServices.Database {
    public class DbHandler {
        private string connectionString;
        private Logger logger = new Logger(ConfigurationManager.AppSettings.Get("DB_LOG_PATH"));
        public DbHandler(string connectionString) {
            this.connectionString = connectionString;
        }

        public DataSet Call(string spName, SqlConnection conn, Action<SqlDataAdapter> body) {
            SqlDataAdapter da = new SqlDataAdapter(spName, conn);
            //da.SelectCommand.Parameters.AddWithValue("@null", DBNull.Value);
            body(da);
            DataSet ds = new DataSet();
            da.Fill(ds, getDbName());
            return ds;
        }

        public void Using(string user, string password, Action<SqlConnection> body, Action<string> onServerException) {
            string conn = getConnectionString(user, password);
            using (SqlConnection cn = new SqlConnection(conn)) {
                try {
                    cn.Open();
                    body(cn);
                    cn.Close();
                } catch (Exception e) {
                   logger.write(e.ToString());
                   onServerException(e.ToString());
                }
            }
        }

        public string getConnectionString(string sUser, string sPassword) {
            return ""
                       + "Data Source = " + getServerName() + ";"
                       + "Initial Catalog = " + getDbName() + ";"
                       + "User Id = " + sUser + ";"
                       + "Password = " + sPassword + ";";
        }
        public string getServerName() {
            return new SqlConnectionStringBuilder(connectionString).DataSource.ToString();
        }
        public string getDbName() {
            return new SqlConnectionStringBuilder(connectionString).InitialCatalog.ToString();
        }
    }
}
