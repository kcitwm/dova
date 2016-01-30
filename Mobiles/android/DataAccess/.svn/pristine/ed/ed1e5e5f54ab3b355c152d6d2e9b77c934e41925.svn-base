package wqfree.com.dac;

import java.util.HashMap;
import java.util.Map;

public enum ParameterDirection {

	 // 摘要:
    //     参数是输入参数。
    Input (1),
    //
    // 摘要:
    //     参数是输出参数。
    Output (2),
    //
    // 摘要:
    //     参数既能输入，也能输出。
    InputOutput (3),
    //
    // 摘要:
    //     参数表示诸如存储过程、内置函数或用户定义函数之类的操作的返回值。
    ReturnValue (6);
    
private int value;  
    
ParameterDirection(int value) {  
        this.value = value;  
        registerValue(); //map.put(value, this);  
    }  
      
    @Override  
    public String toString() {  
        return Integer.toString(this.value);  
    }  
      
    private void registerValue() {   
    	if(null==ParameterDirection.map)
    		ParameterDirection.map= new HashMap<Integer, ParameterDirection>();
    	ParameterDirection.map.put(value, this);  
    }  
  
    public static ParameterDirection fromInt(int i) {  
        return ParameterDirection.map.get(i);  
    }  
      
    private static   Map<Integer, ParameterDirection> map = new HashMap<Integer, ParameterDirection>(); 
    
}
