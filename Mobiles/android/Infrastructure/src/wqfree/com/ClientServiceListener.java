package wqfree.com;

 

public class ClientServiceListener {

	public String userToken="";

	public ClientServiceListener(){
		this.userToken=UserContext.UserToken();
	}
	public ClientServiceListener(String userToken){
		this.userToken=userToken;
	}
	
	public void eventChanged(ServiceEvent event){
		
	}
	


}
