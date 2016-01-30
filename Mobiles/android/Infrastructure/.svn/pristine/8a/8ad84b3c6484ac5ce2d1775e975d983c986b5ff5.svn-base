package wqfree.com;

import java.util.Properties;
import javax.xml.parsers.SAXParser;
import javax.xml.parsers.SAXParserFactory;
import java.net.URL;

public class ParseXML{
 
private Properties props;

//这里的props
public Properties getProps() {
return this.props;
}

public void parse(String filename)   {

	Configs handler=null;
	SAXParserFactory factory=null;
	SAXParser parser=null;
	try{
//将我们的解析器对象化
  handler = new Configs();

//获取SAX工厂对象
 factory = SAXParserFactory.newInstance();
factory.setNamespaceAware(false);
factory.setValidating(false);

//获取SAX解析
  parser = factory.newSAXParser();
 
URL confURL = Thread.currentThread().getContextClassLoader().getResource(filename);
 
parser.parse(confURL.toString(), handler); 
props = handler.getProps();
}
	catch(Exception e){
		e.printStackTrace();
	}
	
	finally{
factory=null;
parser=null;
handler=null;
}

}

} 