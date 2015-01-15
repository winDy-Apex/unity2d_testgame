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
	
	private GBaaSObject 	_gbaas; // = new GBaaSObject();
	private GBUserObject 	_userInfo;
	private LoginData		_loginData;

	public void sendLoginData(LoginData loginData) {
		Debug.Log("In sendLoginData");

		_gbaas = GBaaSObject.Instance;
		_gbaas.Init(this);
    	
		_loginData = loginData;

    	if(loginData.facebookToken.Length > 0) {
			_gbaas.API.LoginWithFaceBook(loginData.facebookToken);
    	} else if(loginData.username.Length == 0) {
    		Debug.Log("Try LoginWithoutID");
			_gbaas.API.LoginWithoutID("ABABABABABCCCCCCCCDDDD12345");
		} else {
			Debug.Log("Try Login");
			_gbaas.API.Login(loginData.username, loginData.password);
    	}
    }

	public override void OnLogin(GBResult result) {
		Response response = new Response(); 

		if(result.isSuccess == true) {
			Debug.Log("You are loggin successfully loginName:" + _loginData.username);
			GBaaSObject.loginName = _loginData.username;
			response.error = false;
			response.message = "";
		} else {
			// Error was while connecting/reading response 
			// from server:
			Debug.Log("Error: OnLogin : " + result.reason);
			response.error = true;
			response.message = "Error: failed Login to server";
		}

		// Calling response handler:
		_listener.loginResponseHandler(response);
	}

	public override void OnLoginWithFaceBook(GBResult result) {
		OnLogin(result);
	}

	public override void OnLoginWithoutID(GBResult result) {
		Debug.Log("OnLoginWithoutID");

		if(result.isSuccess == true) {
			_gbaas.API.UpdateUserName("John");
		}

		OnLogin(result);
	}
	
	public override void OnUpdateUserName(GBResult result) {
		if(result.isSuccess) {
			_userInfo = _gbaas.API.GetUserInfo();
		}
	}

	public override void OnGetUserInfo(GBUserObject result) {
		if(result != default(GBUserObject)) {
			_userInfo = result;
			Debug.Log("GetUserInfo username : "	+ _userInfo.username);
			Debug.Log("GetUserInfo name : " 	+ _userInfo.name);
			OnLogin(new GBResult { isSuccess = true, returnCode = ReturnCode.Success, reason = "" });
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
