class RegistrationService {
    public function sendRegistrationData(registrationData : RegistrationData, responseHandler : function(Response)) {
        var response : Response = new Response(); 
        
    	var cs = GameObject.Find("GBaaSObject");
    	var script : GBaaSObject;
    	script = cs.GetComponent("GBaaSObject");
    	script.Init();
	
    	var result = script.CreateUser(registrationData.username, registrationData.password, registrationData.email);
    	
    	yield result; // StartCoroutine must need yield ~!!!
    	
    	if(result.length > 0) {
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
        responseHandler(response);
    }
}