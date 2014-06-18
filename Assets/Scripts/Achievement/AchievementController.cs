using UnityEngine;
using System.Collections;

public class AchievementController : MonoBehaviour, AchievementViewListener {
	// Common GUI skin:
	public GUISkin guiSkin;

	// GUI styles for labels:
	public GUIStyle header1Style;
	public GUIStyle header2Style;
	public GUIStyle header2ErrorStyle; 
	public GUIStyle formFieldStyle;
	public GUIStyle errorMessageStyle;

	// Active view name:
	string activeViewName = AchievementView.NAME;

	// Map views by name:
	Hashtable viewByName;

	// Login view:
	AchievementView achievementView = new AchievementView();

	// Do we need block UI:
	bool blockUI = false;

	// This function will be called when scene loaded:
	public void Start () {   

	    // Setup of login view:
		achievementView.guiSkin 			= guiSkin;
		achievementView.header1Style 		= header1Style;
		achievementView.header2Style 		= header2Style;
		achievementView.header2ErrorStyle 	= header2ErrorStyle;
		achievementView.formFieldStyle 		= formFieldStyle;
	    
		achievementView.SetListener(this);
	    
	    viewByName = new Hashtable();
	    
	    // Adding login view to views by name map:
		viewByName[AchievementView.NAME] = achievementView;
	}

	public void OnEnterGame() {
		blockUI = true; 
		// Sending login request:
		//StartCoroutine(loginService.sendLoginData(loginView.data,
		//    loginResponseHandler));
		Application.LoadLevel(1);
	}

	public void OnExitGame() {
		blockUI = true; 
		// Sending login request:
		//StartCoroutine(loginService.sendLoginData(loginView.data,
		//    loginResponseHandler));
		Application.LoadLevel(3);
	}

	// This function will draw UI components
	public void OnGUI () {
		// Getting current view by active view name:
		View currentView = (View)viewByName[activeViewName];
	 
	    // Set blockUI for current view:
	    currentView.setBlockUI(blockUI);

	    // Rendering current view:
	    currentView.render();

	    // Show box with "Wait..." when UI is blocked:
	    var screenWidth = Screen.width;
	    var screenHeight = Screen.height;
	    if(blockUI) {
	        GUI.Box(new Rect((screenWidth - 200)/2, (screenHeight - 60)/2, 200, 60), "Wait...");
	   	}
	}

	// Processing login response from HTTP server:
	public void loginResponseHandler(Response response) {
	    blockUI = false;
	    //loginView.error = response.error;
	    //loginView.errorMessage = response.message;
	    //if(!loginView.error) {
	    //    UserSessionUtils.setUserLogin(loginView.data.login);
	    //    Application.LoadLevel(0);
	    //}
	}
}
