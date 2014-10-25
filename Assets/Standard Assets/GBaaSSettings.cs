using UnityEngine;
using System.IO;
using System.Collections;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
#endif

public class GBaaSSettings : ScriptableObject
{
	
	const string gbaasSettingsAssetName = "GBaaSSettings";
	const string gbaasSettingsPath = "GBaaS/Resources";
	const string gbaasettingsAssetExtension = ".asset";
	
	private static GBaaSSettings instance;
	
	static GBaaSSettings Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Resources.Load(gbaasSettingsAssetName) as GBaaSSettings;
				if (instance == null)
				{
					// If not found, autocreate the asset object.
					instance = CreateInstance<GBaaSSettings>();
					#if UNITY_EDITOR
					string properPath = Path.Combine(Application.dataPath, gbaasSettingsPath);
					if (!Directory.Exists(properPath))
					{
						AssetDatabase.CreateFolder("Assets/GBaaS", "Resources");
					}
					
					string fullPath = Path.Combine(Path.Combine("Assets", gbaasSettingsPath),
					                               gbaasSettingsAssetName + gbaasettingsAssetExtension
					                               );
					AssetDatabase.CreateAsset(instance, fullPath);
					#endif
				}
			}
			return instance;
		}
	}
	
	#if UNITY_EDITOR
	[MenuItem("GBaaS/Guide")]
	public static void ToGBaaSGuide()
	{
		string url = "http://gbaas.io/DEVELOPER/developer_list.gb?depth_1=13";
		Application.OpenURL(url);
	}
	
	[MenuItem("GBaaS/Developers Page")]
	public static void ToDevPage()
	{
		string url = "http://dev.gbaas.io/";
		Application.OpenURL(url);
	}
	
	[MenuItem("GBaaS/PUSH Configuration to GCM")]
	public static void PUSHConfigurationToGCM()
	{
		ProcessStartInfo psi = new ProcessStartInfo(); 
		//psi.FileName = Application.dataPath+"/test.sh";
		psi.FileName = "cp";
		psi.UseShellExecute = false; 
		psi.RedirectStandardOutput = true;
		psi.Arguments = "./Assets/Plugins/Android/AndroidManifest_GCM.xml ./Assets/Plugins/Android/AndroidManifest.xml";
		Process p = Process.Start(psi); 
		string strOutput = p.StandardOutput.ReadToEnd(); 
		p.WaitForExit(); 
		UnityEngine.Debug.Log("Set Push Configuration to GCM : " + strOutput);
	}

	[MenuItem("GBaaS/PUSH Configuration to CHINA")]
	public static void PUSHConfigurationToCHINA()
	{
		ProcessStartInfo psi = new ProcessStartInfo(); 
		//psi.FileName = Application.dataPath+"/test.sh";
		psi.FileName = "cp";
		psi.UseShellExecute = false; 
		psi.RedirectStandardOutput = true;
		psi.Arguments = "./Assets/Plugins/Android/AndroidManifest_CHINA.xml ./Assets/Plugins/Android/AndroidManifest.xml";
		Process p = Process.Start(psi); 
		string strOutput = p.StandardOutput.ReadToEnd(); 
		p.WaitForExit(); 
		UnityEngine.Debug.Log("Set Push Configuration to CHINA : " + strOutput);
	}
	#endif
	
	#region App Settings
	
	[SerializeField]
	private int selectedAppIndex = 0;
	[SerializeField]
	private string[] appIds = new[] { "0" };
	[SerializeField]
	private string[] appLabels = new[] { "App Name" };
	[SerializeField]
	private bool cookie = true;
	[SerializeField]
	private bool logging = true;
	[SerializeField]
	private bool status = true;
	[SerializeField]
	private bool xfbml = false;
	[SerializeField]
	private bool frictionlessRequests = true;
	[SerializeField]
	private string iosURLSuffix = "";
	
	public void SetAppIndex(int index)
	{
		if (selectedAppIndex != index)
		{
			selectedAppIndex = index;
			DirtyEditor();
		}
	}
	
	public int SelectedAppIndex
	{
		get { return selectedAppIndex; }
	}
	
	public void SetAppId(int index, string value)
	{
		if (appIds[index] != value)
		{
			appIds[index] = value;
			DirtyEditor();
		}
	}
	
	public string[] AppIds
	{
		get { return appIds; }
		set
		{
			if (appIds != value)
			{
				appIds = value;
				DirtyEditor();
			}
		}
	}
	
	public void SetAppLabel(int index, string value)
	{
		if (appLabels[index] != value)
		{
			AppLabels[index] = value;
			DirtyEditor();
		}
	}
	
	public string[] AppLabels
	{
		get { return appLabels; }
		set
		{
			if (appLabels != value)
			{
				appLabels = value;
				DirtyEditor();
			}
		}
	}
	
	public static string AppId
	{
		get
		{
			return Instance.AppIds[Instance.SelectedAppIndex];
		}
	}
	
	public static bool IsValidAppId
	{
		get
		{
			return GBaaSSettings.AppId != null 
				&& GBaaSSettings.AppId.Length > 0 
					&& !GBaaSSettings.AppId.Equals("0");
		}
	}
	
	public static bool Cookie
	{
		get { return Instance.cookie; }
		set
		{
			if (Instance.cookie != value)
			{
				Instance.cookie = value;
				DirtyEditor();
			}
		}
	}
	
	public static bool Logging
	{
		get { return Instance.logging; }
		set
		{
			if (Instance.logging != value)
			{
				Instance.logging = value;
				DirtyEditor();
			}
		}
	}
	
	public static bool Status
	{
		get { return Instance.status; }
		set
		{
			if (Instance.status != value)
			{
				Instance.status = value;
				DirtyEditor();
			}
		}
	}
	
	public static bool Xfbml
	{
		get { return Instance.xfbml; }
		set
		{
			if (Instance.xfbml != value)
			{
				Instance.xfbml = value;
				DirtyEditor();
			}
		}
	}
	
	public static string IosURLSuffix
	{
		get { return Instance.iosURLSuffix; }
		set 
		{
			if (Instance.iosURLSuffix != value)
			{
				Instance.iosURLSuffix = value;
				DirtyEditor ();
			}
		}
	}
	
	public static string ChannelUrl
	{
		get { return "/channel.html"; }
	}
	
	public static bool FrictionlessRequests
	{
		get { return Instance.frictionlessRequests; }
		set
		{
			if (Instance.frictionlessRequests != value)
			{
				Instance.frictionlessRequests = value;
				DirtyEditor();
			}
		}
	}
	
	#if UNITY_EDITOR
	
	private string testGBaaSId = "";
	private string testAccessToken = "";
	
	public static string TestGBaaSId
	{
		get { return Instance.testGBaaSId; }
		set
		{
			if (Instance.testGBaaSId != value)
			{
				Instance.testGBaaSId = value;
				DirtyEditor();
			}
		}
	}
	
	public static string TestAccessToken
	{
		get { return Instance.testAccessToken; }
		set
		{
			if (Instance.testAccessToken != value)
			{
				Instance.testAccessToken = value;
				DirtyEditor();
			}
		}
	}
	#endif
	
	private static void DirtyEditor()
	{
		#if UNITY_EDITOR
		EditorUtility.SetDirty(Instance);
		#endif
	}
	
	#endregion
}