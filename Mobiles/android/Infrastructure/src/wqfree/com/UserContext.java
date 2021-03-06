package wqfree.com;

import java.io.Serializable;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonInclude;

/*
 * 1. login
 * 2. UserContext context=  UserContext;
 * 3. context.UserName=....;
 * 4. context.save();  //save instance
 * 5.不能序列化static 的变量,不序列化null值的变量
 */

@JsonInclude(JsonInclude.Include.NON_NULL)
@JsonIgnoreProperties(ignoreUnknown = true)
@JsonFormat(pattern = "yyyy-MM-dd HH:mm:ss", timezone = "GMT+8")
public class UserContext implements Serializable {

	private static final long serialVersionUID = 1L;

	public String UserGid = "";
	public long UserId = 0;
	public String UserName = "";
	public String UserRealName = "";
	public String UserMobile = "";
	public boolean IsLogin = false;
	public String UserToken = "";
	public long UserType = 4;// 默认为司机
	public String OrgGid = "";
	public long OrgId = 0;
	public String OrgCode = "";
	public String OrgName = "";
	public int OrgStatus;
	public int Status;
	
//	public int FlagOrgUser;
//	public int FlagOrgAdmin;
	public long GroupId = 0;
	public String GroupName = "";
	public String GroupCode = "";
	public String VehicleCode = "";
	public int LocationType = 1;// 1:来自手机 2:来自车载 3:来自其他
	public String ServiceConnectString = Configs.DefaultConnectionName;
	public String MessageAddress = Configs.MessageAddress;
	public int MessagePort = Configs.MessagePort;
	public String ServiceAddress = Configs.Address;
	public int ServicePort = Configs.Port;
	public ServiceConnectionString uscs = new ServiceConnectionString();
	 
	// 保存当前的经纬度 值
//	public  double Latitude = 39.904965;
//	public  double Longitude = 116.327764;
//	public  String Province="";
//	public  String City="";
//	public  String District="";
//	public  String Addr="";
	
	
//	public List<String> Rights = new ArrayList<String>();
	
	static UserContext instance = new UserContext();

	public static UserContext getInstance() {
		if (instance == null)
			instance = new UserContext();
		return instance;
	}

	public static void save() {
		// serialize instance file save "系统存储";
		String json = JsonUtils.serialize(instance);
		UtilityFile.writeFile(json, Configs.StoragePath, "usercontext");
	}

	public static void InitContext() {
		try {
			String json = UtilityFile.readFile(Configs.StoragePath,
					"usercontext");
			UserContext context = JsonUtils
					.deserialize(json, UserContext.class);
			if (null != context)
				instance = context;
		} catch (Exception e) {
		}
	}

	public String UserGid() {
		if (null == instance || instance.UserToken == null) {
			InitContext();
		}
		return instance.UserGid;
	}

	public static long UserId() {
		if (null == instance || instance.UserToken == null
				|| instance.UserId == 0) {
			InitContext();
		}
		return instance.UserId;
	}

	public static String UserName() {
		if (null == instance || instance.UserToken == null
				|| instance.UserName == "") {
			InitContext();
		}
		return instance.UserName;
	}

	public static String UserRealName() {
		if (null == instance || instance.UserToken == null
				|| instance.UserRealName == "") {
			InitContext();
		}
		return instance.UserRealName;
	}

	public static String UserMobile() {
		if (null == instance || instance.UserToken == null
				|| instance.UserMobile == "") {
			InitContext();
		}
		return instance.UserMobile;
	}

	public static boolean IsLogin() {
		if (null == instance || instance.UserToken == null || !instance.IsLogin) {
			InitContext();
		}
		return instance.IsLogin;
	}

	public static String UserToken() {
		if (null == instance || instance.UserToken == null
				|| instance.UserToken == "") {
			InitContext();
		}
		return instance.UserToken;
	}

	public static long UserType() {
		if (null == instance || instance.UserToken == null
				|| instance.UserType == 4) {
			InitContext();
		}
		return instance.UserType;
	}

	public static String OrgGid() {
		if (null == instance || instance.UserToken == null
				|| instance.OrgGid == "") {
			InitContext();
		}
		return instance.OrgGid;
	}

	public static long OrgId() {
		if (null == instance || instance.UserToken == null
				|| instance.OrgId == 0) {
			InitContext();
		}
		return instance.OrgId;
	}

	public static String OrgCode() {
		if (null == instance || instance.OrgCode == null
				|| instance.OrgCode == "") {
			InitContext();
		}
		return instance.OrgCode;
	}

	public static String OrgName() {
		if (null == instance || instance.UserToken == null
				|| instance.OrgName == "") {
			InitContext();
		}
		return instance.OrgName;
	}

	public static int OrgStatus() {
		if (null == instance || instance.UserToken == null
				|| instance.OrgStatus == 0) {
			InitContext();
		}
		return instance.OrgStatus;
	}

	public static int Status(){
		if (null == instance || instance.UserToken == null
				|| instance.Status == 0) {
			InitContext();
		}
		return instance.Status;
	}
//	public static int FlagOrgUser() {
//		if (null == instance || instance.UserToken == null
//				|| instance.FlagOrgUser == 0) {
//			InitContext();
//		}
//		return instance.FlagOrgUser;
//	}
//
//	public static int FlagOrgAdmin() {
//		if (null == instance || instance.UserToken == null
//				|| instance.FlagOrgAdmin == 0) {
//			InitContext();
//		}
//		return instance.FlagOrgAdmin;
//	}

	public static long GroupId() {
		if (null == instance || instance.UserToken == null
				|| instance.GroupId == 0) {
			InitContext();
		}
		return instance.GroupId;
	}

	public static String GroupName() {
		if (null == instance || instance.UserToken == null
				|| instance.GroupName == "") {
			InitContext();
		}
		return instance.GroupName;
	}

	public static String GroupCode() {
		if (null == instance || instance.UserToken == null
				|| instance.GroupCode == "") {
			InitContext();
		}
		return instance.GroupCode;
	}

	public static String VehicleCode() {
		if (null == instance || instance.UserToken == null
				|| instance.VehicleCode == "") {
			InitContext();
		}
		return instance.VehicleCode;
	}

	public static int LocationType() {
		if (null == instance || instance.UserToken == null
				|| instance.LocationType == 1) {
			InitContext();
		}
		return instance.LocationType;
	}

	public static String ServiceConnectString() {
		if (instance == null) {
			InitContext();
		}
		return instance.ServiceConnectString;
	}

	public static String MessageAddress() {
		if (instance == null) {
			InitContext();
		}
		return instance.MessageAddress;
	}

	public static int MessagePort() {
		if (instance == null) {
			InitContext();
		}
		return instance.MessagePort;
	}

	public static String ServiceAddress() {
		if (instance == null) {
			InitContext();
		}
		return instance.ServiceAddress;
	}

	public static int ServicePort() {
		if (instance == null) {
			InitContext();
		}
		return instance.ServicePort;
	}

	public static ServiceConnectionString uscs() {
		if (instance == null) {
			InitContext();
		}
		return instance.uscs;
	}

//	// 保存当前的经纬度 值
//	public static double Latitude() {
//		// if(instance==null||Latitude==39.904965){
//		InitContext();
//		// }
//		return instance.Latitude;
//	}
//
//	public static double Longitude() {
//		// if(instance==null||Longitude==116.327764){
//		InitContext();
//		// }
//		return instance.Longitude;
//	}
//
//	public static String Province() {
//		// if(instance==null||"".equalsIgnoreCase(Province)){
//		InitContext();
//		// }
//		return instance.Province;
//	}
//
//	public static String City() {
//		// if(instance==null||"".equalsIgnoreCase(City)){
//		InitContext();
//		// }
//		return instance.City;
//	}
//
//	public static String District() {
//		// if(instance==null||"".equalsIgnoreCase(District)){
//		InitContext();
//		// }
//		return instance.District;
//	}
//
//	public static String Addr() {
//		// if(instance==null||"".equalsIgnoreCase(Addr)){
//		InitContext();
//		// }
//		return instance.Addr;
//	}
//
//	public static void setDistrict(String District) {
//		instance.District = District;
//	}
//
//	public static void setProvince(String Province) {
//		instance.Province = Province;
//	}
//
//	public static void setCity(String City) {
//		instance.City = City;
//	}
//
//	public static void setAddr(String Addr) {
//		instance.Addr = Addr;
//	}

	public static void setUserGid(String userGid) {
		instance.UserGid = userGid;
	}

	public static void setUserId(long userId) {
		instance.UserId = userId;
	}

	public static void setUserName(String userName) {
		instance.UserName = userName;
	}

	public static void setUserRealName(String userRealName) {
		instance.UserRealName = userRealName;
	}

	public static void setUserMobile(String userMobile) {
		instance.UserMobile = userMobile;
	}

	public static void setIsLogin(boolean isLogin) {
		instance.IsLogin = isLogin;
	}

	public static void setUserToken(String userToken) {
		instance.UserToken = userToken;
	}

	public static void setUserType(long userType) {
		instance.UserType = userType;
	}

	public static void setOrgGid(String orgGid) {
		instance.OrgGid = orgGid;
	}

	public static void setOrgId(long orgId) {
		instance.OrgId = orgId;
	}

	public static void setOrgCode(String orgCode) {
		instance.OrgCode = orgCode;
	}

	public static void setOrgName(String orgName) {
		instance.OrgName = orgName;
	}

	public static void setGroupId(long groupId) {
		instance.GroupId = groupId;
	}

	public static void setGroupName(String groupName) {
		instance.GroupName = groupName;
	}

	public static void setGroupCode(String groupCode) {
		instance.GroupCode = groupCode;
	}

	public static void setVehicleCode(String vehicleCode) {
		instance.VehicleCode = vehicleCode;
	}

	public static void setLocationType(int locationType) {
		instance.LocationType = locationType;
	}

	public static void setServiceConnectString(String serviceConnectString) {
		instance.ServiceConnectString = serviceConnectString;
	}

	public static void setMessageAddress(String messageAddress) {
		instance.MessageAddress = messageAddress;
	}

	public static void setMessagePort(int messagePort) {
		instance.MessagePort = messagePort;
	}

	public static void setServiceAddress(String serviceAddress) {
		instance.ServiceAddress = serviceAddress;
	}

	public static void setServicePort(int servicePort) {
		instance.ServicePort = servicePort;
	}

	public static void setUscs(ServiceConnectionString uscs) {
		instance.uscs = uscs;
	}

//	public static void setRights(List<String> rights) {
//		instance.Rights = rights;
//	}

//	public static void setLatitude(double latitude) {
//		instance.Latitude = latitude;
//	}
//
//	public static void setLongitude(double longitude) {
//		instance.Longitude = longitude;
//	}

	public static void setOrgStatus(int orgStatus) {
		instance.OrgStatus = orgStatus;
	}

	public static void setStatus(int Status) {
		instance.Status = Status;
	}
	
//	public static void setFlagOrgUser(int flagOrgUser) {
//		instance.FlagOrgUser = flagOrgUser;
//	}
//
//	public static void setFlagOrgAdmin(int flagOrgAdmin) {
//		instance.FlagOrgAdmin = flagOrgAdmin;
//	}


}
