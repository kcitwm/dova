package wqfree.com.dac;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.List;
import java.util.TimeZone;

import wqfree.com.Configs;
import wqfree.com.MQService;
import wqfree.com.ServiceConnectionString;
import wqfree.com.TcpHelper;
import wqfree.com.UserContext;
import wqfree.com.Utils;
import wqfree.com.WQMessage;

import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.JavaType;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.SerializationFeature;


public class DAC
{
	static String className = "DAC";
	static String RoutingGroupName = "DAC"; 
	public  String ConnString = Configs.DefaultConnectionName;  
	static  ObjectMapper mapper=new  ObjectMapper();   
	public ServiceConnectionString scs=null;
	static String encoding="utf-8";

	static int authoType=Configs.AuthoType; 
	
	public MQService mqService=null;

	public DAC(){
		scs=ServiceConnectionString.getConnection(ConnString);
		SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss"); 
		mapper.configure(SerializationFeature.WRITE_DATES_AS_TIMESTAMPS, false); 
 
		mapper.setTimeZone(TimeZone.getTimeZone("GMT+8"));  
		mapper.setDateFormat(format);  
	} 

	public DAC(String connString){
		scs=ServiceConnectionString.getConnection(connString); 
		SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");  
		mapper.configure(SerializationFeature.WRITE_DATES_AS_TIMESTAMPS, false); 
		mapper.setTimeZone(TimeZone.getTimeZone("GMT+8"));  
		mapper.setDateFormat(format);  
	}
	

	public DAC(ServiceConnectionString uscs){
			scs=uscs; 
			SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");  
			mapper.configure(SerializationFeature.WRITE_DATES_AS_TIMESTAMPS, false);  
			mapper.setTimeZone(TimeZone.getTimeZone("GMT+8"));  
			mapper.setDateFormat(format);  
	}
	
	public DAC(MQService mqs){ 
		mqService=mqs; 
		scs=ServiceConnectionString.getConnection(ConnString); 
		SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");  
		mapper.configure(SerializationFeature.WRITE_DATES_AS_TIMESTAMPS, false); 
		mapper.setTimeZone(TimeZone.getTimeZone("GMT+8"));  
		mapper.setDateFormat(format);  
	} 
	
	
	public DAC(MQService mqs,String connString){ 
		mqService=mqs; 
		scs=ServiceConnectionString.getConnection(connString); 
		SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");  
		mapper.configure(SerializationFeature.WRITE_DATES_AS_TIMESTAMPS, false); 
		mapper.setTimeZone(TimeZone.getTimeZone("GMT+8"));  
		mapper.setDateFormat(format);  
	} 
	
	
	private byte[] ExecuteMq(String methodName,String parameters) throws Exception{ 
		  WQMessage msg=new WQMessage();
		  msg.ServiceName="DACService";
		  msg.MethodName=methodName;
		  msg.Body=parameters;
		  msg.Async=true;
		  return Utils.toByte(mqService.send(msg));  
	}
	

	private byte[] Execute(String methodName,String msg) throws Exception{
		//String msg="{\"cmdText\":\"GetTestInfo\",\"CmdType\":1,\"ConnectionString\":\"DefaultConnection\",\"DatabaseParameters\":[{\"DbType\":11,\"ParameterName\":\"NO\",\"Value\":37931619}],\"TableName\":\"Test\"}";
		if(null==mqService){
		byte[] method=methodName.getBytes();
		byte[] header = Utils.toByte(method.length); 
		byte[] data=msg.getBytes(); 
		int len=method.length+header.length+data.length;  
		byte[] req=null;
		int startIdx=0;
		if(authoType==1)
		{
			byte[] user=UserContext.UserName().getBytes();
			byte[] userHeader = Utils.toByte(user.length);  
			len=len+userHeader.length+user.length;
			req=new byte[len];
			System.arraycopy(userHeader,0,req,0,userHeader.length);
			System.arraycopy(user,0,req,userHeader.length,user.length);
			startIdx=userHeader.length+user.length;
		}
		else
			req=new byte[len];
		System.arraycopy(header,0,req,startIdx,header.length);
		System.arraycopy(method,0,req,startIdx+header.length,method.length);
		System.arraycopy(data,0,req,startIdx+header.length+method.length,data.length);  
		return  TcpHelper.sendVar(scs.Address,scs.Port,req,4) ;  
		}
		return ExecuteMq(methodName,msg);
	}

	
	private byte[] ExecuteFile(String methodName,String fileName,byte[] data) throws Exception{
		//String msg="{\"cmdText\":\"GetTestInfo\",\"CmdType\":1,\"ConnectionString\":\"DefaultConnection\",\"DatabaseParameters\":[{\"DbType\":11,\"ParameterName\":\"NO\",\"Value\":37931619}],\"TableName\":\"Test\"}";
		byte[] method=methodName.getBytes();
		byte[] header = Utils.toByte(method.length);
        int dataLen=0;
		if(null!=data)
			dataLen=data.length;
		byte[] file=fileName.getBytes();
		byte[] fileHeader = Utils.toByte(file.length);  
		int len=method.length+header.length+fileHeader.length+file.length+dataLen;
		
		
		byte[]  req=new byte[len]; 
		System.arraycopy(header,0,req,0,header.length);
		System.arraycopy(method,0,req,header.length,method.length); 

		System.arraycopy(fileHeader,0,req, header.length+method.length,fileHeader.length);
		System.arraycopy(file,0,req, header.length+method.length+fileHeader.length,file.length);   
		if(null!=data)
			System.arraycopy(data,0,req, header.length+method.length+fileHeader.length+file.length,dataLen); 
		
		return  TcpHelper.sendVar(scs.Address,scs.Port,req,4) ;  
	}

	
	
	public LoginRes Login(LoginReq req) throws Exception {
		String msg=mapper.writeValueAsString(req);
		return mapper.readValue(Execute("Login",msg),LoginRes.class);
	}
	
	public <T> T Login(LoginReq req,Class<T> c) throws Exception {
		String msg=mapper.writeValueAsString(req);
		return mapper.readValue(Execute("Login",msg),c);
	}

	public int Regist(LoginReq req) throws Exception{
		String msg=mapper.writeValueAsString(req);
		return Utils.toInt(Execute("Regist",msg));

	} 

	/**
	 *执行分页返回数据:
	 * @param prp 是PagedRecordParameter对象,包含了数据的信息.
	 * @return json对象列表,和数据库的字段对应.
	 */
	public String ExecutePagedDataList(PagedRecordParameter prp) throws Exception{
			String msg=mapper.writeValueAsString(prp);
			return new String(Execute("ExecutePagedDataList",msg),encoding);
	} 
	
	public Object ExecutePagedDataList(PagedRecordParameter prp,Class<?> clazz)  throws Exception{
			String msg=mapper.writeValueAsString(prp);
			return mapper.readValue(new String(Execute("ExecutePagedDataList",msg),encoding),clazz);
	}   
	
	
	
	/**
	 * 执行数据库ExecuteNonQuery方法
	 * @param connectionString 连接串名字.
	 * @param cmdText 脚本名字 
	 * @param cmdType 脚本类型:1 Text 4 存储过程
	 * @param parameters 脚本参数
	 * @return 受影响的行数
	 */
	public int ExecuteNonQuery(String connectionString,String cmdText,int cmdType,DatabaseParameter...parameters)  throws Exception{
			String msg=mapper.writeValueAsString(new WrapedDatabaseParameter(connectionString,cmdText,cmdType,parameters));
			return  Utils.toInt(Execute("ExecuteNonQuery",msg));
	}
	
	/**
	 * 执行数据库ExecuteNonQuery方法
	 * @param cmdText 脚本名字 
	 * @param cmdType 脚本类型:1 Text 4 存储过程
	 * @param parameters 脚本参数
	 * @return 受影响的行数
	 */
	public int ExecuteNonQuery(String cmdText,int cmdType,DatabaseParameter...parameters)  throws Exception{
			String msg=mapper.writeValueAsString(new WrapedDatabaseParameter(scs.ConnectionName,cmdText,cmdType,parameters));
			return  Utils.toInt(Execute("ExecuteNonQuery",msg));
	}

	/**
	 * 执行数据库ExecuteNonQuery方法
	 * @param cmdText 脚本名字  
	 * @param parameters 脚本参数
	 * @return 受影响的行数
	 */
	public int ExecuteNonQuery(String cmdText,DatabaseParameter...parameters)  throws Exception{
//		try{
			String msg=mapper.writeValueAsString(new WrapedDatabaseParameter(scs.ConnectionName,cmdText,CommandType.Text.getValue(),parameters));
			return  Utils.toInt(Execute("ExecuteNonQuery",msg));
//		}
//		catch(Exception e){
//			return -1;
//		}
	}

	/**
	 * 执行数据库ExecuteNonQuery方法
	 * @param cmdText 脚本名字 
	 * @param cmdType 脚本类型:1 Text 4 存储过程
	 * @param parameters 脚本参数
	 * @return 受影响的行数
	 */
	public int ExecuteNonQuery(String cmdText,int cmdType,List<DatabaseParameter> parameters)  throws Exception{
			String msg=mapper.writeValueAsString(new WrapedDatabaseParameter(scs.ConnectionName,cmdText,cmdType,parameters));
			return  Utils.toInt(Execute("ExecuteNonQuery",msg));
	}

	/**
	 * 执行数据库ExecuteNonQuery方法
	 * @param cmdText 脚本名字  
	 * @param parameters 脚本参数
	 * @return 受影响的行数
	 */
	public int ExecuteNonQuery(String cmdText,List<DatabaseParameter> parameters)  throws Exception{
			String msg=mapper.writeValueAsString(new WrapedDatabaseParameter(scs.ConnectionName,cmdText,CommandType.Text.getValue(),parameters));
			return  Utils.toInt(Execute("ExecuteNonQuery",msg));
	} 

	/**
	 * 执行数据库ExecuteNonQuery方法 程
	 * @param parms 脚本参数:打包组合的脚本相关设置及参数
	 * @return 受影响的行数
	 */
	public int ExecuteNonQuery(WrapedDatabaseParameter parms)  throws Exception{
			String msg=mapper.writeValueAsString(parms);
			return  Utils.toInt(Execute("ExecuteNonQuery",msg));
	} 


	/**
	 * 执行数据库ExecuteScalar方法
	 * @param connectionString 连接串名字.
	 * @param cmdText 脚本名字 
	 * @param cmdType 脚本类型:1 Text 4 存储过程
	 * @param parameters 脚本参数
	 * @return  查询结果对象
	 */
	public Object ExecuteScalar(String connectionString,String cmdText,int cmdType,DatabaseParameter...parameters)  throws Exception{
//		try{
			String msg=mapper.writeValueAsString(new WrapedDatabaseParameter(connectionString,cmdText,cmdType,parameters));
			return  Utils.toObject(Execute("ExecuteScalar",msg));
//		}
//		catch(Exception e){
//			return "系统错误,稍后再试!"+e.getStackTrace();
//		}
	} 

	/**
	 * 执行数据库ExecuteScalar方法 
	 * @param cmdText 脚本名字 
	 * @param cmdType 脚本类型:1 Text 4 存储过程
	 * @param parameters 脚本参数
	 * @return  查询结果对象
	 */
	public Object ExecuteScalar(String cmdText,int cmdType,DatabaseParameter...parameters)  throws Exception{
//		try{
			String msg=mapper.writeValueAsString(new WrapedDatabaseParameter(scs.ConnectionName,cmdText,cmdType,parameters));
			return  new String(Execute("ExecuteScalar",msg));
			//			return  Utils.toObject(Execute("ExecuteScalar",msg));
//		}
//		catch(Exception e){
//			return "系统错误,稍后再试!"+e.getStackTrace();
//		}
	}  

	/**
	 * 执行数据库ExecuteScalar方法 
	 * @param cmdText 脚本名字  
	 * @param parameters 脚本参数
	 * @return  查询结果对象
	 */
	public Object ExecuteScalar(String cmdText,DatabaseParameter...parameters)  throws Exception{
//		try{
			String msg=mapper.writeValueAsString(new WrapedDatabaseParameter(scs.ConnectionName,cmdText,CommandType.Text.getValue(),parameters));
			return  Utils.toObject(Execute("ExecuteScalar",msg));
//		}
//		catch(Exception e){
//			return "系统错误,稍后再试!"+e.getStackTrace();
//		}
	} 

	/**
	 * 执行数据库ExecuteScalar方法 
	 * @param cmdText 脚本名字 
	 * @param cmdType 脚本类型:1 Text 4 存储过程
	 * @param parameters 脚本参数
	 * @return  查询结果对象
	 */
	public String ExecuteScalar(String cmdText,int cmdType,List<DatabaseParameter> parameters)  throws Exception{
//		try{
			String msg=mapper.writeValueAsString(new WrapedDatabaseParameter(scs.ConnectionName,cmdText,cmdType,parameters));
			return   new String(Execute("ExecuteScalar",msg));
//		}
//		catch(Exception e){
//			return "系统错误,稍后再试!"+e.getStackTrace();
//		}
	} 

	/**
	 * 执行数据库ExecuteScalar方法 
	 * @param cmdText 脚本名字  
	 * @param parameters 脚本参数
	 * @return  查询结果对象
	 */
	public Object ExecuteScalar(String cmdText,List<DatabaseParameter> parameters)  throws Exception{
//		try{
			String msg=mapper.writeValueAsString(new WrapedDatabaseParameter(scs.ConnectionName,cmdText,CommandType.Text.getValue(),parameters));
			return  Utils.toObject(Execute("ExecuteScalar",msg));
//		}
//		catch(Exception e){
//			return "系统错误,稍后再试!"+e.getStackTrace();
//		}
	}  

	/**
	 * 执行数据库ExecuteScalar方法 
	 * @param parms 脚本参数:打包组合的脚本相关设置及参数
	 * @return  查询结果对象
	 */
	public Object ExecuteScalar(WrapedDatabaseParameter parms)  throws Exception{
//		try{
			String msg=mapper.writeValueAsString(parms);
			return  Utils.toObject(Execute("ExecuteScalar",msg));
//		}
//		catch(Exception e){
//			return "系统错误,稍后再试!"+e.getStackTrace();
//		}
	} 


	/**
	 * 执行数据库ExecuteDataList方法
	 * @param connectionString 连接串名字.
	 * @param cmdText 脚本名字 
	 * @param cmdType 脚本类型:1 Text 4 存储过程
	 * @param parameters 脚本参数
	 * @return json对象列表,数据对应数据库表的各字段及值.
	 */
	public String ExecuteDataList(String connectionString,String cmdText,int cmdType,DatabaseParameter...parameters)  throws Exception{
//		try{
			String msg=mapper.writeValueAsString(new WrapedDatabaseParameter(connectionString,cmdText,cmdType,parameters));
			return new String(Execute("ExecuteDataList",msg),encoding);
//		}
//		catch(Exception e){
//			return "系统错误,稍后再试!"+e.getStackTrace();
//		}
	}   


	/**
	 * 执行数据库ExecuteDataList方法 
	 * @param cmdText 脚本名字 
	 * @param cmdType 脚本类型:1 Text 4 存储过程
	 * @param parameters 脚本参数
	 * @return json对象列表,数据对应数据库表的各字段及值.
	 */
	public String ExecuteDataList(String cmdText,int cmdType,DatabaseParameter...parameters)  throws Exception{
//		try{
			String msg=mapper.writeValueAsString(new WrapedDatabaseParameter(scs.ConnectionName,cmdText,cmdType,parameters));
			return new String(Execute("ExecuteDataList",msg),encoding);
//		}
//		catch(Exception e){
//			return "系统错误,稍后再试!"+e.getStackTrace();
//		}
	} 


	/**
	 * 执行数据库ExecuteDataList方法 
	 * @param cmdText 脚本名字  
	 * @param parameters 脚本参数
	 * @return json对象列表,数据对应数据库表的各字段及值.
	 */
	public String ExecuteDataList(String cmdText,DatabaseParameter...parameters)  throws Exception{
//		try{
			String msg=mapper.writeValueAsString(new WrapedDatabaseParameter(scs.ConnectionName,cmdText,CommandType.Text.getValue(),parameters));
			return new String(Execute("ExecuteDataList",msg),encoding);
//		}
//		catch(Exception e){
//			return "系统错误,稍后再试!"+e.getStackTrace();
//		}
	} 

	
	public <T> List<T> ExecuteDataList( Class<T> clazz,String cmdText,int cmdType,DatabaseParameter...parameters)  throws Exception{
//		try{
			String msg=mapper.writeValueAsString(new WrapedDatabaseParameter(scs.ConnectionName,cmdText,cmdType,parameters));
			List<T> list= mapper.readValue(new String(Execute("ExecuteDataList",msg),encoding),new TypeReference<List<T>>(){});
			return list;
//		}
//		catch(Exception e){
//			return  null;
//		}
	} 
	
	public <T> List<T> ExecuteDataList( Class<T> clazz,String cmdText,int cmdType,List<DatabaseParameter> parameters)  throws Exception{
//		try{
			String msg=mapper.writeValueAsString(new WrapedDatabaseParameter(scs.ConnectionName,cmdText,cmdType,parameters));
			List<T> list= mapper.readValue(new String(Execute("ExecuteDataList",msg),encoding),new TypeReference<List<T>>(){});
			return list;
//		}
//		catch(Exception e){
//			return  null;
//		}
	} 
	
	
	/**
	 * 执行数据库ExecuteDataList方法 
	 * @param cmdText 脚本名字  
	 * @param parameters 脚本参数
	 * @return json对象列表,数据对应数据库表的各字段及值.
	 */
	public <T> List<T> ExecuteDataList( Class<?> collectionClass,Class<T> elementClasses,String cmdText,int cmdType,DatabaseParameter...parameters)  throws Exception{
//		try{
			String msg=mapper.writeValueAsString(new WrapedDatabaseParameter(scs.ConnectionName,cmdText,cmdType,parameters));
			JavaType jt=mapper.getTypeFactory().constructParametricType(collectionClass, elementClasses); 
			return mapper.readValue(new String(Execute("ExecuteDataList",msg),encoding),jt);
//		}
//		catch(Exception e){
//			return  null;
//		}
	} 
	 

	public  <T> List<T> ExecuteDataList(Class<?> collectionClass,Class<T> elementClasses ,String cmdText,int cmdType,List<DatabaseParameter> parameters) throws Exception {
//		try{
			String msg=mapper.writeValueAsString(new WrapedDatabaseParameter(scs.ConnectionName,cmdText,cmdType,parameters));
			JavaType jt=mapper.getTypeFactory().constructParametricType(collectionClass, elementClasses); 
			return   mapper.readValue(new String(Execute("ExecuteDataList",msg),encoding), jt);
//		}
//		catch(Exception e){
//			return null;
//		}
	} 
 
	

	/**
	 * 执行数据库ExecuteDataList方法 
	 * @param cmdText 脚本名字 
	 * @param cmdType 脚本类型:1 Text 4 存储过程
	 * @param parameters 脚本参数
	 * @return json对象列表,数据对应数据库表的各字段及值.
	 */
	public String ExecuteDataList(String cmdText,int cmdType,List<DatabaseParameter> parameters)  throws Exception{
//		try{
			String msg=mapper.writeValueAsString(new WrapedDatabaseParameter(scs.ConnectionName,cmdText,cmdType,parameters));
			return new String(Execute("ExecuteDataList",msg),encoding);
//		}
//		catch(Exception e){
//			return "系统错误,稍后再试!"+e.getStackTrace();
//		}
	} 


	/**
	 * 执行数据库ExecuteDataList方法 
	 * @param cmdText 脚本名字  
	 * @param parameters 脚本参数
	 * @return json对象列表,数据对应数据库表的各字段及值.
	 */
	public String ExecuteDataList(String cmdText,List<DatabaseParameter> parameters) throws Exception {
//		try{
			String msg=mapper.writeValueAsString(new WrapedDatabaseParameter(scs.ConnectionName,cmdText,CommandType.Text.getValue(),parameters));
			return new String(Execute("ExecuteDataList",msg),encoding);
//		}
//		catch(Exception e){
//			return "系统错误,稍后再试!"+e.getStackTrace();
//		}
	} 

	

	/**
	 * 执行数据库ExecuteDataList方法 
	 * @param parms  脚本参数:打包组合的脚本相关设置及参数
	 * @return json对象列表,数据对应数据库表的各字段及值.
	 */
	public String ExecuteDataList(WrapedDatabaseParameter parms)  throws Exception{
//		try{
			String msg=mapper.writeValueAsString(parms);
			return new String(Execute("ExecuteDataList",msg),encoding);
//		}
//		catch(Exception e){
//			return "系统错误,稍后再试!"+e.getStackTrace();
//		}
	}
	
	  public byte[] GetFile(String name)  {
		  try{
			  byte[] bs=ExecuteFile("GetFile",name,null);
			  return bs;
		  }
		  catch(Exception e){ 
			   return null;
		  }
      }

      public String  SaveFile(String name, byte[] img) {
    	  try{
    		  return  new String(ExecuteFile("SaveFile",name, img));
    	  }
		  catch(Exception e){
				return e.getMessage();
		}
      }
      
      

} 
