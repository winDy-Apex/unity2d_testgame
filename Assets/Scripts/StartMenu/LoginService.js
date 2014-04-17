import System;
import System.IO;

class LoginService {
    public function sendLoginData(loginData : LoginData, responseHandler : function(Response)) {
        var response : Response = new Response(); 
        
    	var cs = GameObject.Find("GBaaSObject");
    	var script : GBaaSObject;
    	script = cs.GetComponent("GBaaSObject");
    	script.Init();
    	
    	var result;
    	if(loginData.facebookToken.length > 0) {
    		result = script.LoginWithFacebook(loginData.facebookToken);
    	} else if(loginData.username.length == 0) {
    		Debug.Log("Try LoginWithoutID");
    		
    		result = script.LoginWithoutID("ABABABABABCCCCCCCCDDDD123");
    	
    		Debug.Log("After LoginWithoutID");
    		
    		//result = script.LoginWithoutID(GetUniqueUserKey());
    		//result = script.LoginWithoutID(GetUniqueUserKey());
    		result = script.UpdateUserName("John");
    		
    		var userInfo = script.GetUserInfo();
    		Debug.Log("GetUserInfo username : " + userInfo.username);
    		Debug.Log("GetUserInfo name : " + userInfo.name);
    	} else {
    		result = script.Login(loginData.username, loginData.password);
    	}
    	
    	yield result; // StartCoroutine must need yield ~!!!
    	
    	if(result == true) {
            Debug.Log("You are loggin successfully");
            script.loginName = loginData.username;
            response.error = false;
            response.message = "";
        } else {
            // Error was while connecting/reading response 
            // from server:
            Debug.Log("Error: Login");
            response.error = true;
            response.message = "Error: failed Login to server";
        }
        
        // Calling response handler:
        responseHandler(response);
    }
    
    private function GetUniqueUserKey() {
    	var fileName 	= "UserUUID.txt";
		var uuid 		= "";

        if (File.Exists(fileName)) {
            Debug.Log(fileName+" already exists.");
            var sr = File.OpenText(fileName);

        	var line = sr.ReadLine();

        	if(line != null) {
       			Debug.Log(line); // prints each line of the file
       			uuid = line;
       			sr.Close();
       			return uuid;
			}
        } else {
	        var sw = File.CreateText(fileName);
	        uuid = System.Guid.NewGuid().ToString();
	        
	        sw.WriteLine(uuid);
	        sw.Close();
		}
		
    	return uuid;
    }
}
