class AchievementView implements View {

    public final static var NAME : String = "Achievement";

    public var guiSkin : GUISkin;
    
    public var header1Style : GUIStyle;
    public var header2Style : GUIStyle;
    public var header2ErrorStyle : GUIStyle;
    public var formFieldStyle : GUIStyle;
    
    public var error = false;
    public var errorMessage : String = "";
    
    public var enterGameHandler : function();
    public var exitGameHandler : function();

    private var blockUI 		= false;
    private var achievement : System.Collections.Generic.List.<GBaaS.io.Objects.GBAchievementObject>;
 
    private var isMyScoreOnly : boolean;
    
 	function getAchievement() {
 		var cs = GameObject.Find("GBaaSObject");
    	var script : GBaaSObject;
    	script = cs.GetComponent("GBaaSObject");
    	script.Init();
    	
 		achievement = script.GetAchievement(script.loginName, 10, "ko-KR");
       	//print(result.ToString());
 	}
 	
 	function getAchievementBuf() {
 		var cs = GameObject.Find("GBaaSObject");
    	var script : GBaaSObject;
    	script = cs.GetComponent("GBaaSObject");
    	
    	script.Init();
    	achievement = script.GetAchievementBuf();
       	//Debug.Log(achievement[0].EarnedDescription);
 	}
 	
 	function achievementBufReset() {
 		var cs = GameObject.Find("GBaaSObject");
    	var script : GBaaSObject;
    	script = cs.GetComponent("GBaaSObject");
    	
    	script.AchievementBufReset();
 	}

 	function logout() {
 		var cs = GameObject.Find("GBaaSObject");
    	var script : GBaaSObject;
    	script = cs.GetComponent("GBaaSObject");
    	
    	script.Init();
    	script.Logout();
 	}
 	
    function render() {

        var screenWidth = Screen.width;
        var screenHeight = Screen.height;
    
        var xShift = (screenWidth - 260)/2;
        var yShift = (screenHeight - 600)/2;
       
        GUI.skin = guiSkin;
        
        // Disabling UI if blockUI is true: 
        GUI.enabled = !blockUI;

        // Main label:
        GUI.Label(Rect(0, yShift, screenWidth, 30), "지바맨 달성목표", header1Style);
        
        getAchievementBuf();
        
        // Message label:
        if(error) {
            GUI.Label(Rect(0, yShift + 70, screenWidth, 30), errorMessage, header2ErrorStyle);
        } else {      
	       	for(var i=0; i<achievement.Count; i++)
	       	{
	       		//Debug.Log(achievement[i].isUnlocked);
	       		
	       		if(achievement[i].isUnlocked) {
	       			//Debug.Log("isUnlocked");
	       			GUI.Label(Rect(0, yShift + 70 + (60 * i), screenWidth, 30), achievement[i].achievementName + " : " + achievement[i].earnedDescription, header2Style);
	       		} else {
	       		    //Debug.Log("Achievement locked");
	       			GUI.Label(Rect(0, yShift + 70 + (60 * i), screenWidth, 30), achievement[i].achievementName + " : " + achievement[i].preEarnedDescription, header2ErrorStyle);
	       		}
	        }
	    }
	    
        // Login button:
        if(GUI.Button(Rect(xShift, yShift + 420, 120, 30), "게임시작")) {
        	achievementBufReset();
            enterGameHandler();
        }
       
        // Switch to registration view button:
        if(GUI.Button(Rect(xShift + 140, yShift + 420, 120, 30), "점수보기")) {
        	achievementBufReset();
        	//logout();
            exitGameHandler();
        }

        // Enabling UI: 

        GUI.enabled = true;
    }

    public function setBlockUI(blockUI) {
        this.blockUI = blockUI;
    }
    
    public function setMyScoreMode(boolValue) {
        this.isMyScoreOnly = boolValue;
    }
}