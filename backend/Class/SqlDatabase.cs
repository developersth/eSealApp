using System.Data;
using Microsoft.Data.SqlClient;

namespace backend
{
    public class SqlDatabase
    {
        static string strconn = Startup._Configuration.GetConnectionString("ConnectionSQLServer");
        #region Exec_Query
        public static DataTable Query_TBL(string sql)
        {
            DataTable dt = new DataTable("tmp");
            using (SqlConnection conn = new SqlConnection(strconn))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand();
                comm.Connection = conn;
                comm.CommandText = sql;
                SqlDataReader dr = comm.ExecuteReader();
                dt.Load(dr);
                conn.Close();
                conn.Dispose();
            }

            return dt;
        }

        public static DataTable Query_DA_TBL(string sql)
        {
            DataTable dt = new DataTable("tmp");
            using (SqlConnection conn = new SqlConnection(strconn))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand();
                comm.Connection = conn;
                comm.CommandText = sql;
                SqlDataAdapter da = new SqlDataAdapter(comm);
                da.Fill(dt);
                conn.Close();
                conn.Dispose();
            }

            return dt;
        }

        public static string Exec_NonQuery(string sql)
        {
            string res = string.Empty;
            using (SqlConnection conn = new SqlConnection(strconn))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand();
                comm.Connection = conn;
                comm.CommandText = sql;
                try
                {
                    if (comm.ExecuteNonQuery() > 0)
                    {
                        res = "OK";
                    }
                    else
                    {
                        res = "NOK|No Data Execute";
                    }
                }
                catch (Exception ex)
                {
                    res = "NOK|" + ex.Message.ToString().Trim();
                }
                conn.Close();
                conn.Dispose();
            }

            return res;
        }

        public static object Query_Object(string sql)
        {
            object obj;
            using (SqlConnection conn = new SqlConnection(strconn))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand();
                comm.Connection = conn;
                comm.CommandText = sql;
                obj = comm.ExecuteScalar();
                conn.Close();
                conn.Dispose();
            }

            return obj;
        }

        public static string Exec_Transaction_NonQuery(List<string> list)
        {
            string res = string.Empty;
            using (SqlConnection conn = new SqlConnection(strconn))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand();
                comm.Connection = conn;
                SqlTransaction trans = conn.BeginTransaction();
                comm.Transaction = trans;
                try
                {
                    foreach (string s in list)
                    {
                        comm.CommandText = s;
                        if (comm.ExecuteNonQuery() > 0)
                        {
                            res = "OK|";
                        }
                        else
                        {
                            trans.Rollback();
                            res = "NOK|No Data Execute";
                            break;
                        }
                    }
                    if (res.Equals("OK|"))
                    {
                        trans.Commit();
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    res = "NOK|" + ex.Message.ToString().Trim();
                }
                conn.Close();
                conn.Dispose();
            }

            return res;
        }

        #endregion
    }
}