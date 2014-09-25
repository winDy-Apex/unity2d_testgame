using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GBaaS.io;
using GBaaS.io.Objects;

using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

// **************************
// 사용자 정보로 변경하여야 하는 부분
public class UserDefines {
	//For Preview of GBaaS, USE ONLY FOR TEST
	public static string API_ENDPOINT = "https://api.gbaas.io/33e8b61a-3340-11e4-ab01-b99509431e86/2608ef70-3344-11e4-9ca9-15e8a7c9ff3a/"; //GBaaS Test Project
	public static string GOOGLE_PROJECT_NUM_FOR_GCM = "941440455383";

	//YOUR OWN API_ENDPOINT
	//public static string API_ENDPOINT = "ENTER YOUR GBAAS API ENDPOINT";
	//public static string GOOGLE_PROJECT_NUM_FOR_GCM = "ENTER YOUR OWN";
}
// **************************

// GBaaS Api 를 맵핑하여 게임에 특화된 파라미터를 미리 입력하여 편리하게 사용한다.
// dev.gbaas.io 에서 얻을 수 있는 Summary >> Access Information 의
// API Endpoint 값으로 Init()을 호출한 후 사용할 수 있다.
public class GBaaSObject : MonoBehaviour {

	static GBaaSObject instance;
	
	public static GBaaSObject Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(GBaaSObject)) as GBaaSObject;
				if (instance == null) {
					GameObject obj = new GameObject ();
					obj.hideFlags = HideFlags.HideAndDontSave;
					instance = (GBaaSObject)obj.AddComponent (typeof(GBaaSObject));
				}
			}
			return instance;
		}
	}

	// C# Doesn't Allow Multiple Inheritance.
	// So GBaaS Api Handler Implement as a Inner Class.
	// GBaaS SDK 를 Async 로 동작 시킬 경우
	// GBaaS 처리후 이벤트를 받을 핸들러의 원형(GBaaSApiHandler)
	// 핸들러의 해당 메시지에 처리 후 동작을 정의하여 사용하면 됩니다.
	// HTTP 의 특성상 처리 지연에 대비하여 되도록 비동기로 작업할 것을 권장합니다.
	class GBaaSAsyncHandler : GBaaSApiHandler {
		GBaaSObject _outerClass;

		public GBaaSAsyncHandler(GBaaSObject outerClass) {
			_outerClass = outerClass;
		}

		public override void OnGetAchievement(List<GBAchievementObject> result) {
			Debug.Log ("GBaaSAsyncHandler OnGetAchievement count = " + result.Count.ToString());
			_outerClass.asyncRuns["GetAchievementBuf"] = "OnGetAchievement";
			_outerClass.achievement = result;
			
			foreach (var item in _outerClass.achievement) {
				Debug.Log(item.achievementName);
				Debug.Log(item.currentStepCount);
				Debug.Log(item.earnedDescription);
				Debug.Log(item.incrementalCount);
				Debug.Log(item.isHidden);
				Debug.Log(item.isMoreThanOnce);
				Debug.Log(item.isUnlocked);
				Debug.Log(item.points);
				Debug.Log(item.preEarnedDescription);
				Debug.Log(item.uuid);
			}
		}
		
		public override void OnIsRegisteredDevice(bool result) {
			Debug.Log ("GBaaSAsyncHandler OnIsRegisteredDevice " + result.ToString());
			_outerClass.asyncRuns["IsRegisteredDevice"] = "OnIsRegisteredDevice";

			if(!result) {
				_outerClass.RegisterDevice(SystemInfo.deviceModel, SystemInfo.operatingSystem, "android", GBaaSObject._registrationId);
			}
		}
	}

	GBaaSApi _gBaaSApi = null;
	public static string		loginName 		= "";
	public static string		_registrationId	= "";
	public List<GBScoreObject> 	score 			= null;
	public List<GBAchievementObject> achievement = null;

	// Map views by name:
	public Hashtable asyncRuns = new Hashtable();

	public static bool Validator (object sender, X509Certificate certificate, X509Chain chain,
	                              SslPolicyErrors sslPolicyErrors) {
		return true;
	}

	// for Javascript
	public void Init() {
		Init (null);
	}

	// GBaaS Api 를 초기화 한다.
	public void Init(GBaaSApiHandler handler = null) {
		if(_gBaaSApi == null) {
			ServicePointManager.ServerCertificateValidationCallback = Validator;
			_gBaaSApi = new GBaaSApi(UserDefines.API_ENDPOINT); //GBaaSMan2

			//GBaaSAsyncHandler handler = new GBaaSAsyncHandler();
			_gBaaSApi.AddHandler(new GBaaSAsyncHandler(this));

			// 구글에서 받은 Project Number를 입력하는 부분 
			string[] senderIds = {UserDefines.GOOGLE_PROJECT_NUM_FOR_GCM};

#if UNITY_ANDROID
			// PUSH 알림 수신을 위한 GCM 초기화 부분
			GCM.Initialize ();
			
			// Set callbacks
			GCM.SetErrorCallback ((string errorId) => {
				Debug.Log ("Error!!! " + errorId);
			});
			
			// PUSH 알림 메시지 수신에 대한 처리를 추가하여야 하는 부분
			GCM.SetMessageCallback ((Dictionary<string, object> table) => {
				Debug.Log ("Message!!! " + table.ToString());

				//string text = "Message: " + System.Environment.NewLine;
				//foreach (var key in  table.Keys) {
				//	text += key + "=" + table[key] + System.Environment.NewLine;
				//}
				//GCM.ShowToast(text);

				GCM.ShowToast (table["message"].ToString());
			});

			// PUSH 알림을 받을 단말기의 정보를 입력하는 부분
			GCM.SetRegisteredCallback ((string registrationId) => {
				Debug.Log ("Registered!!! registrationId = " + registrationId);
				IsRegisteredDevice(SystemInfo.deviceModel, SystemInfo.operatingSystem, "android", registrationId);
			});

			GCM.SetUnregisteredCallback ((string registrationId) => {
				Debug.Log ("Unregistered!!! " + registrationId);
			});
			
			GCM.SetDeleteMessagesCallback ((int total) => {
				Debug.Log ("DeleteMessages!!! " + total);
			});

			GCM.Register (senderIds);
#endif
			// Just For Monitoring Status Save Test
			// 커스텀 오브젝트 호출 테스트를 위한 부분
			//GameMonitorStatusSave(100, 200, 300);
		}

		if(handler != null) {
			bool isAdded = _gBaaSApi.AddHandler(handler);
			Debug.Log ("GBaaSObject Init handled is Added : " + isAdded.ToString());
		}
	}

//********** For UserService ********** //
	/// GBaaS 사용자 서비스에 로그인 하는 방법은 아래의 몇가지 방법중 하나를 이용하면 된다.
	/// LoginWithFacebook, facebook SDK 를 통해서 얻은 Facebook Token 을 이용
	/// facebook 과 사용자 정보가 연동된다.
	///
	/// Login, CreateUser 를 통해서 생성한 GBaaS 계정을 이용한 로그인
	///
	/// LoginWithoutID, 특별한 사용자 생성없이 사용자에 대한 식별값으로 로그인을 대신함
	
	/// <summary>
	/// 페이스북 Token 을 이용하여 로그인 한다.
	/// 로그인시 자동으로 페이스북 정보와 연동하여 사용자 정보를
	/// GBaaS 의 dev.gbaas.io 의 User 항목에 생성한다.
	/// </summary>
	/// <returns><c>true</c>, if with facebook was logined, <c>false</c> otherwise.</returns>
	/// <param name="facebookToken">페이스북 Token 은 Facebook SDK를 통해서 구한다.</param>
	public bool LoginWithFacebook(string facebookToken) {
		return _gBaaSApi.LoginWithFaceBook(facebookToken);
	}

	/// <summary>
	/// 사용자 ID와 암호를 이용하여 GBaaS 에 로그인 한다.
	/// </summary>
	/// <param name="userName">사용자 ID</param>
	/// <param name="password">암호</param>
	public bool Login(string userName, string password) {
		return _gBaaSApi.Login(userName, password);
	}
	
	/// <summary>
	/// GBaaS 의 ID/Password 가 아니라
	/// 임의의 유니크 키를 이용하여 바로 사용자를 생성하고 로그인 한다.
	/// 같은 사용자에 대해서는 동일한 키를 이용하면 같은 사용자로 인식되어 로그인 된다.
	/// Device ID 나 UUID 또는 카카오톡 회원번호등 게임 환경에 따라 식별값을 사용하면 된다.
	/// 사용자의 정보 수정은 GBaaSApi 의 UpdateUser 를 통해서 가능하며
	/// 사용자의 이름만 수정할 경우 UpdateUserName 을 사용하여 간편하게 할 수 있다.
	/// 로그인 후 사용자 정보를 가져올 때는 GetUserInfo 를 호출한다.
	/// </summary>
	/// <returns><c>true</c>, if without I was logined, <c>false</c> otherwise.</returns>
	/// <param name="uniqueUserKey">Unique user key.</param>
	public bool LoginWithoutID(string uniqueUserKey) {
		Debug.Log ("LoginWithoutID uniqueUserKey : " + uniqueUserKey);
		return _gBaaSApi.LoginWithoutID(uniqueUserKey);
	}

	/// <summary>
	/// LoginWithoutID 와 쌍으로 사용한다.
	/// 임의로 로그인한 사용자에게 표시이름을 부여한다.
	/// </summary>
	/// <returns><c>true</c>, if user name was updated, <c>false</c> otherwise.</returns>
	/// <param name="userName">User name.</param>
	public bool UpdateUserName(string userName) {
		return _gBaaSApi.UpdateUserName(userName);
	}
	
	/// <summary>
	/// 로그인한 사용자의 사용자 정보를 가져온다.
	/// </summary>
	/// <returns>사용자 정보를 표시하는 오브젝트 namespace GBaaS.io.Objects
	/// namespace GBaaS.io.Objects 에 있습니다.
	///{
	/*
	{
		public string uuid { get; set; }
		public string name { get; set; }
		public string username { get; set; }
		public string password { get; set; }
		public string firstname { get; set; }
		public string lastname { get; set; }
		public string title { get; set; }
		public string homePage { get; set; }
		public string email { get; set; }
		public string bday { get; set; }
		public string picture { get; set; }
		public string tel { get; set; }
		public string url { get; set; }
		public int age { get; set; }
		public string gender { get; set; }
	}
	*/
	///}
	/// </returns>
	public GBUserObject GetUserInfo() {
		return _gBaaSApi.GetUserInfo();
	}

	/// 사용자 암호등 사용자 정보를 수정한다.
	/// </summary>
	/// <returns>수정된 사용자 정보에 대한 Json String, 수정 확인용도로만 사용한다.</returns>
	/// <param name="userModel">User model.</param>
	/// @code
	/// var result = gBaasobject.UpdateUser(new GBUserObject {
	/// 	username = un,
	/// 	password = PASSWORD,
	/// 	email = "test@test.com",
	/// 	age = 19,
	/// 	gender = "Female"
	/// });
	/// @endcode
	public string UpdateUser(GBUserObject userModel) {
		return _gBaaSApi.UpdateUser (userModel);
	}

	/// <summary>
	/// GBaaS 서비스에서 사용자를 로그아웃 시킵니다.
	/// </summary>
	public void Logout() {
		_gBaaSApi.Logout();
	}

	/// <summary>
	/// 사용자 정보 생성
	/// 기본적인 사용자 정보를 생성한다.
	/// 사용자 정보는 생성후 UpdateUser 를 통해서 수정 가능하다.
	/// </summary>
	/// <returns>생성된 사용자 정보에 대한 UUID</returns>
	/// <param name="userName">사용자 ID</param>
	/// <param name="password">암호</param>
	/// <param name="email">email</param>
	public string CreateUser(string userName, string password, string email) {
		Debug.Log("CreateUser");
		GBUserObject userModel = new GBUserObject {
			username = userName,
			password = password,
			email = email
		};

		return _gBaaSApi.CreateUser(userModel);
	}
	
	//********** For ScoreService ********** //
	// GetScore 를 호출하면 항상 Score List 를 새로 구한다.
	// 파라미터
	// stage 기록된 게임의 판수
	// unit 기록된 점수 유형, 같은 판에 여러 점수 유형을 기록할 수 있다. 득점, 실점, 킬수 등등
	// limit 한번에 가져올 정보의 최대 숫자 한번에 10개씩 또는 100개씩 등으로 지정할 수 있다.
	// isLog true 로 설정하면 자신의 정보만 가져온다.
	// isMore true 로 설정하면 limit 로 설정한 다음 페이지에 해당하는 정보를 가져온다. (이때 다른 파라미터는 전과 동일하게 보내야한다.)
	//
	// 상위 등수의 사용자 정보를 가져올때 사용한다.
	//
	public List<GBScoreObject> GetScore(string stage, string unit, int limit, bool isLog, bool isMore) {

		if(isLog) {
			this.score = _gBaaSApi.GetScoreLogMore(stage, unit, limit, isMore);
		} else {
			this.score = _gBaaSApi.GetScoreMore(stage, unit, limit, isMore);
		}

		return this.score;
	}

	// rank 로 지정한 주위의 값 범위는 range 에 해당하는 등수의 값을 가져온다.
	// 예) rank = 100, range = 10 으로 지정하면 90 ~ 110 의 21개 등수에 대한 값을 가져온다.
	// period 값을 지정하면 일간, 주간, 월간으로 점수를 가져오는 대상 범위를 지정할 수 있다.
	// 현재 코드는 Period.Weekly 로 되어 있다. (수정 가능)
	// rank = 0 으로 지정하면 자기 등수 주위의 값을 가져온다. 가져오는 ScoreObject 에는
	// rank 정보가 같이 들어있으므로 해당 등수의 값을 출력하면 된다.
	// 단순히 자기 등수만 알고 싶을 때는 rank=0, range=1 로 호출하면 자기 등수와 점수 정보만
	// 구할 수 있다.
	public List<GBScoreObject> GetRank(string stage, string unit, ScoreOrder scoreOrder, Period period, int rank, int range) {

		this.score = _gBaaSApi.GetRank(stage, unit, scoreOrder, period, 0, 10);

		return this.score;
	}

	// 사용자의 점수를 새로 등록한다.
	/* Code Sample
	aClient.AddScore(new GBScoreObject {
		stage = "1",
		score = previousScore,
		unit = "point"
	});
	*/
	public bool AddScore(GBScoreObject score) {
		return _gBaaSApi.AddScore(score);
	}
	
//********** For PushService ********** //
	/// <summary>
	/// Push Notification 을 위해서 디바이스를 등록한다.
	/// </summary>
	/// <returns><c>true</c>, if device was registered, <c>false</c> otherwise.</returns>
	/// <param name="deviceModel">Device model.</param>
	/// <param name="deviceOSVersion">Device OS version.</param>
	/// <param name="devicePlatform">Device platform.</param>
	/// <param name="registeration_id">Registeration_id.</param>
	public bool RegisterDevice(
		string deviceModel, string deviceOSVersion, string devicePlatform, string registeration_id) {
		return _gBaaSApi.RegisterDevice(deviceModel, deviceOSVersion, devicePlatform, registeration_id);
	}

	public bool IsRegisteredDevice(
		string deviceModel, string deviceOSVersion, string devicePlatform, string registeration_id) {

		bool result = false;
		_registrationId = registeration_id;
		if (asyncRuns["IsRegisteredDevice"] == null || asyncRuns["IsRegisteredDevice"].ToString().CompareTo("Run") != 0) {
			asyncRuns["IsRegisteredDevice"] = "Run";
			result = _gBaaSApi.IsRegisteredDevice(deviceModel, deviceOSVersion, devicePlatform, registeration_id);
		}

		return result;
	}

	public bool PushNotify(string msg) {
		return _gBaaSApi.SendMessage(msg, "", "", "", "", PushSendType.alldevices, PushScheduleType.now);
	}

//********** For GameData Service ********** //
	// Key - Value 형태로 서비스 하며
	// 사용자의 게임 데이터를 저장하는 용도로 사용한다.
	// Value 는 저장 전에 암호화 되어 전송되며
	// Load 될 때 자동으로 복호화 된다.
	public bool GameDataSave(string key, string value) {
		return _gBaaSApi.GameDataSave(key, value);
	}
	
	public string GameDataLoad(string key) {
		return _gBaaSApi.GameDataLoad(key);
	}

	// GBaaS 서버에 결제 정보를 저장한다.
	// 아래 코드 샘플을 참고로 사용한다.
	/// @code
	/// var result = gBaasobject.ReceiptSave(new GBReceiptObject {
	/// 	receiptCode = "12312313131213231123131231312313132132",
	/// 	receiptType = "1",
	/// 	userDID = "asjldjflajfdldsjlfjdlsajlfdsaFASFASF23234234243",
	/// 	dayToUse = "2014-04-16 15:28:32.0"
	/// });
	/// @endcode
	public bool ReceiptSave(GBReceiptObject receipt) {
		return receipt.Save();
	}

	// Custom Object를 정의하고 생성해서 저장한다.
	// GBObject 로 부터 상속 받아 임의의 데이터 클래스를 만든다.
	// Custom Object 는 GBaaS 의 dev.gbaas.io 에서
	// 저장을 확인 할 수 있다.
	class CustomStatusObject : GBObject {
		public int mineral { get; set; }
		public int gold { get; set; }
		public int heart { get; set; }
	};

	// 생성한 커스텀 오브젝트에 값을 입력하고 GBaaS 에 저장하기 위해 만든 함수
	// 이와 같이 임의의 데이터 오브젝트를 구성하여 GBaaS 에 저장할 수 있다.
	public bool GameMonitorStatusSave(int mineral, int gold, int heart) {
		CustomStatusObject status = new CustomStatusObject {
			mineral = mineral,
			gold = gold,
			heart = heart
		};

		return status.Save();
	}

	/*
	// for feelApp Presence Status
	// GBUniqueObject exist always as a single (unique) object in GBaaS for a single user.
	// It can be store a single player's game status.
	// But this object is just conception not yet implemented.
	// If feelapp agree this conception, we will make this immediately and release again as soon as possible. (maybe tomorrow)
	// ^^^ GBaaS's Role
	// ==========================================
	// vvv FeelApp's Role
	// But after all any case. We(GBaaS) can't handle user status data and deal reward itself.
	// It's responsibility is up to game logic.
	class GameStatusObject : GBUniqueObject {
		public string 	lastLoginDate { get; set; }
		public long 	totalPresence { get; set; }
		public int		presenceOfThisMonth { get; set; }
		public int		updatePeriod { get; set; } // day, week, month
	};
	*/

//********** For AchievementService ********** //
	/// <summary>
	/// Gets the achievement.
	/// </summary>
	/// <returns>The achievement.</returns>
	/// <param name="locale">Locale.</param>
	/// <param name="limit">Limit.</param>
	/// <param name="cursor">Cursor.</param>
	public List<GBAchievementObject> GetAchievement(string locale, int limit, string cursor) {
		this.achievement = _gBaaSApi.GetAchievement(locale, limit, cursor);
		return this.achievement;
	}

	/// <summary>
	/// Gets the achievement buffer.
	/// </summary>
	/// <returns>The achievement buffer.</returns>
	public List<GBAchievementObject> GetAchievementBuf() {
		if (this.achievement == null) { // && (asyncRuns["GetAchievementBuf"] == null || asyncRuns["GetAchievementBuf"].ToString().CompareTo("Run") != 0)) {
			asyncRuns["GetAchievementBuf"] = "Run";
			GetAchievement ("ko-KR", 0, "");
		}

		return this.achievement;
	} 

	public void AchievementBufReset() {
		this.achievement = null;
	}

	/// <summary>
	/// Adds the achievement.
	/// </summary>
	/// <param name="achievementType">Achievement type.</param>
	public bool UpdateAchievement(int achievementType) {
		GBAchievementObject result = null;
		if(achievementType == 0) {
			result = _gBaaSApi.UpdateAchievement("UseBombMoreThanOnce",
				1,
				true
			);
		} else if(achievementType == 1) {
			result = _gBaaSApi.UpdateAchievement("GetScore2000Over",
			                                          1,
			                                          true
			                                          );
		}
		return (result != null);
	}
}
