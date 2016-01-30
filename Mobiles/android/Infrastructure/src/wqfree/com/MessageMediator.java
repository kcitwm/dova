package wqfree.com;

import java.net.Socket;

public class MessageMediator {
 
	public String userToken;
	public int controlStatus=0;
	Socket socket=null;
	
	public MessageMediator(String userToken,int controlStatus,Socket socket){
		this.userToken=userToken;
		this.controlStatus=controlStatus;
		this.socket=socket; 
	}
	
}
