﻿using UnityEngine;
using System.Collections.Generic;

using GBaaS.io;
using GBaaS.io.Objects;

public interface ScoreViewListener {
	void OnEnterGame();
	void OnExitGame();
}

class ScoreView : GBaaSApiHandler, View {

    public const string NAME = "Score";

	public GUISkin guiSkin;
    
	public GUIStyle header1Style;
	public GUIStyle header2Style;
	public GUIStyle header2ErrorStyle;
	public GUIStyle formFieldStyle;
    
    public bool error = false;
    public string errorMessage = "";
    
    private bool blockUI 		= false;
    private bool isMyScoreOnly 	= false;
    private bool isRankMode		= false;
	private List<GBScoreObject> scores = new List<GBScoreObject>();
 
	private ScoreViewListener _listener = null;
	
	public void SetListener(ScoreViewListener listener) {
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
	
	private GBaaSObject _gbaas = new GBaaSObject();

	public void getScore() {
		Debug.Log("In getScore");
		_gbaas.Init(this);
    	
    	if(isMyScoreOnly) {
			_gbaas.GetScore("1", "point", 10, true, false);
    	} else {
    		_gbaas.GetScore("1", "point", 10, false, false);
    	}
 	}
 	
	public override void OnGetScore(List<GBScoreObject> result) {
		scores = result;
		Debug.Log("In getScore of ScoreView result : " + scores.ToString());
	}

 	public void getRank() {
		_gbaas.Init(this);
		_gbaas.GetRank("1", "point", "DESC", "WEEKLY", 0, 10);
 	}
	
	public override void OnGetRank(List<GBScoreObject> result) {
		scores = result;
		
		Debug.Log("In getRank of ScoreView result : " + scores.ToString());
	}
 	
 	public void getScoreBuf() {
		_gbaas.Init(this);
		_gbaas.GetScoreBuf();
 	}
 	
 	public void scoreBufReset() {
		_gbaas.Init(this);
    	_gbaas.ScoreBufReset();
 	}
 	
 	public void pushNotify(string msg) {
		_gbaas.Init(this);
		_gbaas.PushNotify(msg);
 	}

 	public void logout() {
		_gbaas.Init(this);
		_gbaas.Logout();
 	}
 	
    public void render() {

        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
    
        float xShift = (screenWidth - 260)/2;
        float yShift = (screenHeight - 600)/2;
       
        GUI.skin = guiSkin;
        
        // Disabling UI if blockUI is true: 
        GUI.enabled = !blockUI;

        // Main label:
        GUI.Label(new Rect(0, yShift, screenWidth, 30), "지바맨 랭킹 TOP 10", header1Style);
        
        getScoreBuf();
        if(scores == null) return;
        
        // Message label:
        if(error) {
            GUI.Label(new Rect(0, yShift + 70, screenWidth, 30), errorMessage, header2ErrorStyle);
        } else {      
        	if(this.isRankMode) {
    		  	for(var i=0; i<scores.Count; i++) {
		       		GUI.Label(new Rect(0, yShift + 70 + (30 * i), screenWidth, 30), "[" + scores[i].rank + "] " + scores[i].displayName + " : " + scores[i].score, header2Style);
		        }
        	} else {
		       	for(var j=0; j<scores.Count; j++) {
		       		GUI.Label(new Rect(0, yShift + 70 + (30 * j), screenWidth, 30), scores[j].displayName + " : " + scores[j].score, header2Style);
		        }
			}
        }
        
        // Login button:
        if(GUI.Button(new Rect(xShift, yShift + 420, 120, 30), "게임하기")) {
        	scoreBufReset();
            enterGameHandler();
        }
       
        // Switch to registration view button:
        if(GUI.Button(new Rect(xShift + 140, yShift + 420, 120, 30), "나가기")) {
			Debug.Log("In Button Exit");
        	scoreBufReset();
        	logout();
            exitGameHandler();
        }
        
        // Login button:
        if(GUI.Button(new Rect(xShift, yShift + 460, 260, 30), "자랑하기(push)")) {
            pushNotify("지바맨2 점수자랑");
        }
        
        // Login button:
        if(GUI.Button(new Rect(xShift, yShift + 500, 120, 30), "내점수보기")) {
            setMyScoreMode(true);
            setRankMode(false);
            getScore();
        }
       
        // Switch to registration view button:
        if(GUI.Button(new Rect(xShift + 140, yShift + 500, 120, 30), "최고점수보기")) {
            setMyScoreMode(false);
            setRankMode(false);
            getScore();
        }

        if(GUI.Button(new Rect(xShift + 140, yShift + 540, 120, 30), "내등수보기")) {
            setMyScoreMode(false);
            setRankMode(true);
            getRank();
        }

        // Enabling UI: 

        GUI.enabled = true;
    }

    public void setBlockUI(bool blockUI) {
        this.blockUI = blockUI;
    }
    
    public void setMyScoreMode(bool boolValue) {
        this.isMyScoreOnly = boolValue;
    }
    
    public void setRankMode(bool boolValue) {
    	this.isRankMode = boolValue;
    }
}