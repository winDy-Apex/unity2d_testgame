using UnityEngine;
using System.Collections.Generic;

using GBaaS.io;
using GBaaS.io.Objects;

public interface AchievementViewListener {
	void OnEnterGame();
	void OnExitGame();
}

public class AchievementView : GBaaSApiHandler, View {

    public const string NAME = "Achievement";

	public GUISkin guiSkin;
    
	public GUIStyle header1Style;
	public GUIStyle header2Style;
	public GUIStyle header2ErrorStyle;
	public GUIStyle formFieldStyle;
    
    public bool error = false;
    public string errorMessage = "";
    
	private AchievementViewListener _listener = null;
	
	public void SetListener(AchievementViewListener listener) {
		_listener = listener;
	}
	
	public void enterGameHandler() {
		if(_listener != null) {
			_listener.OnEnterGame();
		}
	}
	
	public void exitGameHandler() {
		if(_listener != null) {
			_listener.OnExitGame();
		}
	}
	
    private bool blockUI 		= false;
	private static List<GBAchievementObject> achievement = new List<GBAchievementObject>();
 
 	public void getAchievement() {
		GBaaSObject.Instance.Init(this);
		GBaaSObject.Instance.GetAchievement(GBaaSObject.loginName, 10, "ko-KR");
 	}
 	
	public override void OnGetAchievement(List<GBAchievementObject> result) {
		Debug.Log ("GBaaS OnGetAchievement AchievementView " + result.Count.ToString());
		achievement = result;
	}

	public void logout() {
		GBaaSObject.Instance.Init(this);
		GBaaSObject.Instance.Logout();
 	}
 	
    public void render() {

        var screenWidth = Screen.width;
        var screenHeight = Screen.height;
    
        var xShift = (screenWidth - 260)/2;
        var yShift = (screenHeight - 600)/2;
       
        GUI.skin = guiSkin;
        
        // Disabling UI if blockUI is true: 
        GUI.enabled = !blockUI;

        // Main label:
        GUI.Label(new Rect(0, yShift, screenWidth, 30), "지바맨 달성목표", header1Style);
        
		if(achievement.Count == 0) {
			getAchievement();
		}
		
        // Message label:
        if(error) {
            GUI.Label(new Rect(0, yShift + 70, screenWidth, 30), errorMessage, header2ErrorStyle);
        } else {      
	       	for(var i=0; i<achievement.Count; i++)
	       	{
	       		//Debug.Log(achievement[i].isUnlocked);
	       		
	       		if(achievement[i].isUnLocked) {
	       			//Debug.Log("isUnlocked");
	       			GUI.Label(new Rect(0, yShift + 70 + (60 * i), screenWidth, 30), achievement[i].achievementName + " : " + achievement[i].earnedDescription, header2Style);
	       		} else {
	       		    //Debug.Log("Achievement locked");
	       			GUI.Label(new Rect(0, yShift + 70 + (60 * i), screenWidth, 30), achievement[i].achievementName + " : " + achievement[i].preEarnedDescription, header2ErrorStyle);
	       		}
	        }
	    }
	    
        // Login button:
        if(GUI.Button(new Rect(xShift, yShift + 420, 120, 30), "게임시작")) {
            enterGameHandler();
        }
       
        // Switch to registration view button:
        if(GUI.Button(new Rect(xShift + 140, yShift + 420, 120, 30), "점수보기")) {
        	//logout();
            exitGameHandler();
        }

        // Enabling UI: 

        GUI.enabled = true;
    }

    public void setBlockUI(bool blockUI) {
        this.blockUI = blockUI;
    }
}
