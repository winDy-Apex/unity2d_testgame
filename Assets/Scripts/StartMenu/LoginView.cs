using UnityEngine;
using System.Net;
using System.IO;
using System;
using System.Collections;

public interface LoginViewListener {
	void OnEnterGame();
	void OnEnterGameWithOutID();
	void OnOpenRegistration();
}

public class LoginView : View {

	public const string NAME = "Login";

	public GUISkin guiSkin;
    
	public GUIStyle header1Style;
	public GUIStyle header2Style;
	public GUIStyle header2ErrorStyle;
	public GUIStyle formFieldStyle;
    
	public LoginData data = new LoginData();
    
    public bool error = false;
    public string errorMessage = "";
	
	private bool blockUI = false;

	private LoginViewListener _listener = null;

	public void SetListener(LoginViewListener listener) {
		_listener = listener;
	}

    public void enterGameHandler() {
		if(_listener != null) {
			_listener.OnEnterGame();
		}
	}

	public void enterGameWithOutIDHandler() {
		if(_listener != null) {
			_listener.OnEnterGameWithOutID();
		}
	}

	public void openRegistrationHandler() {
		if(_listener != null) {
			_listener.OnOpenRegistration();
		}
	}

	public void render() {

        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
    
        float xShift = (screenWidth - 260)/2;
        float yShift = (screenHeight - 500)/2;
       
        GUI.skin = guiSkin;
        
        // Disabling UI if blockUI is true: 
        GUI.enabled = !blockUI;

        // Main label:
        GUI.Label(new Rect(0, yShift, screenWidth, 30), "지바맨 액션 (GBaaS 2D Game for CBT)", header1Style);
       
        // Message label:
        if(error) {
            GUI.Label(new Rect(0, yShift + 30, screenWidth, 30), errorMessage, header2ErrorStyle);
        } else {
            GUI.Label(new Rect(0, yShift + 30, screenWidth, 30), "Apexplatform 2D액션", header2Style);
        }
       
        // Login label and login text field:
        GUI.Label(new Rect(xShift, yShift + 90, 100, 30), "ID:", formFieldStyle);
        data.username = GUI.TextField(new Rect(xShift + 110, yShift + 90, 150, 30), data.username, 16);
    
        // Password label and password text field:
        GUI.Label(new Rect(xShift, yShift + 130, 100, 30), "암호:", formFieldStyle);
        data.password = GUI.PasswordField(new Rect(xShift + 110, yShift + 130, 150, 30), data.password, "*"[0], 16);
        
        // FaceBook Access Token
        GUI.Label(new Rect(xShift, yShift + 170, 100, 30), "Facebook Token:", formFieldStyle);
        data.facebookToken = GUI.TextField(new Rect(xShift + 110, yShift + 170, 200, 30), data.facebookToken, 500);
       
        // Login button:
        if(GUI.Button(new Rect(xShift, yShift + 220, 120, 30), "로그인")) {
            enterGameHandler();
        }
       
        // Switch to registration view button:
        if(GUI.Button(new Rect(xShift + 140, yShift + 220, 120, 30), "가입")) {
            openRegistrationHandler();
        }

		// Login Without ID button:
        if(GUI.Button(new Rect(xShift + 140, yShift + 260, 120, 30), "바로시작")) {
            enterGameWithOutIDHandler();
        }

        // Enabling UI: 
        GUI.enabled = true;

    }
	
	public void setBlockUI(bool blockUI) {
		this.blockUI = blockUI;
	}
}
