//#if UNITY_ANDROID

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MiPayResult : int {
	NO_TRY = 1,
	MI_XIAOMI_PAYMENT_SUCCESS = 0,
	MI_XIAOMI_PAYMENT_ERROR_LOGIN_FAIL = -102,
	MI_XIAOMI_PAYMENT_ERROR_CANCEL = -12,
	MI_XIAOMI_PAYMENT_ERROR_ACTION_EXECUTED = -18006
}

public class GBaaSAndroid : MonoBehaviour {

	private static GBaaSAndroid instance;
	private static string _LOGTAG = "GBaaSAndroid";
	
	public static GBaaSAndroid Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(GBaaSAndroid)) as GBaaSAndroid;
				if (instance == null) {
					GameObject obj = new GameObject ();
					obj.hideFlags = HideFlags.HideAndDontSave;
					instance = (GBaaSAndroid)obj.AddComponent (typeof(GBaaSAndroid));
				}
			}
			return instance;
		}
	}

	private AndroidJavaObject _gbaasAndroid	= null;

	public void Init(string leanAppID, string leanAppKey, string xiaomiAppID, string xiaomiAppKey) {
		Debug.Log (_LOGTAG + " Init");

		using (var pluginClass = new AndroidJavaClass("io.gbaas.unityandroid.GBaaSUnityAndroid")) {
			if(pluginClass != null) {
				
				Debug.Log (_LOGTAG + " Setup Third Party Infos");


				_gbaasAndroid = pluginClass.CallStatic<AndroidJavaObject>("getInstance");

				Debug.Log (_LOGTAG + " try Get GBaaSUnityAndroid instance" + _gbaasAndroid.ToString());

				//_gbaasAndroid.Call("TestCall");

				using (var actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
					AndroidJavaObject playerActivityContext = actClass.GetStatic<AndroidJavaObject>("currentActivity");

					_gbaasAndroid.Call("SetMainContext", playerActivityContext);
					_gbaasAndroid.Call("SetLeanCloudInfo", new object[] {leanAppID, leanAppKey}); 
					_gbaasAndroid.Call("SetMiAppInfo", new object[] {xiaomiAppID, xiaomiAppKey}); 
				}
			} else {
				Debug.Log (_LOGTAG + " Fail Found Android Class!!");
			}
		}
	}

	public bool Payment(string paymentCode) {
		if(_gbaasAndroid == null) {
			Debug.Log (_LOGTAG + " GBaaSAndroid Payment You Have to Init First.");
			return false;
		}

		Debug.Log (_LOGTAG + " GBaaSAndroid Payment Try");
		MiPayResult payResult = (MiPayResult)_gbaasAndroid.Call<int>("MiPay", paymentCode);
		Debug.Log (_LOGTAG + " GBaaSAndroid Payment Result " + payResult.ToString());

		return true;
	}
}
//#endif