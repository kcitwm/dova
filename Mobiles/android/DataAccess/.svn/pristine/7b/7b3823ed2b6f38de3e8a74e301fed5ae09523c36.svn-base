package wqfree.com.dac;

import java.util.HashMap;
import java.util.Map;

public enum DataRowVersion {


    // 摘要:
    //     该行中包含其原始值。
    Original (256),
    //
    // 摘要:
    //     该行中包含当前值。
    Current (512),
    //
    // 摘要:
    //     该行中包含建议值。
    Proposed (1024),
    //
    // 摘要:
    //     System.Data.DataRowState 的默认版本。对于 Added、Modified 或 Deleted 的 DataRowState
    //     值，默认版本是 Current。对于 Detached 的 System.Data.DataRowState 值，该版本是 Proposed。
    Default (1536);
    
    
private int value;  
    
    DataRowVersion(int value) {  
        this.value = value;  
        registerValue(); //map.put(value, this);  
    }  
      
    @Override  
    public String toString() {  
        return Integer.toString(this.value);  
    }  
      
    private void registerValue() {   
    	if(null==DataRowVersion.map)
    		DataRowVersion.map= new HashMap<Integer, DataRowVersion>();
        DataRowVersion.map.put(value, this);  
    }  
  
    public static DataRowVersion fromInt(int i) {  
        return DataRowVersion.map.get(i);  
    }  
      
    private static   Map<Integer, DataRowVersion> map = new HashMap<Integer, DataRowVersion>(); 
    
}
