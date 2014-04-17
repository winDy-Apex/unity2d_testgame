// Common GUI skin:
public var guiSkin : GUISkin;

// GUI styles for labels:
public var header1Style : GUIStyle;
public var header2Style : GUIStyle;
public var header2ErrorStyle : GUIStyle; 
public var formFieldStyle : GUIStyle;
public var errorMessageStyle : GUIStyle;

// Active view name:
var activeViewName : String = ScoreView.NAME;

// Map views by name:
var viewByName : Hashtable;

// Login view:
var scoreView : ScoreView;

// Do we need block UI:
var blockUI = false;

// This function will be called when scene loaded:
function Start () {   

    // Setup of login view:
    scoreView.guiSkin = guiSkin;
    scoreView.header1Style = header1Style;
    scoreView.header2Style = header2Style;
    scoreView.header2ErrorStyle = header2ErrorStyle;
    scoreView.formFieldStyle = formFieldStyle;
    
    scoreView.enterGameHandler = function() {
        blockUI = true;
        Application.LoadLevel(2);
    };
    
    scoreView.exitGameHandler = function() {
        blockUI = true; 
        Application.LoadLevel(0);
    };
    
    viewByName = new Hashtable();
    
    // Adding login view to views by name map:
    viewByName[ScoreView.NAME] = scoreView;
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
}
