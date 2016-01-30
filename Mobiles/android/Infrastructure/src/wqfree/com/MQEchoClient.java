package wqfree.com;
 
import java.util.Vector;
 

public class MQEchoClient extends EchoClient {
 
	 
	
	 Vector   listeners=new   Vector(); 
	 public  void addMqListener(ClientServiceListener   l){listeners.add(l);} 
	   
	 private void fireChanged(ServiceEvent event){
		 for(int   i=0;i<listeners.size();i++){ 
			 ClientServiceListener   l=(ClientServiceListener)listeners.elementAt(i); 
		        l.eventChanged(event);  
		      } 
	 }
	  
	
	@Override 
	protected void Execute(byte[] receiveData){
		String msg=new String(receiveData);
		ServiceEvent event=new 	 ServiceEvent(msg);
		fireChanged(event); 
	}
	
}
