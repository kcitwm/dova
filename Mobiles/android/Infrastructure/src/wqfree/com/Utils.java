package wqfree.com;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.io.Serializable;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.security.SecureRandom;

public class Utils {

	public static int toInt(byte[] bs) { 
		int out = 0; 
		byte b;  
		for ( int i =0; i<4 ; i++) { 
			b = bs[i]; 
			out+= (b & 0xFF) << (8 * i);  
		}    
		return out; 
	}

	public static long toLong(byte[] bs){
		long out = 0;
		for(int i = 0;i < 8;i++) {	
			out |= (bs[7-i] & 0xffL) << (8L * i);
		}
		return out;
	}

	public static byte[] toByte(int n) {  
		ByteBuffer buffer = ByteBuffer.allocate(4).order(ByteOrder.nativeOrder());
		buffer.putInt(n);
		return buffer.array(); 
	} 


	public static Object toObject(byte[] data) {
		byte[] tt=getBytes(1);
		
		ByteArrayInputStream in = null;
		ObjectInputStream is=null;
		Object o=null;
		try{
			in=new ByteArrayInputStream(data);
			is = new ObjectInputStream(in);
			o= is.readObject();
		}
		catch(Exception e){ 
			String ms=e.getMessage();
			ms+=e.getStackTrace();
		}
		finally{ 
			try{
				is.close();
				in.close();
			}
			catch(Exception e){}
		}
		return o;
	}

	public static byte[] getBytes(Serializable obj)  
	{ 
		try{
			ByteArrayOutputStream bo = new ByteArrayOutputStream();
			ObjectOutputStream oo = new ObjectOutputStream(bo);
			oo.writeObject(obj); 
			bo.close();
			return bo.toByteArray();
		}
		catch(Exception e){
			return null;
		}
	}

	public static byte[] toByte(long i) {  
		byte[] b=new byte[8];
		b[0] = (byte)i;
		b[1] = (byte)(i>>8);
		b[2] = (byte)(i>>16);
		b[3] = (byte)(i>>24);
		b[4] = (byte)(i>>32);
		b[5] = (byte)(i>>40);
		b[6] = (byte)(i>>48);
		b[7] = (byte)(i>>56);
		return b;
	}  


	public static int unsignedToBytes(byte b) {
		return b & 0xFF;
	}

	public static long getRadomLong(){
		byte[] bs=new byte[32];
		SecureRandom rd=new SecureRandom();
		rd.nextBytes(bs);
		return Utils.toLong(bs);
	}

}
