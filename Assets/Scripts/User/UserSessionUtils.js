// User session utils:
class UserSessionUtils {

    public static function getUserLogin() {
        //return findSctiptInGameObject("UserSession", "UserSession").userLogin;
    }

    public static function setUserLogin(userLogin : String) {            
    	//findSctiptInGameObject("UserSession", "UserSession").userLogin = userLogin;
    }
    
    /*
    private static function findSctiptInGameObject(objectName : String, scriptName : String) {
    	var userSession : GameObject = GameObject.Find(objectName);
    	return userSession.GetComponent(scriptName);
    }
    */
}
