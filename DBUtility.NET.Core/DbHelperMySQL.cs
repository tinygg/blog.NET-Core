using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace DBUtility
{

    public class DbHelperMySQL
    {
        private static readonly string connString = PubConstant.ConnectionString;

        public DbHelperMySQL() { }

        public static MySqlConnection Connection()
        {
            var conn = new MySqlConnection(connString);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString)
        {
            using (var connection = Connection())
            {
                object obj = ExecuteScalar(SQLString);
                connection.Close();
                if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                {
                    return null;
                }
                else
                {
                    return obj;
                }
            }
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static bool Exists(string strSql)
        {
            object obj = GetSingle(strSql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 是否存在（基于MySqlParameter）
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static bool Exists(string strSql, params object[] cmdParms)
        {
            object obj = GetSingle(strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static object GetSingle(string SQLString, params object[] param)
        {

            using (var connection = Connection())
            {
                try
                {
                    object obj = ExecuteScalar(SQLString, param);

                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (SqlException e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public static int ExecuteSql(string SQLString)
        {
            using (var connection = Connection())
            {
                try
                {
                    int rows = ExecuteNonQuery(SQLString);
                    return rows;
                }
                catch (Exception e)
                {
                    connection.Close();
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public static int ExecuteSql(string SQLString, params object[] cmdParms)
        {
            int rows = ExecuteNonQuery(SQLString, cmdParms);
            return rows;
        }

        public static int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }


        /// <summary>
        /// 拼接SQL语句，使用参数化查询。
        /// </summary>
        /// <param name="fields">Field1=@field1;Field2=@field2;</param>
        /// <param name="parameters"></param>
        /// <param name="sql"></param>
        /// <param name="mysqlprmts"></param>
        public static void SQLAppend(string fields, Dictionary<string, string> parameters, ref StringBuilder sql, ref List<MySqlParameter> mysqlprmts)
        {
            Regex reg_f = null;
            foreach (var p in parameters)
            {
                reg_f = new Regex(@"(([\w\s]*)[=]?\(?@" + p.Key + @"\)?)#?(\w*)?");
                var match = reg_f.Match(fields);
                //匹配是否有这个字段
                if (match.Success && p.Value.Trim() != "")
                {
                    MySqlParameter parameter = new MySqlParameter();
                    parameter.ParameterName = p.Key;
                    parameter.Value = p.Value;
                    string field = match.Groups[2].Value;
                    string type = match.Groups[3].Value;
                    if (type != "")
                    {
                        switch (type.ToUpper())
                        {
                            case "VARCHAR":
                                parameter.MySqlDbType = MySqlDbType.VarChar;
                                break;
                            case "INT":
                                parameter.MySqlDbType = MySqlDbType.Int32;
                                break;
                        }
                    }
                    sql.Append(" and ");
                    sql.Append(match.Groups[1].Value);
                    mysqlprmts.Add(parameter);
                }
            }
        }

        public static IDataReader ExecuteReader(string strSQL)
        {
            return ExecuteReader(strSQL, null);
        }

        /// <summary>
        /// 外部记得关闭连接
        /// connection.Close();
        /// </summary>
        /// <param name="strSQL"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static IDataReader ExecuteReader(string strSQL, params object[] cmdParms)
        {
            using (var connection = Connection())
            {
                try
                {
                    IDataReader myReader = connection.ExecuteReader(strSQL, cmdParms, null, null, CommandType.Text);
                    return myReader;
                }
                catch (SqlException e)
                {
                    throw e;
                }
            }
        }

        public static DataSet Query(string SQLString, params object[] cmdParms)
        {

            using (var connection = Connection())
            {
                try
                {
                    IDataReader dataReader = null;
                    if (cmdParms == null || cmdParms.Length == 0)
                    {
                        dataReader = connection.ExecuteReader(SQLString, null, null, null, CommandType.Text);
                    }
                    else
                    {
                        dataReader = connection.ExecuteReader(SQLString, cmdParms, null, null, CommandType.Text);
                    }

                    string colName = string.Empty;
                    DataSet ds = new DataSet("ds");
                    DataTable dt = ds.Tables.Add("dt");
                    bool is_first = true;
                    while (dataReader.Read())
                    {
                        //dic = new Dictionary<string, object>();
                        DataRow row = dt.NewRow();

                        for (int i = 0; i < dataReader.FieldCount; i++)
                        {
                            colName = dataReader.GetName(i);
                            if (is_first)
                            {
                                dt.Columns.Add(colName);
                            }
                            row[colName] = dataReader[colName];
                        }
                        dt.Rows.Add(row);
                        is_first = false;
                    }
                    //ds.Tables.Add(dt);
                    return ds;
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
        }




        // +ExcuteNonQuery 增、删、改同步操作

        /// <summary>
        /// TODO 好好思考
        /// </summary>
        public static void MultiQuery()
        {
            using (var connection = Connection())
            {
                string sqlStr = @"select Id,Title,Author from Article where Id = @id 
                                  select * from QQModel where Name = @name 
                                  select * from SeoTKD where Status = @status";
                connection.Open();
                using (var multi = connection.QueryMultiple(sqlStr, new { id = 11, name = "打代码", status = 99 }))
                {
                    //multi.IsConsumed   reader的状态 ，true 是已经释放
                    if (!multi.IsConsumed)
                    {
                        //注意一个东西，Read获取的时候必须是按照上面返回表的顺序 （article，qqmodel，seotkd）
                        //强类型
                        var articleList = multi.Read();//类不见得一定得和表名相同
                        var QQModelList = multi.Read();
                        var SeoTKDList = multi.Read();
                    }

                }
                connection.Close();
            }
        }



        /// <summary>
        /// 增、删、改同步操作
        ///  </summary>
        /// <param name="cmdStr">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="flag">true存储过程，false sql语句</param>
        /// <returns>int</returns>
        public static int ExecuteNonQuery(string cmdStr, object param = null, bool flag = false)
        {
            int result = 0;

            using (var connection = Connection())
            {
                try
                {
                    MySqlCommand cmd = connection.CreateCommand();
                    cmd.CommandText = cmdStr;
                    if (param != null)
                    {
                        if (param.GetType() == typeof(List<MySqlParameter>))
                        {
                            cmd.Parameters.AddRange(((List<MySqlParameter>)param).ToArray());
                        }
                        else if (param.GetType() == typeof(MySqlParameter[]))
                        {
                            cmd.Parameters.AddRange((MySqlParameter[])param);
                        }
                    }

                    if (flag)
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        //result = con.Execute(cmd, param, null, null, CommandType.StoredProcedure);
                    }
                    else
                    {
                        cmd.CommandType = CommandType.Text;
                        //result = con.Execute(cmd, param, null, null, CommandType.Text);
                    }
                    result = cmd.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }


        //+QueryData  同步查询数据集合
        /// <summary>
        /// 同步查询数据集合
        /// </summary>
        /// <param name="cmd">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="flag">true存储过程，false sql语句</param>
        /// <returns>t</returns>
        public static List<Dictionary<String, object>> QueryData(string cmd, object param = null, bool flag = false)
        {
            IDataReader dataReader = null;
            using (var connection = Connection())
            {
                try
                {
                    if (flag)
                    {
                        dataReader = connection.ExecuteReader(cmd, param, null, null, CommandType.StoredProcedure);
                    }
                    else
                    {
                        dataReader = connection.ExecuteReader(cmd, param, null, null, CommandType.Text);
                    }
                    List<Dictionary<String, object>> list = new List<Dictionary<string, object>>();
                    Dictionary<String, object> dic = null;
                    string colName = "";
                    while (dataReader.Read())
                    {
                        dic = new Dictionary<string, object>();

                        for (int i = 0; i < dataReader.FieldCount; i++)
                        {
                            colName = dataReader.GetName(i);
                            dic.Add(colName, dataReader[colName]);
                        }


                        if (dic.Keys.Count > 0)
                        {
                            list.Add(dic);
                        }
                    }
                    return list;
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }

            }
        }

        // +ExecuteScalar 同步查询操作
        /// <summary>
        /// 同步查询操作
        /// </summary>
        /// <param name="cmd">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="flag">true存储过程，false sql语句</param>
        /// <returns>object</returns>
        public static object ExecuteScalar(string cmd, object param = null, bool flag = false)
        {
            object result = null;
            using (var connection = Connection())
            {
                try
                {
                    if (flag)
                    {
                        result = connection.ExecuteScalar(cmd, param, null, null, CommandType.StoredProcedure);
                    }
                    else
                    {
                        result = connection.ExecuteScalar(cmd, param, null, null, CommandType.Text);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }


        // +QueryPage 同步分页查询操作
        /// <summary>
        /// 同步分页查询操作
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="orderBy">排序字段</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页面容量</param>
        /// <param name="count">总条数</param>
        /// <param name="param">参数</param>
        /// <param name="strWhere">条件</param>
        /// <returns>返回结果的数据集合</returns>
        public static List<Dictionary<string, Object>> QueryPage(string sql, string orderBy, int pageIndex, int pageSize, out int count, object param = null, string strWhere = "")
        {
            count = 0;
            List<Dictionary<String, Object>> list = new List<Dictionary<string, object>>();

            if (sql.Contains("where"))
            {
                sql = sql + strWhere;
            }
            else
            {
                sql = sql + " where 1=1 " + strWhere;
            }

            string strSQL = "SELECT (@i:=@i+1) AS row_id,tab.* FROM (" + sql + ")  AS TAB,(SELECT @i:=0) AS it ORDER BY " + orderBy + " LIMIT " + (pageIndex - 1) + "," + pageSize;


            list = QueryData(strSQL, param, false);


            string strCount = "SELECT count(*) FROM (" + sql + ") tcount";
            count = Convert.ToInt32(ExecuteScalar(strCount));

            return list;
        }



        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            try
            {
                IDataReader dataReader = null;
                using (var connection = Connection())
                {
                    try
                    {
                        dataReader = connection.ExecuteReader(storedProcName, parameters, null, null, CommandType.StoredProcedure);
                        string colName = string.Empty;
                        DataSet ds = new DataSet("ds");
                        DataTable dt = ds.Tables.Add(tableName);
                        bool is_first = true;
                        while (dataReader.Read())
                        {
                            //dic = new Dictionary<string, object>();
                            DataRow row = dt.NewRow();

                            for (int i = 0; i < dataReader.FieldCount; i++)
                            {
                                if (is_first)
                                {
                                    colName = dataReader.GetName(i);
                                    //dic.Add(colName, dataReader[colName]);
                                    dt.Columns.Add(colName);
                                    is_first = false;
                                }
                                else
                                {
                                    row[colName] = dataReader[colName];
                                }
                            }
                        }
                        return ds;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        connection.Close();
                    }

                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
