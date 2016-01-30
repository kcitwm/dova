package wqfree.com;

import java.io.ByteArrayOutputStream;
import java.io.DataInputStream;
import java.io.IOException;
import java.net.InetSocketAddress;
import java.net.SocketAddress;
import java.net.SocketTimeoutException;
import java.nio.ByteBuffer;
import java.nio.IntBuffer;
import java.nio.channels.SelectionKey;
import java.nio.channels.Selector;
import java.nio.channels.SocketChannel;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Scanner;
import java.util.Set;
import java.util.logging.Logger;
 


public class EchoClient {


	private static DateFormat dateFormatter = new SimpleDateFormat("yyyyMMdd HH:mm:ss");

 

	boolean connected=false;

	String address=UserContext.MessageAddress();
	int port=UserContext.MessagePort();

	SocketChannel client = null;
	public EchoClient(){

	}

	public EchoClient(String address,int port){
		this.address=address;
		this.port=port;
	}
	public int send(byte[] data,int wait){ 
		int i=0;
		while(!connected)  {
			try{
				i++;
				Thread.sleep(2000);
			}catch(Exception e){}
			if(i>5) { 
				return -1;
			}
		} 
		return send(data);
	}
	
	public int send(byte[] data){ 
		if(!connected ||  ((new Date().getTime()-date.getTime())/1000>30))
		{ 
			return -1;
		}
		int total = 0;
		int size = data.length; 
		int left = size;
		int sent=-1; 
		try{ 
			byte[] sizeData=Utils.toByte(data.length);
			ByteBuffer sendBuffer=ByteBuffer.allocate(sizeData.length+size);  
			sendBuffer.put(sizeData);
			sendBuffer.put(data); // 灏嗘暟鎹畃ut杩涚紦鍐插尯 
			sendBuffer.flip();
			 while (total < size)
			{
				sent = client.write(sendBuffer);
				if(sent<=0)
					break;
				total += sent;
				left -= sent;
			} 
		}
		catch(Exception e){
			log("Send to server: 澶辫触 " );
		}
		return  sent; 
	}

	protected void Execute(byte[] receiveData){
		log(new String(receiveData));
	}

	public void start(){
		connected=false;
		Thread t=new Thread(runnable);
		t.start();
	}
	
	Date date=new Date();
	
	public void startreceive() {
		Selector selector=null;
		try {
			 
			int echo = -100; // 瑕佸彂閫佺殑鏁版嵁

			long read=0;
			while (true) { 
				try{  
					if(null==client){
						selector = Selector.open(); // 瀹氫箟涓�釜璁板綍濂楁帴瀛楅�閬撲簨浠剁殑瀵硅薄 
						client = connect(selector);
						date=new Date();
					}  
					long span=(new Date().getTime()-date.getTime())/1000;
					if(span>30) {  
						selector = Selector.open(); 
						client=connect(selector); 
					} 
					if(span>10){
						ByteBuffer sendbuffer = ByteBuffer.allocate(4); // 瀹氫箟鐢ㄦ潵瀛樺偍鍙戦�鏁版嵁鐨刡yte缂撳啿鍖�
						sendbuffer.put(Utils.toByte(echo)); // 灏嗘暟鎹畃ut杩涚紦鍐插尯 
						sendbuffer.flip(); // 灏嗙紦鍐插尯鍚勬爣蹇楀浣�鍥犱负鍚戦噷闈ut浜嗘暟鎹爣蹇楄鏀瑰彉瑕佹兂浠庝腑璇诲彇鏁版嵁鍙戝悜鏈嶅姟鍣�灏辫澶嶄綅
						client.write(sendbuffer); 
					}
				 
					// 鍒╃敤寰幆鏉ヨ鍙栨湇鍔″櫒鍙戝洖鐨勬暟鎹�
					// 濡傛灉瀹㈡埛绔繛鎺ユ病鏈夋墦寮�氨閫�嚭寰幆 
					// 姝ゆ柟娉曚负鏌ヨ鏄惁鏈変簨浠跺彂鐢熷鏋滄病鏈夊氨闃诲,鏈夌殑璇濊繑鍥炰簨浠舵暟閲�
					int shijian = selector.select(10000);
					// 濡傛灉娌℃湁浜嬩欢杩斿洖寰幆
					if (shijian == 0) {
						Thread.sleep(2000);
						continue;
					}
					// 瀹氫箟涓�釜涓存椂鐨勫鎴风socket瀵硅薄
					SocketChannel sc;
					// 閬嶄緥鎵�湁鐨勪簨浠�
					for (SelectionKey key : selector.selectedKeys()) {
						// 鍒犻櫎鏈浜嬩欢
						selector.selectedKeys().remove(key);
						// 濡傛灉鏈簨浠剁殑绫诲瀷涓簉ead鏃�琛ㄧず鏈嶅姟鍣ㄥ悜鏈鎴风鍙戦�浜嗘暟鎹�
						if (key.isReadable()) {
							// 灏嗕复鏃跺鎴风瀵硅薄瀹炰緥涓烘湰浜嬩欢鐨剆ocket瀵硅薄
							sc = (SocketChannel) key.channel();
							// 瀹氫箟涓�釜鐢ㄤ簬瀛樺偍鎵�湁鏈嶅姟鍣ㄥ彂閫佽繃鏉ョ殑鏁版嵁
							ByteArrayOutputStream bos = new ByteArrayOutputStream();
							// 灏嗙紦鍐插尯娓呯┖浠ュ涓嬫璇诲彇 
							// 姝ゅ惊鐜粠鏈簨浠剁殑瀹㈡埛绔璞¤鍙栨湇鍔″櫒鍙戦�鏉ョ殑鏁版嵁鍒扮紦鍐插尯涓� 
							ByteBuffer lenBuffer = ByteBuffer.allocate(4);  
							int len=sc.read(lenBuffer); 
							if(len<=-1){
								selector = Selector.open();
								connect(selector); 
								break;
							}
							byte[] bytes = new byte[4];  
							lenBuffer.flip();
							lenBuffer.get(bytes, 0, bytes.length);
							len=Utils.toInt(bytes);
							if(len!=-100) { 
								ByteBuffer readBuffer=ByteBuffer.allocate(len);
								int left=len;
								int total=0; 
								while ( left>0) {
									// 灏嗘湰娆¤鍙栫殑鏁版嵁瀛樺埌byte娴佷腑
									read=sc.read(readBuffer);
									if(read>0){
										total+=read;
										left-=read;
										bos.write(readBuffer.array()); 
										continue;
									} 
									readBuffer.clear();
								}
								// 濡傛灉byte娴佷腑瀛樻湁鏁版嵁
								if (bos.size() > 0) {
									// 寤虹珛涓�釜鏅�瀛楄妭鏁扮粍瀛樺彇缂撳啿鍖虹殑鏁版嵁
									Execute(bos.toByteArray()); 
								} 
							}
							else{
								date=new Date();
							}
						}
						else if(key.isWritable()){
							log("Send to server: key.isWritable()"   );

						}
						else if(!key.isValid()){

							log("Send to server: !key.isValid()"   );
						} 
					} 
				//Thread.sleep(2000); 
				}
				catch(Exception ie){   
					Thread.sleep(2000);
				} 
			}
		} catch (Exception e) {
			e.printStackTrace();  
		} finally {
			// 鍏抽棴瀹㈡埛绔繛鎺�姝ゆ椂鏈嶅姟鍣ㄥ湪read璇诲彇瀹㈡埛绔俊鎭殑鏃跺�浼氳繑鍥�1 
		}
	}

	
	 Runnable runnable = new  Runnable()  {
			public void run() { 
				try{  			 
					startreceive();
				}
				catch(Exception e){
					log("EchoClient:Error:Runnable.run:"+e.getMessage());
				}
				//serviceHandler.postDelayed(this,1000L); 
			}
		};
		
//		private SocketChannel reconnect(Selector selector,boolean close)  {
////			if (client != null) {
////				try {
////					 client.close();   
////					 client=null;
////					 selector=null;
////					connected=false;
////				} catch (Exception e) {
////				} 
////			} 
//			try{ 
//				selector = Selector.open(); // 瀹氫箟涓�釜璁板綍濂楁帴瀛楅�閬撲簨浠剁殑瀵硅薄 
//				client = connect(selector);
//				return client;
//			}
//			catch(Exception e){
//				 return null;
//			} 
//		}
	private SocketChannel connect(Selector selector) throws Exception {

		try{ 
			SocketAddress sa = new InetSocketAddress(address, port); // 瀹氫箟涓�釜鏈嶅姟鍣ㄥ湴鍧�殑瀵硅薄 
			client = SocketChannel.open(sa); // 瀹氫箟寮傛瀹㈡埛绔�
	
			client.configureBlocking(false); // 灏嗗鎴风璁惧畾涓哄紓姝�
	
			client.register(selector, SelectionKey.OP_READ); // 鍦ㄨ疆璁璞′腑娉ㄥ唽姝ゅ鎴风鐨勮鍙栦簨浠�灏辨槸褰撴湇鍔″櫒鍚戞瀹㈡埛绔彂閫佹暟鎹殑鏃跺�)
			while(!client.isOpen()){
				connect(selector);
				try{
					Thread.sleep(3000);
				}
				catch(Exception e){}
			}
			connected=true;
			return client;
		}
		catch(SocketTimeoutException e){
			throw new  SocketTimeoutException("EchoClient.connect:"+e.getMessage()); 
		}
		catch(Exception e){
			throw new  SocketTimeoutException("EchoClient.connect:"+e.getMessage()); 
		}
	}

	private static void log(Object msg) {
		System.out.println("CLIENT [" + dateFormatter.format(new Date()) + "]: " + msg);
	}
}