using System;
using System.Configuration;
using Commons;

namespace DBUtility
{
    
    public class PubConstant
    {        
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public static string ConnectionString
        {           
            get 
            {
                string _connectionString = ConfigHelper.GetConfigFromJson("ConnectionString");       
                string ConStringEncrypt = ConfigHelper.GetConfigFromJson("ConStringEncrypt");
                if (ConStringEncrypt == "true")
                {
                    _connectionString = DESEncrypt.Decrypt(_connectionString);
                }
                return _connectionString; 
            }
        }

        public static string OraConnectionString
        {
            get
            {
                string _connectionString = ConfigHelper.GetConfigFromJson("OraConnectionString");
                string ConStringEncrypt = ConfigHelper.GetConfigFromJson("ConStringEncrypt");
                if (ConStringEncrypt == "true")
                {
                    _connectionString = DESEncrypt.Decrypt(_connectionString);
                }
                return _connectionString; 
            }
        }


        /// <summary>
        /// 得到web.config里配置项的数据库连接字符串。
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static string GetConnectionString(string configName)
        {
            string connectionString = ConfigHelper.GetConfigFromJson(configName);
            string ConStringEncrypt = ConfigHelper.GetConfigFromJson("ConStringEncrypt");
            if (ConStringEncrypt == "true")
            {
                connectionString = DESEncrypt.Decrypt(connectionString);
            }
            return connectionString;
        }


    }
}
