//#if UNITY_ANDROID

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IAP_Xiaomi : MonoBehaviour {

	static IAP_Xiaomi instance;
	
	public static IAP_Xiaomi Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(IAP_Xiaomi)) as IAP_Xiaomi;
				if (instance == null) {
					GameObject obj = new GameObject ();
					obj.hideFlags = HideFlags.HideAndDontSave;
					instance = (IAP_Xiaomi)obj.AddComponent (typeof(IAP_Xiaomi));
				}
			}
			return instance;
		}
	}

	private AndroidJavaObject	_unityActivity	= null;

	public void Init(string appID, string appKey) {
		Debug.Log ("IAP_Xiaomi Init");

		using (var pluginClass = new AndroidJavaClass("net.apexplatform.gbaassample.CustomUnityPlayerActivity")) {
			if(pluginClass != null) {
				_unityActivity = pluginClass.CallStatic<AndroidJavaObject>("getInstance");
				
				Debug.Log ("try Get CustomUnityPlayerActivity instance" + _unityActivity.ToString());

				_unityActivity.Call("SetMiAppInfo", new object[] {appID, appKey}); 
			}
		}
	}

	public bool Payment(string paymentCode) {
		if(_unityActivity == null) {
			Debug.Log ("IAP_Xiaomi Payment You Have to Init First.");
			return false;
		}

		Debug.Log ("IAP_Xiaomi Payment Try");
		_unityActivity.Call("MiPay", paymentCode);

		return true;
	}
}
//#endif