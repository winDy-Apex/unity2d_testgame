using UnityEngine;
using GBaaS.io;
using GBaaS.io.Objects;
using GBaaS.io.Services;
using System.Collections;

public class Pauser : MonoBehaviour {
	private bool paused = false;
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.P))
		{
			paused = !paused;
		}
		
		if(Input.GetKeyUp(KeyCode.L))
		{
			Debug.Log("Try Get File List From GBaaS");
			GBaaSObject.Instance.API.GetFileList();
		}

		if(paused)
			Time.timeScale = 0;
		else
			Time.timeScale = 1;
	}
}
