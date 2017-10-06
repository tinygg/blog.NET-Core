using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace KXNF.DBUtility2
{
    public enum MySqlDbType
    {
        VarChar,
        Int16,
        Int32,
        Int64,
        Date,
        Text,
        DateTime,
        Decimal,
        Double,
        Bit,
        LongBlob,
        MediumText,
        String
    }

    public class MySqlParameter : IDataParameter
    {
        public string ParameterName;
        public object Value;
        public MySqlDbType MySqlDbType;

        public MySqlParameter()
        {

        }

        public MySqlParameter(string ParameterName, MySqlDbType MySqlDbType)
        {
            this.ParameterName = ParameterName;
            this.MySqlDbType = MySqlDbType;
        }

        public MySqlParameter(string ParameterName, MySqlDbType MySqlDbType, int size)
        {
            this.ParameterName = ParameterName;
            this.MySqlDbType = MySqlDbType;
        }

        public DbType DbType
        {
            get; set;
        }

        public ParameterDirection Direction
        {
            get; set;
        }

        public bool IsNullable
        {
            get; set;
        }

        public string SourceColumn
        {
            get; set;
        }

        public DataRowVersion SourceVersion
        {
            get; set;
        }

        string IDataParameter.ParameterName
        {
            get; set;
        }

        object IDataParameter.Value
        {
            get; set;
        }
    }
}
