using UnityEngine;

using GBaaS.io;
using GBaaS.io.Objects;

public interface LoginServiceListener {
	void	loginResponseHandler(Response response);
}

public class LoginService : GBaaSApiHandler {
	private LoginServiceListener _listener = null;

	public void SetListener(LoginServiceListener listener) {
		_listener = listener;
	}
	
	private GBaaSObject 	_gbaas = new GBaaSObject();
	private GBUserObject 	_userInfo;
	private LoginData		_loginData;

	public void sendLoginData(LoginData loginData) {
        
    	_gbaas.Init(this);
    	
		_loginData = loginData;

    	if(loginData.facebookToken.Length > 0) {
			_gbaas.LoginWithFacebook(loginData.facebookToken);
    	} else if(loginData.username.Length == 0) {
    		Debug.Log("Try LoginWithoutID");
			_gbaas.LoginWithoutID("ABABABABABCCCCCCCCDDDD123");
    	} else {
			_gbaas.Login(loginData.username, loginData.password);
    	}
    }

	public override void OnLogin(bool result) {
		Response response = new Response(); 

		if(result == true) {
			Debug.Log("You are loggin successfully");
			GBaaSObject.loginName = _loginData.username;
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
		_listener.loginResponseHandler(response);
	}

	public override void OnLoginWithFaceBook(bool result) {
		OnLogin(result);
	}

	public override void OnLoginWithoutID(bool result) {
		Debug.Log("OnLoginWithoutID");

		if(result == true) {
			_gbaas.UpdateUserName("John");
		}

		OnLogin(result);
	}
	
	public override void OnUpdateUserName(bool result) {
		if(result == true) {
			_userInfo = _gbaas.GetUserInfo();
		}
	}

	public override void OnGetUserInfo(GBUserObject result) {
		if(result != default(GBUserObject)) {
			Debug.Log("GetUserInfo username : "	+ _userInfo.username);
			Debug.Log("GetUserInfo name : " 	+ _userInfo.name);
			OnLogin(true);
		}
	}
    
    /*
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
    */
}
