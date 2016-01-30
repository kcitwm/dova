package wqfree.com;
 
import java.util.HashMap; 

import wqfree.com.Configs;

public class ServiceConnectionString {
	
	public String ConnectionName=Configs.DefaultConnectionName;
	public String Address=Configs.Address;
	public int Port=Configs.Port;
	public String ProviderName="System.Data.SqlClient";
	
	public ServiceConnectionString (){
	
	}

	
	public ServiceConnectionString(String address,int port){
		this.Address=address;
		this.Port=port;
	}
	
	public static HashMap<String,ServiceConnectionString> map=new HashMap<String,ServiceConnectionString>();
	
	static {
		ServiceConnectionString dacscs=new 	ServiceConnectionString(Configs.Address,Configs.Port);
		map.put(Configs.DefaultConnectionName,dacscs);
		ServiceConnectionString msgscs=new 	ServiceConnectionString(Configs.MessageAddress,Configs.MessagePort);
		map.put(Configs.MessageConnectionName,msgscs);
	}
	
	public static ServiceConnectionString getConnection(String connectionName){
		
		if(map.containsKey(connectionName))
			return map.get(connectionName);
		ServiceConnectionString conn=new ServiceConnectionString(Configs.Address,Configs.Port);
		map.put(connectionName,conn);
		return conn;
		
	}
	
	public static void LoadConnections(){
		
	}
	
}
