﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GBaaS.io;
using GBaaS.io.Objects;

using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

// GBaaS Api 를 맵핑하여 게임에 특화된 파라미터를 미리 입력하여 편리하게 사용한다.
// dev.gbaas.io 에서 얻을 수 있는 Summary >> Access Information 의
// API Endpoint 값으로 Init()을 호출한 후 사용할 수 있다.

public class GBaaSObject : MonoBehaviour {
	
	GBaaSApi _gBaaSApi = null;
	public static string loginName = "";
	public List<GBScoreObject> score = null;
	public List<GBAchievementObject> achievement = null;

	public static bool Validator (object sender, X509Certificate certificate, X509Chain chain,
	                              SslPolicyErrors sslPolicyErrors) {
		return true;
	}

	// GBaaS Api 를 초기화 한다.
	public void Init() {
		if(_gBaaSApi == null) {
			ServicePointManager.ServerCertificateValidationCallback = Validator;
			_gBaaSApi = new GBaaSApi("https://api.gbaas.io/46429fea-a5b2-11e3-a2b5-4526b3bfa68e/4ddcbe30-a5b6-11e3-bbef-779adace1db9/");

			// PUSH 알림 수신을 위한 GCM 초기화 부분
			GCM.Initialize ();
			
			// Set callbacks
			GCM.SetErrorCallback ((string errorId) => {
				Debug.Log ("Error!!! " + errorId);
			});
			
			// PUSH 알림 메시지 수신에 대한 처리를 추가하여야 하는 부분
			GCM.SetMessageCallback ((Dictionary<string, object> table) => {
				Debug.Log ("Message!!! " + table.ToString ());
			});
			
			// PUSH 알림을 받을 단말기의 정보를 입력하는 부분
			GCM.SetRegisteredCallback ((string registrationId) => {
				Debug.Log ("Registered!!! registrationId" + registrationId);
				RegisterDevice(SystemInfo.deviceModel, SystemInfo.operatingSystem, "android", registrationId);
			});
			
			GCM.SetUnregisteredCallback ((string registrationId) => {
				Debug.Log ("Unregistered!!! " + registrationId);
			});
			
			GCM.SetDeleteMessagesCallback ((int total) => {
				Debug.Log ("DeleteMessages!!! " + total);
			});

			// 구글에서 받은 Project Number를 입력하는 부분 
			string[] senderIds = {"941440455383"};
			GCM.Register (senderIds);
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
		return _gBaaSApi.LoginWithoutID(uniqueUserKey);
	}
	 /// <summary>
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
		public string Email { get; set; }
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
	/// 	Email = "test@test.com",
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
	/// <param name="email">Email</param>
	public string CreateUser(string userName, string password, string email) {
		GBUserObject userModel = new GBUserObject {
			username = userName,
			password = password,
			Email = email
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
	public List<GBScoreObject> GetRank(string stage, string unit, string scoreOrder, string period, int rank, int range) {

		this.score = _gBaaSApi.GetRank(stage, unit, ScoreOrder.DESC, Period.Weekly, 0, 10);

		return this.score;
	}

	// 이미 Score List 가 있을때는 새로 구하지 않는다.
	// UI 에서 Score 정보를 반복 호출 할때, 정보 갱신이 필요 없는 경우 사용한다.
	public List<GBScoreObject> GetScoreBuf() {
		if (this.score == null)
			return GetScore ("1", "point", 10, false, false);
		return this.score;
	} 

	// 저장된 Score List 를 삭제한다.
	// 다음의 GetScoreBuf 는 Score List 를 새로 구하게 된다.
	// Score List 를 얻기 위한 함수의 권장 호출 순서는
	// GetScore -> GetScoreBuf 또는 ScoreBufReset -> GetScoreBuf 이다.
	public void ScoreBufReset() {
		this.score = null;
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

	public void PushNotify(string msg) {
		var result = _gBaaSApi.SendMessage(msg, "", "", "", "", PushSendType.alldevices, PushScheduleType.now);
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
		if (this.achievement == null) {
			GetAchievement ("ko-KR", 0, "");
			foreach (var item in this.achievement) {
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
			return this.achievement;
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
	public void UpdateAchievement(int achievementType) {
		if(achievementType == 0) {
			var results = _gBaaSApi.UpdateAchievement("UseBombMoreThanOnce",
				1,
				true
			);
		} else if(achievementType == 1) {
			var results = _gBaaSApi.UpdateAchievement("GetScore2000Over",
			                                          1,
			                                          true
			                                          );
		}
	}
}
