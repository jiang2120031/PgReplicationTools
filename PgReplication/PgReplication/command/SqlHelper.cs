using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Configuration;
using System.Data;

namespace PgReplication.command
{
    public class SqlHelper
    {
        private static readonly string connection_from = ConfigurationManager.ConnectionStrings["connectionfrom"].ConnectionString;
        private static readonly string connection_to = ConfigurationManager.ConnectionStrings["connectionto"].ConnectionString;
        public static int ExecuteNonquery(bool isfrom, string sql, params NpgsqlParameter[] sqlparam)
        {
            var sqlConnection = isfrom ? connection_from : connection_to;
            using (var con = new NpgsqlConnection(sqlConnection))
            {
                using (var cmd = new NpgsqlCommand(sql, con))
                {
                    con.Open();
                    cmd.Parameters.AddRange(sqlparam);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static int DoTransaction(bool isfrom, string sql, params NpgsqlParameter[] sqlparam)
        {
            var sqlConnection = isfrom ? connection_from : connection_to;
            var n = 0;
            using (var con = new NpgsqlConnection(sqlConnection))
            {
                con.Open();
                var transaction = con.BeginTransaction();
                using (var cmd = new NpgsqlCommand(sql, con, transaction))
                {
                    try
                    {
                        cmd.Parameters.AddRange(sqlparam);
                        n = cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        n = -1;
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return n;
        }

        public static DataTable GetDataTable(bool isfrom, string sql, params NpgsqlParameter[] sqlparam)
        {
            var sqlConnection = isfrom ? connection_from : connection_to;
            using (var con = new NpgsqlConnection(sqlConnection))
            {
                using (var sda = new NpgsqlDataAdapter(sql, con))
                {
                    con.Open();
                    sda.SelectCommand.Parameters.AddRange(sqlparam);
                    var dataSet = new DataSet();
                    sda.Fill(dataSet);
                    if (dataSet.Tables.Count > 0)
                    {
                        return dataSet.Tables[0];
                    }
                    return null;
                }
            }
        }

        public static object ExecuteScalar(bool isfrom, string sql, params NpgsqlParameter[] sqlparam)
        {
            var sqlConnection = isfrom ? connection_from : connection_to;
            using (var con = new NpgsqlConnection(sqlConnection))
            {
                using (var cmd = new NpgsqlCommand(sql, con))
                {
                    con.Open();
                    cmd.Parameters.AddRange(sqlparam);
                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}
