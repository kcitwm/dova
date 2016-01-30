package wqfree.com;
 
import java.util.HashMap;  
import java.util.Vector;
 

public class MQService   {

	//private String address=Configs.MessageAddress;
	//private int port=Configs.MessagePort;
	//private static HashMap<String,MessageMediator> connections=new HashMap<String,MessageMediator>();
	private static HashMap<String,MQEchoClient> clients=new HashMap<String,MQEchoClient>();
	//private Handler serviceHandler;
	//public ClientServiceListener listener;
	private Thread thread;
	
	Vector   listeners=new   Vector(); 
	 public  void addMqListener(ClientServiceListener   l){listeners.add(l);} 
	   
	 private void fireChanged(ServiceEvent event){
		 for(int   i=0;i<listeners.size();i++){ 
			 ClientServiceListener   l=(ClientServiceListener)listeners.elementAt(i); 
		        l.eventChanged(event);  
		      } 
	 }
	
//	public IBinder onBind(Intent arg0) {
//		Log.d(getClass().getSimpleName(), "onBind()");
//		return serviceStub;
//	}
 
//	public MQService(ClientServiceListener listener){
//		this.listener=listener; 
//	}
// 
	 
	 public int   send(String msg){
		 MQEchoClient client=clients.get(UserContext.UserToken()); 
		 int rtn=-1;
			if(null!=client) 
			{ 
				rtn= client.send(msg.getBytes() ); 
				 if(rtn==-1)
				 	start();
			}
			return rtn;
	 }
	 
	 public int   send(WQMessage msg){
		return send(JsonUtils.serialize(msg));
	 }
	 
	 
	public int send(String targetId,String msg){
		return send(msg);
	} 
	
	public int send(String targetId,WQMessage msg){
		msg.TargetID=targetId;
		return send(JsonUtils.serialize(msg));
	}
	
	
	public int receive(String userToken){
		MQEchoClient client=clients.get(userToken);
		if(null!=client){
			WQMessage msg=new  WQMessage();
			msg.TransactionID=Utils.getRadomLong(); 
			msg.ServiceName="ReceiveService"; 
			msg.UserToken=userToken; 
			msg.DeviceType=1; 
			return client.send(JsonUtils.serialize(msg).getBytes() );
		}
		return -1;
	}
	
	public void regist(String userToken,MQEchoClient client){
		try {   
			// UserContext.setUserToken(userToken); 
			if(null!=clients.get(userToken))
				clients.remove(userToken);
			WQMessage msg=new  WQMessage();
			msg.TransactionID=Utils.getRadomLong(); 
			msg.ServiceName="LoginService"; 
			msg.UserToken=userToken; 
			msg.DeviceType=1;
			//msg.Async=true;
			byte[] data= JsonUtils.serialize(msg).getBytes(); 
			int sent=client.send(data,2); 
			if(sent>0) 
				clients.put(userToken,client); 
			return; 
		}
		catch(Exception e){
			 	try{
				Thread.sleep(2000); 
			}
			catch(Exception ie){
			
			} 
		}
	}

	 
	public void onDestroy() { 
		//serviceHandler.removeCallbacks(runnable);
		//serviceHandler = null;
		thread.stop(); 
	}

	
	public void start() {   
		MessageRunnable mr=new MessageRunnable();
		//mr.token=token; 
		 thread = new Thread(mr); 
		 thread.start(); 
	}
	

//	public void setListener (String userToken,ClientServiceListener listener) {
//		MQEchoClient client=clients.get(userToken);
//		client.setListener(listener);
//	}

	private String getDeviceId(){ 
			return "";
	}
	
	class MessageRunnable implements  Runnable{ 

		
		//濂藉儚澶氫簡涓�眰绾跨▼,
		public void run() { 
			try{  			 
				MQEchoClient client= new MQEchoClient();
				client.addMqListener(new ClientServiceListener(){
					@Override
					public void eventChanged(ServiceEvent event){
						fireChanged(event);
					} 
				}
						);
				client.start();
				regist(UserContext.UserToken(),client);
			}  
			catch(Exception e){
			 	} 
		} 
	}
	
	

	 
//	 Runnable runnable = new  Runnable()  {
//		public void run() { 
//			try{  			 
//				serviceStub.regist(getDeviceId()); 
//				while(true){
//					try{
//					Set<String> keys=connections.keySet(); 
//					for(String token: keys)
//						serviceStub.receive(token);
//					}
//					catch(Exception e){
//						Thread.sleep(2000);
//					}
//				}
//			}
//			catch(Exception e){
//				Log.d(getClass().getSimpleName(),"Error:Runnable.run:"+e.getMessage());
//			}
//			//serviceHandler.postDelayed(this,1000L); 
//		}
//	};
//
//		public void receive(String userToken) {  
//			//while(true){  
//			try{
//				if(connections.containsKey(userToken)){ 
//					byte[] data=TcpHelper.receiveVar(connections.get(userToken).socket);
//					if(null!=data){
//						String msg=new String(data);
//						DoMessage(msg); 
//					}
////					else
////						Thread.sleep(1000);
//					return; 
//				} 
//				regist(userToken); 
//			}
//			catch(Exception e){
//				try{
//					Thread.sleep(1000);
//				}
//				catch(Exception ex){}
//				Log.d(getClass().getSimpleName(),"receive():"+e.getMessage());
//				regist(userToken);
//			}
//
//			//}    
//		}
//
//		 int msgNumber=0;
//		@SuppressWarnings("deprecation")
//		protected  void DoMessage(String msg){
//			if(!msg.equals("")){
//				//WQMessage message=JsonUtils.deserialize(msg,WQMessage.class);
//				
//			}
//		}
//
//		public int send(String userToken,String msg) {
//			// TODO Auto-generated method stub  
//			try {
//				return TcpHelper.sendVar(connections.get(userToken).socket,msg.getBytes());
//			}
//			catch(Exception e){
//				Log.d(getClass().getSimpleName(),"MQService.send:"+e.getMessage());
//			}
//			return -1;
//		}
//
//		public String regist(String userToken){
//			try { 
//				if(null==connections.get(userToken)){ 
//					UserContext.UserToken()=connect(userToken); 
//				} 
//			}
//			catch(Exception e){
//					Log.d(getClass().getSimpleName(),"MQService.regist:"+e.getMessage());
//					try{
//					Thread.sleep(2000);
//					regist(userToken);
//				}
//				catch(Exception ie){
//				
//				}
//			} 
//			return UserContext.UserToken();
//		}
//		 
//		public   String connect(String userToken) throws Exception{
//			WQMessage msg=new  WQMessage();
//			msg.TransactionID=Utils.getRadomLong(); 
//			msg.ServiceName="LoginService"; 
//			msg.UserToken=userToken; 
//			byte[] data= JsonUtils.serialize(msg).getBytes();
//			Socket socket=new Socket(address,port);
//			TcpHelper.sendVar(socket,data);
//			byte[] rb=TcpHelper.receiveVar(socket); 
//			MessageMediator mm=new MessageMediator(userToken,1,socket);
//			connections.put(userToken,mm); 
//			return new String(rb); 
//		} 
//	};
//	
}
