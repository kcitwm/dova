package wqfree.com;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.util.logging.Logger;


public class TcpHelper {

    public static int send(Socket s, byte[] data) throws IOException {
        int len=data.length;
        OutputStream os=s.getOutputStream();  
        os.write(data); 
        return len;
    }
   
    public static int sendVar(Socket s, byte[] data) throws IOException{    
        return sendVar(s, data, 4);
    }

    public static byte[] sendVar(String address,int port, byte[] data, int lenHeader) throws IOException {
      Socket s = null;  
      try{
      s = new Socket(address,port); 
      sendVar(s, data, lenHeader); 
      byte[] bs= receiveVar(s, lenHeader);  
      int len=bs.length; 
      return bs;
      } finally{
    	  if(null!=s)
    	  s.close();
      }
    }

    public static int sendVar(Socket s, byte[] data, int lenHeader) throws IOException { 
    	 int len=data.length;
    	 byte[] dataSize = new byte[lenHeader]; 
    	 if(lenHeader==4)
    		 dataSize=Utils.toByte(len);
    	 else
    		 dataSize=Utils.toByte((long)len); 
         OutputStream os=s.getOutputStream(); 
         byte[] sendData=new byte[lenHeader+len];
         System.arraycopy(dataSize,0,sendData,0,lenHeader); 
         System.arraycopy(data,0,sendData,lenHeader,len);
         os.write(sendData); 
         return len; 
    }

    public static byte[] receiveVar(Socket s)   {
        return receiveVar(s, 4);
    }

    public static byte[] receiveVar(Socket s, int lenHeader)  { 
    	byte[] bs=null;
    	try{
    	byte[] dataSize = new byte[lenHeader];
    	int readed = 0;
    	InputStream is=null; 
    	try{
	    	is=s.getInputStream();
	    	if(is.available() >= 0){
		    	is.read(dataSize,0,lenHeader);
		    	int length=Utils.toInt(dataSize);
		    	int left = length;
		    	int total=0;
		    	bs = new byte[length];
		    	while (left > 0) { 
			    	readed = is.read(bs, total,left);
			    	if (readed >0) {
			    		total+=readed;
			    		left-=readed;
			    		continue;
			    	}
		    		break;
		    	}  
	    	}
    	}
    	catch(Exception e){
    		throw e;
    	}
    	finally{
    		 
    	}
    	}catch(Exception e){ 
    	}
    	//return handleByte(bs);
    	return bs;
    }

    public static byte[]  handleByte(byte[] data)
    {
    	int len=data.length;
		byte newdata[]=null;
	    if(data.length>=4&&data[len-1]==0&&data[len-2]==0&&data[len-3]==0&&data[len-4]==0){
	    	newdata=new byte[data.length-4];
			System.arraycopy(data, 0, newdata, 0, len-4);
	    }
	    if(newdata==null){
	    	return data;
	    }
	    else{
	    	return  newdata;	
	    }
    }


    public static String ReceiveVar(Socket s, String encoding, int lenHeader)  {
    	try{
        return new String(receiveVar(s, lenHeader),encoding);
    	}catch(Exception e){
    		return "";
    	}
    }
}
