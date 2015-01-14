using UnityEngine;
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
    
    public bool 	error 			= false;
    public string 	errorMessage 	= "";
    
	private bool 	blockUI 		= false;

	private List<GBScoreObject> _scores = null;
 
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
 	
	public override void OnGetScore(List<GBScoreObject> result) {
		if(result[0].isSuccess) {
			Debug.Log("ScoreView OnGetScore result : " + _scores.Count.ToString());
			_scores = result;
		} else {
			Debug.Log("ScoreView OnGetScore Fail returnCode(" + result[0].returnCode + ") reason(" + result[0].reason + ")");
		}
	}

	public override void OnGetScoreLog(List<GBScoreObject> result) {
		_scores = result;
		Debug.Log("ScoreView OnGetScoreLog result : " + _scores.Count.ToString());
	}
	
	public override void OnGetRank(List<GBScoreObject> result) {
		_scores = result;
		Debug.Log("ScoreView OnGetRank result : " + _scores.Count.ToString());
	}
	 	
 	public void pushNotify(string msg) {
		GBaaSObject.Instance.Init(this);
		GBaaSObject.Instance.PushNotify(msg);
 	}

 	public void logout() {
		GBaaSObject.Instance.Init(this);
		GBaaSObject.Instance.API.Logout();
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
        
        if(_scores == null) {
			Debug.Log("In render Score == null");
			_scores = new List<GBScoreObject>();
			GBaaSObject.Instance.Init(this);
			GBaaSObject.Instance.API.GetScore("1", "point", 10);
			return;
		}

        // Message label:
        if(error) {
            GUI.Label(new Rect(0, yShift + 70, screenWidth, 30), errorMessage, header2ErrorStyle);
        } else {     
			bool isRankMode = false;
			if(_scores.Count > 0) {
				isRankMode = (_scores[0].rank > 0);
			}

			float yPos = yShift + 40;
			GUI.Label(new Rect(xShift - 320, yPos, 150, 30), "rank", header2Style);
			GUI.Label(new Rect(xShift - 170, yPos, 150, 30), "diaplayName", header2Style);
			GUI.Label(new Rect(xShift - 20, yPos, 150, 30), "username(ID)", header2Style);
			GUI.Label(new Rect(xShift + 130, yPos, 150, 30), "stage", header2Style);
			GUI.Label(new Rect(xShift + 280, yPos, 150, 30), "score", header2Style);
			GUI.Label(new Rect(xShift + 430, yPos, 150, 30), "unit", header2Style);

			for(var i=0; i<_scores.Count; i++) {
				yPos = yShift + 70 + (30 * i);
				if(isRankMode) GUI.Label(new Rect(xShift - 320, yPos, 150, 30), _scores[i].rank.ToString(), header2Style);
				GUI.Label(new Rect(xShift - 170, yPos, 150, 30), _scores[i].displayName, header2Style);
				if(_scores[i].username.Length > 10) {
					GUI.Label(new Rect(xShift - 20, yPos, 150, 30), _scores[i].username.Substring(0, 10) + "...", header2Style);
				} else {
					GUI.Label(new Rect(xShift - 20, yPos, 150, 30), _scores[i].username.Substring(0, _scores[i].username.Length), header2Style);
				}
				GUI.Label(new Rect(xShift + 130, yPos, 150, 30), _scores[i].stage, header2Style);
				GUI.Label(new Rect(xShift + 280, yPos, 150, 30), _scores[i].score.ToString(), header2Style);
				GUI.Label(new Rect(xShift + 430, yPos, 150, 30), _scores[i].unit, header2Style);
	        }
        }
        
        if(GUI.Button(new Rect(xShift, yShift + 420, 120, 30), "게임하기")) {
            enterGameHandler();
        }
       
        if(GUI.Button(new Rect(xShift + 140, yShift + 420, 120, 30), "나가기")) {
			Debug.Log("In Button Exit");
        	logout();
            exitGameHandler();
        }
        
        if(GUI.Button(new Rect(xShift, yShift + 460, 260, 30), "자랑하기(push)")) {
            pushNotify("지바맨2 점수자랑");
        }
        
		if(GUI.Button(new Rect(xShift, yShift + 500, 120, 30), "자기점수기록")) {
			GBaaSObject.Instance.API.GetScoreLog("1", "point", 10);
		}
		
		if(GUI.Button(new Rect(xShift + 140, yShift + 500, 120, 30), "상위등수보기")) {
			GBaaSObject.Instance.API.GetScore("1", "point", 10);
        }

		if(GUI.Button(new Rect(xShift, yShift + 540, 120, 30), "자기등수")) {
			GBaaSObject.Instance.API.GetRank("", "", ScoreOrder.DESC, Period.Monthly);
		}

		if(GUI.Button(new Rect(xShift + 140, yShift + 540, 120, 30), "자기등수주변")) {
			GBaaSObject.Instance.API.GetRank("", "", ScoreOrder.DESC, Period.Monthly, 0, 10);
		}
		
        // Enabling UI: 

        GUI.enabled = true;
    }

    public void setBlockUI(bool blockUI) {
        this.blockUI = blockUI;
    }
}
