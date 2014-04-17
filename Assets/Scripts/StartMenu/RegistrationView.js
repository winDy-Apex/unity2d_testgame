class RegistrationView implements View {

    public final static var NAME : String = "Registration";

    public var guiSkin : GUISkin;
    
    public var header2Style : GUIStyle;
    public var formFieldStyle : GUIStyle;
    public var errorMessageStyle : GUIStyle;

    public var error = false;
    public var errorMessage : String = "";

    public var data : RegistrationData = new RegistrationData();
    
    public var registrationHandler : function();
    public var cancelHandler : function();
    
    private var blockUI = false;
    
    function setBlockUI(blockUI) {
    	this.blockUI = blockUI;
    } 

    function render() { 
    
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
        GUI.Label(Rect(0, yShift + 0, screenWidth, 30), "회원가입", header2Style);
        
        // Login label and text filed:
        yAlignPos = 50;
        GUI.Label(Rect(xShift, yShift + yAlignPos, 100, 30), "이름:", formFieldStyle);
        data.name = GUI.TextField(Rect(xShift + 110, yShift + yAlignPos, 250, 30), data.name, 16);
        
        // Login label and text filed:
        yAlignPos = 100;
        GUI.Label(Rect(xShift, yShift + yAlignPos, 100, 30), "사용자이름:", formFieldStyle);
        data.username = GUI.TextField(Rect(xShift + 110, yShift + yAlignPos, 250, 30), data.username, 16);
        
        // Password label and text filed:
        yAlignPos = 150;
        GUI.Label(Rect(xShift, yShift + yAlignPos, 100, 30), "암호:", formFieldStyle);
        data.password = GUI.PasswordField(Rect(xShift + 110, yShift + yAlignPos, 250, 30), data.password, "*"[0], 16);
        
        // Confirm password label and text filed:
        yAlignPos = 200;
        GUI.Label(Rect(xShift - 50, yShift + yAlignPos, 150, 30), "암호 확인:", formFieldStyle);
        data.passwordConfirm = GUI.PasswordField(Rect(xShift + 110, yShift + yAlignPos, 250, 30), data.passwordConfirm, "*"[0], 16);
        
        // Email label and text filed:
        yAlignPos = 250;
        GUI.Label(Rect(xShift, yShift + yAlignPos, 100, 30), "이메일:", formFieldStyle);
        data.email = GUI.TextField(Rect(xShift + 110, yShift + yAlignPos, 250, 30), data.email, 32);
        
        // Register button:
        yAlignPos = 300;
        if(GUI.Button(Rect(xShift + 50, yShift + yAlignPos, 120, 30), "등록")) {
            registrationHandler();
        }
        
        // Cancel button:
        if(GUI.Button(Rect(xShift + 190, yShift + yAlignPos, 120, 30), "취소")) {
            cancelHandler();
        }

        // Enabling UI:
        GUI.enabled = true;
        
        // Show errors:
        showErrors();
    }


    // In case of registration error render error window:
    private function showErrors() {
        if(error) {
            var screenWidth = Screen.width;
            var screenHeight = Screen.height;
            var windowRect : Rect;
            windowRect = GUI.Window (0, Rect((screenWidth - 400)/2, (screenHeight - 300)/2, 400, 300), 
                renderErrorWindow, "Registration Error");
        }
    }
    
    // Render error window content:
    private function renderErrorWindow(windowId : int) {
        GUI.Label(Rect(10, 30, 380, 230), errorMessage, errorMessageStyle);
        if(GUI.Button(Rect((400 - 120)/2, 260, 120, 30), "OK")) {
            error = false;
            errorMessage = "";
        }
    }
}