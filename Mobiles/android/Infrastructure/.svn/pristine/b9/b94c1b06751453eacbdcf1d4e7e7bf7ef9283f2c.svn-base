package wqfree.com;

import java.util.Properties; 

import org.xml.sax.Attributes;
import org.xml.sax.helpers.DefaultHandler;
import org.xml.sax.SAXException;  
 
public class Configs extends DefaultHandler {
	 
	private Properties props; 
	private String currentName;
	private StringBuffer currentValue = new StringBuffer();
    static String url="configs.xml"; 

	public static String Address="114.215.197.135";
	public static int Port=18000;
	public static String DefaultConnectionName="DefaultConnection";
	public static String LoginConnectionName="DefaultConnection";
	
	public static String MessageConnectionName="MessageConnectionName";
	
	public static String MessageAddress="114.215.197.135";
	public static int MessagePort=19000;
	
	public static String StoragePath="/sdcard/weiyun/la/";
	
	
	public static int AuthoType=0;
	
	
	//构建器初始化props
	public Configs() {

	this.props = new Properties();
	}
 
	
	private static  Properties realProps;
	
	static {
		try {
			//ParseXML myRead = new ParseXML();
			//myRead.parse(url);
			//realProps = new Properties();
			//realProps = myRead.getProps();
			} catch (Exception e) {
			e.printStackTrace();
			}
		//Address=Configs.getString("address");
		//Port=Configs.getInt("port");
	}
	
	public Properties getProps() {
	return this.props;
	}
	
	public static String getString(String key){
		return realProps.getProperty(key);
	}
	

	public static int getInt(String key){
		return Integer.parseInt(realProps.getProperty(key));
	}
	
	public static long getLong(String key){
		return Long.parseLong(realProps.getProperty(key));
	}
 
	public void startElement(String uri, String localName, String qName, Attributes attributes)
	throws SAXException {
	currentValue.delete(0, currentValue.length());
	this.currentName =qName;

	}

	public void characters(char[] ch, int start, int length) throws SAXException {

	currentValue.append(ch, start, length);

	}

	public void endElement(String uri, String localName, String qName) throws SAXException {

	props.put(qName.toLowerCase(), currentValue.toString().trim());
	}
	

}
