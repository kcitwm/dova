package wqfree.com.dac;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import com.fasterxml.jackson.annotation.JsonInclude;

import wqfree.com.Configs;

//@JsonInclude(JsonInclude.Include.NON_DEFAULT) 
public class WrapedDatabaseParameter { 
      public String ConnectionString=Configs.DefaultConnectionName; 
      public String TableName ="DataTable"; 
      public String CmdText=""; 
      public int CmdType =0; 
      public List<DatabaseParameter> DatabaseParameters=new ArrayList<DatabaseParameter>();      
      public String RoutingKey  ="";
      
      public static  String UserName="";
      
      public WrapedDatabaseParameter(){
      
      }
      
      public WrapedDatabaseParameter(String cmdText,DatabaseParameter... parameters){
    	   this.CmdText=cmdText; 
    	  this.DatabaseParameters=Arrays.asList(parameters);
      }
      
      public WrapedDatabaseParameter(String cmdText,List<DatabaseParameter>  parameters){
    	   this.CmdText=cmdText; 
    	  this.DatabaseParameters=parameters;
      }
      
      
      
      public WrapedDatabaseParameter(String connectionString,String cmdText,int cmdType,DatabaseParameter... parameters){
    	  this.ConnectionString=connectionString;
    	  this.CmdText=cmdText;
    	  this.CmdType=cmdType;
    	  this.DatabaseParameters=Arrays.asList(parameters);
      }
      
      public WrapedDatabaseParameter(String connectionString,String cmdText,int cmdType,List<DatabaseParameter>  parameters){
    	  this.ConnectionString=connectionString;
    	  this.CmdText=cmdText;
    	  this.CmdType=cmdType;
    	  this.DatabaseParameters=parameters;
      }
      
      public   String toString()
      { 
          return "ConnectionString:" + ConnectionString + ";TableName:" +TableName + ";CmdText:" + CmdText + ";CmdType:" + CmdType +";RoutingKey:"+RoutingKey+";DatabaseParameters.length:" +DatabaseParameters.size();
      }

  } 