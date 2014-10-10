using UnityEngine;

interface RegistrationViewListener {
	void OnRegistration();
	void OnCancel();
}

class RegistrationView : View {

    public const string NAME = "Registration";

	public GUISkin guiSkin;
    
	public GUIStyle header2Style;
	public GUIStyle formFieldStyle;
	public GUIStyle errorMessageStyle;

    public bool error;
    public string errorMessage = "";

	public RegistrationData data = new RegistrationData();
	
	private RegistrationViewListener _listener = null;
	
	public void SetListener(RegistrationViewListener listener) {
		_listener = listener;
	}

    public void registrationHandler() {
		_listener.OnRegistration();
	}
    
	public void cancelHandler() {
		_listener.OnCancel();
	}
    
    private bool blockUI = false;
    
    public void setBlockUI(bool blockUI) {
    	this.blockUI = blockUI;
    } 

    public void render() { 
    
        var screenWidth = Screen.width;
        var screenHeight = Screen.height;
    
        var xShift = (screenWidth - 360)/2;
        var yShift = (screenHeight - 500)/2;
        var yAlignPos = 0;
        
        GUI.skin = guiSkin;
        
        // Disable UI in case of blockUI is true or any error:
        if(error || blockUI){
            GUI.enabled = false;
        } else {
            GUI.enabled = true;
        }

        // Message label:
        GUI.Label(new Rect(0, yShift + 0, screenWidth, 30), "회원가입", header2Style);
        
        // Login label and text filed:
        yAlignPos = 50;
        GUI.Label(new Rect(xShift, yShift + yAlignPos, 100, 30), "이름:", formFieldStyle);
        data.name = GUI.TextField(new Rect(xShift + 110, yShift + yAlignPos, 250, 30), data.name, 16);
        
        // Login label and text filed:
        yAlignPos = 100;
        GUI.Label(new Rect(xShift, yShift + yAlignPos, 100, 30), "ID:", formFieldStyle);
        data.username = GUI.TextField(new Rect(xShift + 110, yShift + yAlignPos, 250, 30), data.username, 16);
        
        // Password label and text filed:
        yAlignPos = 150;
        GUI.Label(new Rect(xShift, yShift + yAlignPos, 100, 30), "암호:", formFieldStyle);
        data.password = GUI.PasswordField(new Rect(xShift + 110, yShift + yAlignPos, 250, 30), data.password, "*"[0], 16);
        
        // Confirm password label and text filed:
        yAlignPos = 200;
        GUI.Label(new Rect(xShift - 50, yShift + yAlignPos, 150, 30), "암호 확인:", formFieldStyle);
        data.passwordConfirm = GUI.PasswordField(new Rect(xShift + 110, yShift + yAlignPos, 250, 30), data.passwordConfirm, "*"[0], 16);
        
        // Email label and text filed:
        yAlignPos = 250;
        GUI.Label(new Rect(xShift, yShift + yAlignPos, 100, 30), "이메일:", formFieldStyle);
        data.email = GUI.TextField(new Rect(xShift + 110, yShift + yAlignPos, 250, 30), data.email, 32);
        
        // Register button:
        yAlignPos = 300;
        if(GUI.Button(new Rect(xShift + 50, yShift + yAlignPos, 120, 30), "등록")) {
            registrationHandler();
        }
        
        // Cancel button:
        if(GUI.Button(new Rect(xShift + 190, yShift + yAlignPos, 120, 30), "취소")) {
            cancelHandler();
        }

        // Enabling UI:
        GUI.enabled = true;
        
        // Show errors:
        showErrors();
    }

    // In case of registration error render error window:
    private void showErrors() {
        if(error) {
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;
            Rect windowRect;
            windowRect = GUI.Window (0, new Rect((screenWidth - 400)/2, (screenHeight - 300)/2, 400, 300), 
                renderErrorWindow, "Registration Error");
        }
    }
    
    // Render error window content:
    private void renderErrorWindow(int windowId) {
        GUI.Label(new Rect(10, 30, 380, 230), errorMessage, errorMessageStyle);
        if(GUI.Button(new Rect((400 - 120)/2, 260, 120, 30), "OK")) {
            error = false;
            errorMessage = "";
        }
    }
}