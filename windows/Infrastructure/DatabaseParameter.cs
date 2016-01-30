using System;
using System.Data;
using System.Reflection;
using System.Data.Common;
using System.ServiceModel;
using System.Configuration;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Dova
{
    [Serializable]
    [DataContract]
    public class DatabaseParameter
    {
        public DatabaseParameter()
        {
        }

        public DatabaseParameter(string name, object value, DbType type)
        {
            ParameterName = name;
            Value = value;
            this.DbType = type;
        }

        public DatabaseParameter(string name, object value, DbType type, ParameterDirection direction)
        {
            ParameterName = name;
            Value = value;
            this.DbType = type;
            this.Direction = direction;
        }

        public DatabaseParameter(string name, DbType type, ParameterDirection direction)
        {
            ParameterName = name;
            this.DbType = type;
            this.Direction = direction;
        }


        string _providerName = "System.Data.SqlClient";
        [DataMember(EmitDefaultValue = false)]
        public string ProviderName
        {
            get
            {
                if (string.IsNullOrEmpty(_providerName)) return "System.Data.SqlClient";
                return _providerName;
            }
            set { if (null != value) _providerName = value; }
        }

        DbType dbType = DbType.String;

        [DataMember(EmitDefaultValue = false)]
        [DefaultValue(DbType.String)]
        public DbType DbType
        {
            get
            {
                if (dbType == 0) return DbType.String;
                return dbType;
            }
            set {dbType = value; }
        }
        //
        // 摘要:
        //     获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。
        //
        // 返回结果:
        //     System.Data.ParameterDirection 值之一。 默认值为 Input。
        //
        // 异常:
        //   System.ArgumentException:
        //     该属性未设置为有效的 System.Data.ParameterDirection 值之一。
        ParameterDirection direction = ParameterDirection.Input;
        [DefaultValue(ParameterDirection.Input)]
        [DataMember(EmitDefaultValue = false)]
        public ParameterDirection Direction
        {
            get
            {
                if (direction == 0) return ParameterDirection.Input;
                return direction;
            }
            set {  direction = value; }
        }
        //
        // 摘要:
        //     获取或设置一个值，该值指示参数是否接受空值。
        //
        // 返回结果:
        //     如果接受 null 值，则为 true；否则为 false。 默认值为 false。 
        [DataMember(EmitDefaultValue = false)]
        public bool IsNullable { get; set; }
        //
        // 摘要:
        //     获取或设置 System.Data.Common.DbParameter 的名称。
        //
        // 返回结果:
        //     System.Data.Common.DbParameter 的名称。 默认值为空字符串 ("")。
        [DefaultValue("")]
        [DataMember(EmitDefaultValue = false)]
        public string ParameterName { get; set; }
        //
        // 摘要:
        //     获取或设置列中数据的最大大小（以字节为单位）。
        //
        // 返回结果:
        //     列中数据的最大大小（以字节为单位）。 默认值是从参数值推导出的。  
        public int Size { get; set; }
        //
        // 摘要:
        //     获取或设置源列的名称，该源列映射到 System.Data.DataSet 并用于加载或返回 System.Data.Common.DbParameter.Value。
        //
        // 返回结果:
        //     映射到 System.Data.DataSet 的源列的名称。 默认值为空字符串。
        [DefaultValue("")]
        [DataMember(EmitDefaultValue = false)]
        public string SourceColumn { get; set; }
        //
        // 摘要:
        //     设置或获取一个值，该值指示源列是否可以为 null。 这使得 System.Data.Common.DbCommandBuilder 能够正确地为可以为
        //     null 的列生成 Update 语句。
        //
        // 返回结果:
        //     如果源列可以为 null，则为 true；如果不可以为 null，则为 false 
        [DataMember(EmitDefaultValue = false)]
        public bool SourceColumnNullMapping { get; set; }
        //
        // 摘要:
        //     获取或设置在加载 System.Data.Common.DbParameter.Value 时使用的 System.Data.DataRowVersion。
        //
        // 返回结果:
        //     System.Data.DataRowVersion 值之一。 默认值为 Current。
        //
        // 异常:
        //   System.ArgumentException:
        //     该属性未设置为 System.Data.DataRowVersion 值之一。 
        DataRowVersion version = DataRowVersion.Current;
        [DataMember(EmitDefaultValue = false)]
        [DefaultValue(DataRowVersion.Current)]
        public DataRowVersion SourceVersion
        {
            get
            {
                if (version == 0) return DataRowVersion.Current;
                return version;
            }
            set { version = value; }
        }
        //
        // 摘要:
        //     获取或设置该参数的值。
        //
        // 返回结果:
        //     一个 System.Object，它是该参数的值。 默认值为 null。
        [DefaultValue("")]
        [DataMember(EmitDefaultValue = false)]
        public object Value { get; set; }

        // 摘要:
        //     将 DbType 属性重置为其原始设置。 
        public void ResetDbType()
        {
            DbType = DbType.String;
        }


        [DataMember(EmitDefaultValue = false)]
        public byte Precision { get; set; }
        //
        // 摘要:
        //     指示数值参数的小数位数。
        //
        // 返回结果:
        //     要将 System.Data.OleDb.OleDbParameter.Value 解析为的小数位数。 默认值为 0。
        [DataMember(EmitDefaultValue = false)]
        public byte Scale { get; set; }

        public string ToString()
        {
            if (null == Value) return "";
            return Value.ToString();
        }

        public string ToKeyString()
        {
            return "ParameterName:" + ParameterName + ";value:"+(Value==null?"null":Value.ToString())+";DbType:" + DbType + ";Size:" + Size +";Direction:"+Direction;
        }

    }
}
