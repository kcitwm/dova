 package wqfree.com;
interface IMQService { 
	  void receive(String userToken);
	  int send(String userToken,String message); 
	  String regist(String id);   
	  void setParameters(String packageName,String className,int iconid);
}
