package wqfree.com;

import java.io.UnsupportedEncodingException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;

public class EncryptHelper {
	public static  String sha1(String source) { 
		try{		
		MessageDigest md = null; 
			md = MessageDigest.getInstance("SHA-1"); 
			md.update(source.getBytes("UTF-8")); 
	    byte[] result = md.digest(); 
	    StringBuffer sb = new StringBuffer(); 
	    for (byte b : result) {
	        int i = b & 0xff;
	        if (i < 0xf) {
	            sb.append(0);
	        }
	        sb.append(Integer.toHexString(i));
	    }
	    return sb.toString().toUpperCase();
		}
		catch(Exception e){
			return "";
		} 
	}
}
