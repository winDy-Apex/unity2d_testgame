using UnityEngine;
using System.Collections;

public class StartMenuController : MonoBehaviour, LoginViewListener, RegistrationViewListener, LoginServiceListener, RegistrationServiceListener {
	// Common GUI skin:
	public GUISkin guiSkin;

	// GUI styles for labels:
	public GUIStyle header1Style;
	public GUIStyle header2Style;
	public GUIStyle header2ErrorStyle; 
	public GUIStyle formFieldStyle;
	public GUIStyle errorMessageStyle;
	
	// Active view name:
	string activeViewName = LoginView.NAME;
	
	// Map views by name:
	Hashtable viewByName;
	
	// Login view:
	LoginView loginView = new LoginView();
	
	// Registration view:
	RegistrationView registrationView = new RegistrationView();
	
	// Login service:
	LoginService loginService = new LoginService();
	
	// Registration service:
	RegistrationService registrationService = new RegistrationService();
	
	// Do we need block UI:
	bool 	blockUI 	= false;
	int 	_jumpLevel 	= -1;

	// Implement LoginView Handler;
	public void OnOpenRegistration() {
		// Clear reistration fields:
		registrationView.data.clear();
		// Set active view to registration:
		activeViewName = RegistrationView.NAME;
	}

	public void OnEnterGame() {
		blockUI = true; 
		// Sending login request:
		//StartCoroutine(loginService.sendLoginData(loginView.data));
		loginService.sendLoginData(loginView.data);

	}

	public void OnEnterGameWithOutID() {
		blockUI = true; 
		// Sending login request:
		loginView.data.username = "";
		//StartCoroutine(loginService.sendLoginData(loginView.data));
		loginService.sendLoginData(loginView.data);
	}
	
	// Handler of cancel button click:
	public void OnCancel() {
		// Clear reistration fields:
		loginView.data.clear();
		// Set active view to registration:
		activeViewName = LoginView.NAME;
	}
	
	// Handler of Register button:
	public void OnRegistration() {
		blockUI = true;
		// Sending registration request:
		//StartCoroutine(registrationService.sendRegistrationData(
		//	registrationView.data, registrationResponseHandler));
		registrationService.sendRegistrationData(registrationView.data);
	}

	// This function will be called when scene loaded:
	public void Start () {   
		header1Style.normal.textColor = Color.green;
		header1Style.fontSize = 22;
		header1Style.alignment = TextAnchor.MiddleCenter;

		header2Style.normal.textColor = Color.green;
		header2Style.fontSize = 18;
		header2Style.alignment = TextAnchor.MiddleCenter;

		header2ErrorStyle.normal.textColor = Color.red;
		header2ErrorStyle.fontSize = 18;
		header2ErrorStyle.alignment = TextAnchor.MiddleCenter;

		formFieldStyle.normal.textColor = Color.white;
		formFieldStyle.fontSize = 16;
		formFieldStyle.alignment = TextAnchor.MiddleRight;
		
		errorMessageStyle.normal.textColor = Color.red;
		errorMessageStyle.fontSize = 16;
		errorMessageStyle.fontStyle = FontStyle.Bold;
		errorMessageStyle.alignment = TextAnchor.UpperLeft;
		
		// Setup of login view:
		loginView.guiSkin = guiSkin;
		loginView.header1Style = header1Style;
		loginView.header2Style = header2Style;
		loginView.header2ErrorStyle = header2ErrorStyle;
		loginView.formFieldStyle = formFieldStyle;

		// Setup of login view:
		registrationView.guiSkin = guiSkin;
		registrationView.header2Style = header2Style;
		registrationView.formFieldStyle = formFieldStyle;
		registrationView.errorMessageStyle = errorMessageStyle;

		loginView.SetListener(this);
		registrationView.SetListener(this);
		loginService.SetListener(this);
		registrationService.SetListener(this);

		viewByName = new Hashtable();
		
		// Adding login view to views by name map:
		viewByName[LoginView.NAME] = loginView;
		viewByName[RegistrationView.NAME] = registrationView;
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

		if(_jumpLevel > -1) {
			Application.LoadLevel(_jumpLevel); // go Achievement
			_jumpLevel = -1;
		}

		//Debug.Log("In OnGUI");
	}
	
	// Processing login response from HTTP server:
	public void loginResponseHandler(Response response) {
		blockUI = false;
		loginView.error = response.error;
		loginView.errorMessage = response.message;
		if(!loginView.error) {
			//UserSessionUtils.setUserLogin(loginView.data.login);
			Debug.Log("In loginResponseHandler Call LoadLevel(2)");
			//Application.LoadLevel(2); // go Achievement, Call By MainThread Only
			_jumpLevel = 2;
		}
	}
	
	// Processing registration response from HTTP server:
	public void registrationResponseHandler(Response response) {
		blockUI = false;
		registrationView.error = response.error;
		registrationView.errorMessage = response.message;
		if(!response.error) {
			//UserSessionUtils.setUserLogin(registrationView.data.login);
			
			// Set active view to registration:
			activeViewName = LoginView.NAME;
		}
	}
}
