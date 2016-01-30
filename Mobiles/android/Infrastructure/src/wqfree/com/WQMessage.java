package wqfree.com;

import java.util.Date;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonInclude; 

@JsonInclude(JsonInclude.Include.NON_NULL) 
@JsonIgnoreProperties(ignoreUnknown = true)
public class WQMessage
{ 
	public long    TransactionID; //int64类型整数,要求随机生成唯一
	public boolean Async=false;
	public String  UserToken=""; //当前用户的token,
	public String  AppId="";//应用程序ID,在IOS中是官方生成的appid
	public int DeviceType=1;//1 表示anroid 2 表示IOS 
	public String  ServiceName="" ;//服务名字
	public int     MethodID=0;  //方法id  
	public String     MethodName="";  //方法id 
	public String  TargetID=""; //发送目标,对方的UserToken
	public String  Autho="" ;//认证串
	public String  Format ="json";  // 格式,默认为json
	public int 	   Status=0;//响应的消息的业务状态
	public String Message="";  //业务返回的提示消息
	public String  Body="" ;//传输的消息体
} 