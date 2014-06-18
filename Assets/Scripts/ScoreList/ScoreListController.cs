using UnityEngine;
using System.Collections;

public class ScoreListController : MonoBehaviour, ScoreViewListener {
	// Common GUI skin:
	public GUISkin guiSkin;

	// GUI styles for labels:
	public GUIStyle header1Style;
	public GUIStyle header2Style;
	public GUIStyle header2ErrorStyle; 
	public GUIStyle formFieldStyle;
	public GUIStyle errorMessageStyle;

	// Active view name:
	string activeViewName = ScoreView.NAME;

	// Map views by name:
	Hashtable viewByName;

	// Login view:
	ScoreView scoreView = new ScoreView();

	// Do we need block UI:
	bool blockUI = false;

	// This function will be called when scene loaded:
	public void Start () {   

	    // Setup of login view:
	    scoreView.guiSkin = guiSkin;
	    scoreView.header1Style = header1Style;
	    scoreView.header2Style = header2Style;
	    scoreView.header2ErrorStyle = header2ErrorStyle;
	    scoreView.formFieldStyle = formFieldStyle;

		scoreView.SetListener(this);
		
		viewByName = new Hashtable();
	    
	    // Adding login view to views by name map:
	    viewByName[ScoreView.NAME] = scoreView;
	}
		
	public void OnEnterGame() {
		Debug.Log("OnEnterGame");
		blockUI = true;
		Application.LoadLevel(2);
	}
	
	public void OnExitGame() {
		blockUI = true; 
		Application.LoadLevel(0);
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
	}
}
