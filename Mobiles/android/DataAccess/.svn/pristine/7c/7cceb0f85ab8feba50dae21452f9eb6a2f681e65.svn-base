package wqfree.com.dac;

import java.util.HashMap;
import java.util.Map;

public enum CommandType {

	 // 摘要:
    //     SQL 文本命令。（默认。）
    Text (1),
    //
    // 摘要:
    //     存储过程的名称。
    StoredProcedure (4),
    //
    // 摘要:
    //     表的名称。
    TableDirect (512);
    
    private int value; 
    
    CommandType(int value) {  
            this.value = value;  
            registerValue(); //map.put(value, this);  
        }  
          
        @Override  
        public String toString() {  
            return Integer.toString(this.value);  
        }  
          
        private void registerValue() { 
        	if(null==CommandType.map)
        		CommandType.map= new HashMap<Integer, CommandType>();
        	CommandType.map.put(value, this);  
        }  
        
        public   int getValue(){
        	return value; 
        }
      
        public static CommandType fromInt(int i) {  
            return CommandType.map.get(i);  
        }  
          
        private static  Map<Integer, CommandType> map =null; 
         
}
