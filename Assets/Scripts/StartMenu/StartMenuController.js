// Common GUI skin:
public var guiSkin : GUISkin;

// GUI styles for labels:
public var header1Style : GUIStyle;
public var header2Style : GUIStyle;
public var header2ErrorStyle : GUIStyle; 
public var formFieldStyle : GUIStyle;
public var errorMessageStyle : GUIStyle;

// Active view name:
var activeViewName : String = LoginView.NAME;

// Map views by name:
var viewByName : Hashtable;

// Login view:
var loginView : LoginView;

// Registration view:
var registrationView : RegistrationView;

// Login service:
var loginService : LoginService;

// Registration service:
var registrationService : RegistrationService;

// Do we need block UI:
var blockUI = false;

// This function will be called when scene loaded:
function Start () {   

    // Setup of login view:
    loginView.guiSkin = guiSkin;
    loginView.header1Style = header1Style;
    loginView.header2Style = header2Style;
    loginView.header2ErrorStyle = header2ErrorStyle;
    loginView.formFieldStyle = formFieldStyle;
    
    // Handler of registration button click:
    loginView.openRegistrationHandler = function() {
        // Clear reistration fields:
        registrationView.data.clear();
        // Set active view to registration:
        activeViewName = RegistrationView.NAME;
    };
    
    loginView.enterGameHandler = function() {
        blockUI = true; 
        // Sending login request:
        StartCoroutine(loginService.sendLoginData(loginView.data,
            loginResponseHandler));
    };
    
    loginView.enterGameWithOutIDHandler = function() {
        blockUI = true; 
        // Sending login request:
        loginView.data.username = "";
        StartCoroutine(loginService.sendLoginData(loginView.data,
            loginResponseHandler));
    };
    
    // Setup of login view:
    registrationView.guiSkin = guiSkin;
    registrationView.header2Style = header2Style;
    registrationView.formFieldStyle = formFieldStyle;
    registrationView.errorMessageStyle = errorMessageStyle;

    // Handler of cancel button click:
    registrationView.cancelHandler = function() {
        // Clear reistration fields:
        loginView.data.clear();
        // Set active view to registration:
        activeViewName = LoginView.NAME;
    };
    
    // Handler of Register button:
	registrationView.registrationHandler = function() {
        blockUI = true;
        // Sending registration request:
        StartCoroutine(registrationService.sendRegistrationData(
            registrationView.data, registrationResponseHandler));
    };
    
    viewByName = new Hashtable();
    
    // Adding login view to views by name map:
    viewByName[LoginView.NAME] = loginView;
    viewByName[RegistrationView.NAME] = registrationView;
}

// This function will draw UI components
function OnGUI () {

    // Getting current view by active view name:
    var currentView : View = viewByName[activeViewName];
 
    // Set blockUI for current view:
    currentView.setBlockUI(blockUI);

    // Rendering current view:
    currentView.render();

    // Show box with "Wait..." when UI is blocked:
    var screenWidth = Screen.width;
    var screenHeight = Screen.height;
    if(blockUI) {
        GUI.Box(Rect((screenWidth - 200)/2, (screenHeight - 60)/2, 200, 60), "Wait...");
   	}
}

// Processing login response from HTTP server:
function loginResponseHandler(response : Response) {
    blockUI = false;
    loginView.error = response.error;
    loginView.errorMessage = response.message;
    if(!loginView.error) {
        //UserSessionUtils.setUserLogin(loginView.data.login);
        Application.LoadLevel(2); // go Achievement
    }
}

// Processing registration response from HTTP server:
function registrationResponseHandler(response : Response) {
    blockUI = false;
    registrationView.error = response.error;
    registrationView.errorMessage = response.message;
    if(!response.error) {
        //UserSessionUtils.setUserLogin(registrationView.data.login);
            
        // Set active view to registration:
        activeViewName = LoginView.NAME;
    }
}