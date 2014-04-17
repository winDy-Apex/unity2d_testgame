class ScoreView implements View {

    public final static var NAME : String = "Score";

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
    private var isMyScoreOnly 	= false;
    private var isRankMode		= false;
    private var scores : System.Collections.Generic.List.<GBaaS.io.Objects.GBScoreObject>;
 
 	function getScore() {
 		var cs = GameObject.Find("GBaaSObject");
    	var script : GBaaSObject;
    	script = cs.GetComponent("GBaaSObject");
    	script.Init();
    	
    	if(isMyScoreOnly) {
    		scores = script.GetScore("1", "point", 10, true, false);
    	} else {
    		scores = script.GetScore("1", "point", 10, false, false);
    	}
    	
       	Debug.Log("In getScore of ScoreView result : " + scores.ToString());
 	}
 	
 	 function getRank() {
 		var cs = GameObject.Find("GBaaSObject");
    	var script : GBaaSObject;
    	script = cs.GetComponent("GBaaSObject");
    	script.Init();
    	
    	scores = script.GetRank("1", "point", "DESC", "WEEKLY", 0, 10);
    	
       	Debug.Log("In getRank of ScoreView result : " + scores.ToString());
 	}
 	
 	function getScoreBuf() {
 		var cs = GameObject.Find("GBaaSObject");
    	var script : GBaaSObject;
    	script = cs.GetComponent("GBaaSObject");
    	
    	//Debug.Log("in getScoreBuf script is : " + script.ToString());
    	
    	script.Init();
    	scores = script.GetScoreBuf();
 	}
 	
 	function scoreBufReset() {
 		var cs = GameObject.Find("GBaaSObject");
    	var script : GBaaSObject;
    	script = cs.GetComponent("GBaaSObject");
    	
    	script.ScoreBufReset();
 	}
 	
 	function pushNotify(msg) {
 		var cs = GameObject.Find("GBaaSObject");
    	var script : GBaaSObject;
    	script = cs.GetComponent("GBaaSObject");
    	
    	script.PushNotify(msg);
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
        GUI.Label(Rect(0, yShift, screenWidth, 30), "지바맨 랭킹 TOP 10", header1Style);
        
        getScoreBuf();
        if(scores == null) return;
        
        // Message label:
        if(error) {
            GUI.Label(Rect(0, yShift + 70, screenWidth, 30), errorMessage, header2ErrorStyle);
        } else {      
        	if(this.isRankMode) {
    		  	for(var i=0; i<scores.Count; i++) {
		       		GUI.Label(Rect(0, yShift + 70 + (30 * i), screenWidth, 30), "[" + scores[i].rank + "] " + scores[i].displayName + " : " + scores[i].score, header2Style);
		        }
        	} else {
		       	for(var j=0; j<scores.Count; j++) {
		       		GUI.Label(Rect(0, yShift + 70 + (30 * j), screenWidth, 30), scores[j].displayName + " : " + scores[j].score, header2Style);
		        }
			}
        }
        
        // Login button:
        if(GUI.Button(Rect(xShift, yShift + 420, 120, 30), "게임하기")) {
        	scoreBufReset();
            enterGameHandler();
        }
       
        // Switch to registration view button:
        if(GUI.Button(Rect(xShift + 140, yShift + 420, 120, 30), "나가기")) {
			Debug.Log("In Button Exit");
        	scoreBufReset();
        	logout();
            exitGameHandler();
        }
        
        // Login button:
        if(GUI.Button(Rect(xShift, yShift + 460, 260, 30), "자랑하기(push)")) {
            pushNotify("지바맨2 점수자랑");
        }
        
        // Login button:
        if(GUI.Button(Rect(xShift, yShift + 500, 120, 30), "내점수보기")) {
            setMyScoreMode(true);
            setRankMode(false);
            getScore();
        }
       
        // Switch to registration view button:
        if(GUI.Button(Rect(xShift + 140, yShift + 500, 120, 30), "최고점수보기")) {
            setMyScoreMode(false);
            setRankMode(false);
            getScore();
        }

        if(GUI.Button(Rect(xShift + 140, yShift + 540, 120, 30), "내등수보기")) {
            setMyScoreMode(false);
            setRankMode(true);
            getRank();
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
    
    public function setRankMode(boolValue) {
    	this.isRankMode = boolValue;
    }
}
