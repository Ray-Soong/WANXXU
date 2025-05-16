using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Datas
{
    class db
    {
        public static System.Data.DataTable proc(string sql)
        {
            string connectionString = "User Id=wcs_db;Password=wcs_db;Data Source=172.30.121.57:1521/orcl;";

            // 创建Oracle连接
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                // 打开连接
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        System.Data.DataTable tab = new System.Data.DataTable();

                        int c = reader.FieldCount;
                        for (int i = 0; i < c; i++)
                        {
                            tab.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                        }
                        // 遍历结果集
                        while (reader.Read())
                        {

                            var r = tab.NewRow();
                            for (int i = 0; i < c; i++)
                            {
                                r[i] = reader.GetValue(i);
                            }
                            tab.Rows.Add(r);
                            // 输出每一行的数据
                            //  Console.WriteLine(reader.GetString(0)); // 假设第一列是字符串类型
                        }
                        connection.Close();
                        return tab;

                    }
                }
            }
        }
    }
}
