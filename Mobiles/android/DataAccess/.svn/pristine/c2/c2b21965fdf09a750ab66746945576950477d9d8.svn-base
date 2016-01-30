package wqfree.com.dac;

import wqfree.com.Configs;

/**
 * 分页读取数据列表参数类
 * 必填写字段为:
 * TableName: 数据库对应的表名 
 * Fields: 要读取的字段,默认为"*"
 * OrderBy:排序: ID desc,Name asc 不用加Order by
 * PageSize: 每次读取的数目 *
 */
public class PagedRecordParameter {   
	
     public String ConnectionString=Configs.DefaultConnectionName; 
     public String TableName="";
 
     public String DSTableName ="Table";
 
     public String Fields ="*";
 
     public String OrderBy=""; 
     public String Where ="";
 
     public long PageIndex =1; 
     public long PageSize =10; 
     public long RecordCount =0;
 
     public String RoutingKey="";

     public   String toString()
     {
         return "ConnectionString:" + ConnectionString + ";TableName:" + TableName + ";CmdText:PagedRecordSP;CmdType:" +4 + ";RoutingKey:" + RoutingKey;
     }
}
