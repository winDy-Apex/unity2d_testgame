class LoginView implements View {

    public final static var NAME : String = "Login";

    public var guiSkin : GUISkin;
    
    public var header1Style : GUIStyle;
    public var header2Style : GUIStyle;
    public var header2ErrorStyle : GUIStyle;
    public var formFieldStyle : GUIStyle;
    
    public var data : LoginData = new LoginData();
    
    public var error = false;
    public var errorMessage : String = "";
    
    public var enterGameHandler : function();
    public var enterGameWithOutIDHandler : function();
    public var openRegistrationHandler : function();

    private var blockUI = false;

    function render() {

        var screenWidth = Screen.width;
        var screenHeight = Screen.height;
    
        var xShift = (screenWidth - 260)/2;
        var yShift = (screenHeight - 500)/2;
       
        GUI.skin = guiSkin;
        
        // Disabling UI if blockUI is true: 
        GUI.enabled = !blockUI;

        // Main label:
        GUI.Label(Rect(0, yShift, screenWidth, 30), "지바맨 액션 (GBaaS 2D Game for CBT)", header1Style);
       
        // Message label:
        if(error) {
            GUI.Label(Rect(0, yShift + 30, screenWidth, 30), errorMessage, header2ErrorStyle);
        } else {
            GUI.Label(Rect(0, yShift + 30, screenWidth, 30), "Apexplatform 2D액션", header2Style);
        }
       
        // Login label and login text field:
        GUI.Label(Rect(xShift, yShift + 90, 100, 30), "사용자이름:", formFieldStyle);
        data.username = GUI.TextField(Rect(xShift + 110, yShift + 90, 150, 30), data.username, 16);
    
        // Password label and password text field:
        GUI.Label(Rect(xShift, yShift + 130, 100, 30), "암호:", formFieldStyle);
        data.password = GUI.PasswordField(Rect(xShift + 110, yShift + 130, 150, 30), data.password, "*"[0], 16);
        
        // FaceBook Access Token
        GUI.Label(Rect(xShift, yShift + 170, 100, 30), "Facebook Token:", formFieldStyle);
        data.facebookToken = GUI.TextField(Rect(xShift + 110, yShift + 170, 200, 30), data.facebookToken, 500);
       
        // Login button:
        if(GUI.Button(Rect(xShift, yShift + 220, 120, 30), "로그인")) {
            enterGameHandler();
        }
       
        // Switch to registration view button:
        if(GUI.Button(Rect(xShift + 140, yShift + 220, 120, 30), "가입")) {
            openRegistrationHandler();
        }

		// Login Without ID button:
        if(GUI.Button(Rect(xShift + 140, yShift + 260, 120, 30), "바로시작")) {
            enterGameWithOutIDHandler();
        }
        
        // Enabling UI: 

        GUI.enabled = true;

    }

    public function setBlockUI(blockUI) {
        this.blockUI = blockUI;
    }

}