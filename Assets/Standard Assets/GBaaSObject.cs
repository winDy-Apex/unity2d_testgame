using UnityEngine;
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
// GBaaS SDK의 기능을 호출 할 수 있는 진입점을 제공하는 싱글톤 클래스
// 모든 GBaaS 기능은 이 싱글톤 클래스를 이용하여 사용할 수 있습니다.
// 예. GBaaSObject.Instance.API.GetScore(...생략...) 등
// GBaaS API 의 자세한 사용법은 GBaaS 서비스 사이트의 개발자 가이드를 참고하세요.
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

		public override void OnLogin(GBResult result) {
			Debug.Log("GBaaSAsyncHandler OnLogin");

			if(GBaaSObject._registrationId != "") {
				_outerClass.API.RegisterDevice(SystemInfo.deviceModel, SystemInfo.operatingSystem, "android", GBaaSObject._registrationId);
			}
		}
		
		public override void OnLoginWithFaceBook(GBResult result) {
			OnLogin(result);
		}
		
		public override void OnLoginWithoutID(GBResult result) {
			OnLogin(result);
		}

		public override void OnIsRegisteredDevice(bool result) {
			Debug.Log ("GBaaSAsyncHandler OnIsRegisteredDevice " + result.ToString());
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

	// for Javascript Dummy Init
	public void Init() {
		Init (null);
	}

	// GBaaS Api 를 초기화 한다.
	// 초기화를 제외한 대부분의 동작은
	// GBaaSObject.Instance.API.GetScore(...생략...) 의 형식으로
	// GBaaS API 를 직접 호출 하며
	// GBaaSObject에는 API 호출을 간소화하기 위한 일부 함수가 제공되어 있습니다.
	// 편의를 위해서 코드를 추가한 경우에는 SDK 업데이트시 수정내용이 손실 되지 않도록
	// 별도의 버전관리를 사용하시기를 추천드립니다.
	// GBaaS API 사용법은 개발자 가이드를 참고 부탁드립니다.
	public void Init(GBaaSApiHandler handler = null) {
		if(API == null) {
			ServicePointManager.ServerCertificateValidationCallback = Validator;
			API = new GBaaSApi(GBaaSUserObject.API_ENDPOINT);
			API.AddHandler(new GBaaSAsyncHandler(this));

			string[] senderIds = {GBaaSUserObject.GOOGLE_PROJECT_NUM_FOR_GCM};

#if UNITY_ANDROID
			if(GBaaSUserObject.GOOGLE_PROJECT_NUM_FOR_GCM.Length > 0) {
				// PUSH 알림 수신을 위한 GCM 초기화 부분
				Debug.Log ("[GBaaS-GCM] Try GCM Init");
				GCM.Initialize ();
				
				// Set callbacks
				GCM.SetErrorCallback ((string errorId) => {
					Debug.Log ("[GBaaS-GCM] Error!!! " + errorId);
				});
				
				// PUSH 알림 메시지 수신에 대한 처리를 추가하여야 하는 부분
				GCM.SetMessageCallback ((Dictionary<string, object> table) => {
					Debug.Log ("[GBaaS-GCM] Message!!! " + table.ToString());

					//string text = "Message: " + System.Environment.NewLine;
					//foreach (var key in  table.Keys) {
					//	text += key + "=" + table[key] + System.Environment.NewLine;
					//}
					//GCM.ShowToast(text);

					GCM.ShowToast (table["message"].ToString());
				});

				// PUSH 알림을 받을 단말기의 정보를 입력하는 부분
				GCM.SetRegisteredCallback ((string registrationId) => {
					Debug.Log ("[GBaaS-GCM] Registered!!! registrationId = " + registrationId + " / deviceModel = " + SystemInfo.deviceModel + " / operatingSystem = " + SystemInfo.operatingSystem);
					
					_registrationId = registrationId;
				});

				GCM.SetUnregisteredCallback ((string registrationId) => {
					Debug.Log ("[GBaaS-GCM] Unregistered!!! " + registrationId);
				});
				
				GCM.SetDeleteMessagesCallback ((int total) => {
					Debug.Log ("[GBaaS-GCM] DeleteMessages!!! " + total);
				});

				GCM.Register (senderIds);
			}

			if(GBaaSUserObject.XIAOMI_APPID.Length > 0) {
				// GBaaSAndroid Initialize with Lean and Xiaomi info.
				GBaaSAndroid.Instance.Init(GBaaSUserObject.LEAN_APPID, GBaaSUserObject.LEAN_APPKEY, 
				                           GBaaSUserObject.XIAOMI_APPID, GBaaSUserObject.XIAOMI_APPKEY);
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
	public GBResult CreateUser(string userID, string displayName, string password, string email) {
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
	public GBResult ReceiptSave(GBReceiptObject receipt) {
		return receipt.Save();
	}

//********** For AchievementService ********** //
	// 사용자가 dev.gbaas.io 를 통해서 생성한 업적 정보를 갱신하기 위해서
	// 필요한 정보를 입력하여 사용하는 부분
	// 아래의 코드는 샘플로서 유니티에서 업적 타입을 지정하여 요청하면
	// GBaaS에 생성된 업적 정보로 변환하여 갱신을 요청하도록 되어있습니다.
	public bool UpdateAchievement(int achievementType) {
		GBAchievementObject result = null;
		if(achievementType == 0) {
			Debug.Log ("Update Achievement UseBombMoreThanOnce");
			result = API.UpdateAchievement("87b9c7fa-bd8f-11e4-bb1e-d7881e792072", 1, true);
		} else if(achievementType == 1) {
			Debug.Log ("Update Achievement GetScore2000Over");
			result = API.UpdateAchievement("6cd6b1aa-bd8f-11e4-b9ca-215d8ebe7816", 1, true);
		}
		return (result != null);
	}
}
