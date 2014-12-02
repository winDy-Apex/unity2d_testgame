﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GBaaS.io;
using GBaaS.io.Objects;

using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

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
			//_outerClass.achievement = result;
			
			foreach (var item in result) {
				Debug.Log("achievementName:" + item.achievementName);
				Debug.Log("currentStepCount:" + item.currentStepCount);
				Debug.Log("earnedDescription:" + item.earnedDescription);
				Debug.Log("incrementalCount:" + item.incrementalCount);
				Debug.Log("isHidden:" + item.isHidden);
				Debug.Log("isMoreThanOnce:" + item.isMoreThanOnce);
				Debug.Log("isUnLocked:" + item.isUnLocked);
				Debug.Log("points:" + item.points);
				Debug.Log("preEarnedDescription:" + item.preEarnedDescription);
				Debug.Log("uuid:" + item.uuid);
			}
		}

		public override void OnLogin(bool result) {
			Debug.Log("GBaaSAsyncHandler OnLogin");

			if(GBaaSObject._registrationId != "") {
				_outerClass.API.RegisterDevice(SystemInfo.deviceModel, SystemInfo.operatingSystem, "android", GBaaSObject._registrationId);
			}
		}
		
		public override void OnLoginWithFaceBook(bool result) {
			OnLogin(result);
		}
		
		public override void OnLoginWithoutID(bool result) {
			OnLogin(result);
		}

		public override void OnIsRegisteredDevice(bool result) {
			Debug.Log ("GBaaSAsyncHandler OnIsRegisteredDevice " + result.ToString());

			if(!result) {
				//_outerClass.RegisterDevice(SystemInfo.deviceModel, SystemInfo.operatingSystem, "android", GBaaSObject._registrationId);
			}
		}
		
		public override void OnFileUpload(bool result) {
			Debug.Log ("GBaaSAsyncHandler OnFileUpload " + result.ToString());
		}

		public override void OnFileDownload(bool result) {
			Debug.Log ("GBaaSAsyncHandler OnFileDownload " + result.ToString());
		}

		public override void OnGetFileList(List<GBAsset> result) {
			Debug.Log ("GBaaSAsyncHandler OnGetFileList " + result.Count.ToString());
		}
	}

	public GBaaSApi API = null;
	public static string loginName = "";
	public static string _registrationId = "";

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
		if(API == null) {
			ServicePointManager.ServerCertificateValidationCallback = Validator;
			API = new GBaaSApi(GBaaSUserObject.API_ENDPOINT); //GBaaSMan2

			API.AddHandler(new GBaaSAsyncHandler(this));

			// 구글에서 받은 Project Number를 입력하는 부분 
			string[] senderIds = {GBaaSUserObject.GOOGLE_PROJECT_NUM_FOR_GCM};

#if UNITY_ANDROID
			if(GBaaSUserObject.GOOGLE_PROJECT_NUM_FOR_GCM.Length > 0) {
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
					Debug.Log ("[GBaaS] Registered!!! registrationId = " + registrationId + " / deviceModel = " + SystemInfo.deviceModel + " / operatingSystem = " + SystemInfo.operatingSystem);
					
					_registrationId = registrationId;
				});

				GCM.SetUnregisteredCallback ((string registrationId) => {
					Debug.Log ("Unregistered!!! " + registrationId);
				});
				
				GCM.SetDeleteMessagesCallback ((int total) => {
					Debug.Log ("DeleteMessages!!! " + total);
				});

				GCM.Register (senderIds);
			}
#endif
		}

		bool isAdded = API.AddHandler(handler);
		Debug.Log ("GBaaSObject Init handled is Added : " + isAdded.ToString());
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
	public string CreateUser(string userID, string displayName, string password, string email) {
		Debug.Log("CreateUser");
		GBUserObject userModel = new GBUserObject {
			username = userID,
			name = displayName,
			password = password,
			email = email
		};

		return API.CreateUser(userModel);
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
		return API.RegisterDevice(deviceModel, deviceOSVersion, devicePlatform, registeration_id);
	}

	public bool IsRegisteredDevice(
		string deviceModel, string deviceOSVersion, string devicePlatform, string registeration_id) {

		_registrationId = registeration_id;

		return API.IsRegisteredDevice(deviceModel, deviceOSVersion, devicePlatform, registeration_id);
	}

	public bool PushNotify(string msg) {
		return API.SendMessage(msg, "", "", "", "", PushSendType.alldevices, PushScheduleType.now);
	}

//********** For GameData Service ********** //
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

//********** For AchievementService ********** //
	/// <summary>
	/// Adds the achievement.
	/// </summary>
	/// <param name="achievementType">Achievement type.</param>
	public bool UpdateAchievement(int achievementType) {
		GBAchievementObject result = null;
		if(achievementType == 0) {
			Debug.Log ("Update Achievement UseBombMoreThanOnce");
			result = API.UpdateAchievement("6fc2135a-54f8-11e4-ba58-3bba8e7daf56", 1, true);
		} else if(achievementType == 1) {
			Debug.Log ("Update Achievement GetScore2000Over");
			result = API.UpdateAchievement("d0b24e5a-54f3-11e4-b688-3199c630a20e", 1, true);
		}
		return (result != null);
	}
}
