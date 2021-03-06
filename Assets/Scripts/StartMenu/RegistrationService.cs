﻿using UnityEngine;

using GBaaS.io;
using GBaaS.io.Objects;

interface RegistrationServiceListener {
	void	registrationResponseHandler(Response response);
}

class RegistrationService : GBaaSApiHandler {
	private RegistrationServiceListener _listener = null;
	
	public void SetListener(RegistrationServiceListener listener) {
		_listener = listener;
	}
	
	public void sendRegistrationData(RegistrationData registrationData) {
		Debug.Log("sendRegistrationData " + registrationData.username);
		GBaaSObject.Instance.Init(this);
		GBaaSObject.Instance.CreateUser(registrationData.username, registrationData.name, registrationData.password, registrationData.email);
    }
	
	public override void OnCreateUser(GBResult result) {
		Debug.Log("OnCreateUser " + result.reason);

		Response response = new Response(); 

		if(result.isSuccess) {
			// Registration was successful:
			Debug.Log("You was registered successfully");
			response.error = false;
			response.message = "";
		} else {
			Debug.Log("Error: Registration to Server");
			response.error = true;
			response.message = "Error: Registration to Server";
		}

		// Calling response handler:
		_listener.registrationResponseHandler(response);
	}
}
