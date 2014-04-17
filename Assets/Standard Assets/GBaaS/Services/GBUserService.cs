using System;
using System.Collections.Generic;
using GBaaS.io.Utils;
using System.Threading;

namespace GBaaS.io.Services
{
	class GBUserService : GBService<GBUserService>
	{
		private string _userNmae = "";

		public GBUserService() {}

		public GBaaSApiHandler _handler = null;

		public void SetHandler(GBaaSApiHandler handler) {
			_handler = handler;
		}

		private bool IsAsync() {
			return (_handler != null);
		}

		public string GetLoginName() {
			return _userNmae;
		}

		public bool Login(string userName, string password) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.LoginThread(userName, password));
				workerThread.Start();
				return false;
			} else {
				return this.LoginThread(userName, password);
			}
		}

		private bool LoginThread(string userName, string password) {
			var result = GBRequestService.Instance.GetToken(userName, password);
			if (result.Length > 0) {
				HttpHelper.Instance._accessToken = result;
				_userNmae = userName;
				if (IsAsync()) {
					_handler.OnLogin(true);
				} else {
					return true;
				}
			} else {
				if (IsAsync()) {
					_handler.OnLogin(false);
				} else {
					return false;
				}
			}

			return false;
		}

		public bool LoginWithFaceBook(string facebookToken) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.LoginWithFaceBookThread(facebookToken));
				workerThread.Start();
				return false;
			} else {
				return this.LoginWithFaceBookThread(facebookToken);
			}
		}

		private bool LoginWithFaceBookThread(string facebookToken) {
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/auth/facebook?fb_access_token=" + facebookToken);

			if (rawResults.Length > 0) {
				string result = GBRequestService.Instance.GetValueFromJson("access_token", rawResults);
				HttpHelper.Instance._accessToken = result;

				if (IsAsync()) {
					_handler.OnLoginWithFaceBook(true);
				} else {
					return true;
				}
			} else {
				if (IsAsync()) {
					_handler.OnLoginWithFaceBook(false);
				} else {
					return false;
				}
			}

			return false;
		}

		// GBaaS 에 CreateUser 과정을 거치지 않고
		// Device 에서 획득한 deviceID 또는 사용자 어플리케이션 단위로 생성한 UUID 등을
		// 암묵적인 사용자 ID로 사용하여 로그인 한다.
		// 이 경우 UpdateUserName 을 호출하여 사용자의 표시 이름은 별도로 수정할 수 있다.
		// 단말에 설치된 앱 단위로 유니크한 유저키를 생성하는 방법은 아래의 링크를 참고로 한다.
		// DeviceID 를 사용하여도 무방하다.
		// http://blog.naver.com/PostView.nhn?blogId=huewu&logNo=110107222113
		public bool LoginWithoutID(string uniqueUserKey) {
			uniqueUserKey = "gbaas_" + uniqueUserKey;
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.LoginWithoutIDThread(uniqueUserKey));
				workerThread.Start();
				return false;
			} else {
				return this.LoginWithoutIDThread(uniqueUserKey);
			}
		}

		private bool LoginWithoutIDThread(string uniqueUserKey) {
			bool isLogin = Login(uniqueUserKey, uniqueUserKey);
			string result = "";

			if (isLogin == false) {
				result = CreateUser(new Objects.GBUserObject {
					username = uniqueUserKey,
					password = uniqueUserKey,
					Email = ""
				});

				if (result.Length > 0) {
					isLogin = Login(uniqueUserKey, uniqueUserKey);
				}
			}

			if (IsAsync()) {
				_handler.OnLoginWithoutID(isLogin);
			} else {
				return isLogin;
			}

			return false;
		}

		public bool UpdateUserName(string userName) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.UpdateUserNameThread(userName));
				workerThread.Start();
				return false;
			} else {
				return this.UpdateUserNameThread(userName);
			}
		}

		private bool UpdateUserNameThread(string userName) {
			if (IsLogin() == false)
				return false;

			var result = UpdateUser(new Objects.GBUserObject {
				username = GetLoginName(),
				name = userName
			});

			if (IsAsync()) {
				_handler.OnUpdateUserName(HttpHelper.Instance.CheckSuccess(result));
			} else {
				return HttpHelper.Instance.CheckSuccess(result);
			}

			return false;
		}

		public void Logout() {
			HttpHelper.Instance._accessToken = "";
		}

		public bool IsLogin() {
			return (HttpHelper.Instance._accessToken.Length > 0);
		}

		public string CreateUser(Objects.GBUserObject userModel) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.CreateUserThread(userModel));
				workerThread.Start();
				return default(string);
			} else {
				return this.CreateUserThread(userModel);
			}
		}

		private string CreateUserThread(Objects.GBUserObject userModel) {
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/users", HttpHelper.RequestTypes.Post, userModel);
			var entitiesResult = GBRequestService.Instance.GetEntitiesFromJson(rawResults);
			if (entitiesResult != null) {
				if (IsAsync()) {
					_handler.OnCreateUser(entitiesResult[0]["uuid"].ToString());
				} else {
					return entitiesResult[0]["uuid"].ToString();
				}
			} else {
				if (IsAsync()) {
					_handler.OnCreateUser(UpdateUser(userModel));
				} else {
					return UpdateUser(userModel);
				}
			}

			return default(string);
		}

		public string UpdateUser(Objects.GBUserObject userModel) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.UpdateUserThread(userModel));
				workerThread.Start();
				return default(string);
			} else {
				return this.UpdateUserThread(userModel);
			}
		}

		private string UpdateUserThread(Objects.GBUserObject userModel) {
			var rawResults = GBRequestService.Instance.PerformRequest<string>(
				"/users/" + userModel.username, HttpHelper.RequestTypes.Put, userModel);

			if (IsAsync()) {
				_handler.OnUpdateUser(rawResults.ToString());
			} else {
				return rawResults.ToString();
			}

			return default(string);
		}

		public Objects.GBUserObject GetUserInfo() {

			if (IsLogin() == false)
				return null;

			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.GetUserInfoThread());
				workerThread.Start();
				return default(Objects.GBUserObject);
			} else {
				return this.GetUserInfoThread();
			}
		}

		private Objects.GBUserObject GetUserInfoThread() {
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/users/me/?access_token=" + HttpHelper.Instance._accessToken);
			var user = GBRequestService.Instance.GetEntitiesFromJson(rawResults);

			if (IsAsync()) {
				_handler.OnGetUserInfo(MakeUserInfo(user));
			} else {
				return MakeUserInfo(user);
			}

			return default(Objects.GBUserObject);
		}

		public List<Objects.GBUserObject> GetUserList() {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.GetUserListThread());
				workerThread.Start();
				return default(List<Objects.GBUserObject>);
			} else {
				return this.GetUserListThread();
			}
		}

		private List<Objects.GBUserObject> GetUserListThread() {
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/users");
			var users = GBRequestService.Instance.GetEntitiesFromJson(rawResults);

			if (IsAsync()) {
				_handler.OnGetUserList(MakeUserList(users));
			} else {
				return MakeUserList(users);
			}

			return default(List<Objects.GBUserObject>);
		}

		public List<Objects.GBUserObject> GetFollowers() {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.GetFollowersThread());
				workerThread.Start();
				return default(List<Objects.GBUserObject>);
			} else {
				return this.GetFollowersThread();
			}
		}

		private List<Objects.GBUserObject> GetFollowersThread() {
			if (!IsLogin()) {
				return default(List<Objects.GBUserObject>);
			}
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/users/me/followers");
			var users = GBRequestService.Instance.GetEntitiesFromJson(rawResults);
		
			if (IsAsync()) {
				_handler.OnGetUserList(MakeUserList(users));
			} else {
				return MakeUserList(users);
			}

			return default(List<Objects.GBUserObject>);
		}

		public List<Objects.GBUserObject> GetFollowing() {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.GetFollowingThread());
				workerThread.Start();
				return default(List<Objects.GBUserObject>);
			} else {
				return this.GetFollowingThread();
			}
		}

		private List<Objects.GBUserObject> GetFollowingThread() {
			if (!IsLogin()) {
				return default(List<Objects.GBUserObject>);
			}
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/users/me/following");
			var users = GBRequestService.Instance.GetEntitiesFromJson(rawResults);

			if (IsAsync()) {
				_handler.OnGetUserList(MakeUserList(users));
			} else {
				return MakeUserList(users);
			}

			return default(List<Objects.GBUserObject>);
		}

		public bool FollowUser(Objects.GBUserObject userModel) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.FollowUserThread(userModel));
				workerThread.Start();
				return false;
			} else {
				return this.FollowUserThread(userModel);
			}
		}

		private bool FollowUserThread(Objects.GBUserObject userModel) {
			if (!IsLogin()) {
				return false;
			}
			var rawResults = GBRequestService.Instance.PerformRequest<string>(
				"/users/me/following/users/" + userModel.username, HttpHelper.RequestTypes.Post, "");

			if (IsAsync()) {
				_handler.OnFollowUser(HttpHelper.Instance.CheckSuccess(rawResults));
			} else {
				return HttpHelper.Instance.CheckSuccess (rawResults);
			}

			return false;
		}

		public bool CreateGroup(Objects.GBGroupObject groupModel) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.CreateGroupThread(groupModel));
				workerThread.Start();
				return false;
			} else {
				return this.CreateGroupThread(groupModel);
			}
		}

		private bool CreateGroupThread(Objects.GBGroupObject groupModel) {
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/group", HttpHelper.RequestTypes.Post, groupModel);

			if (IsAsync()) {
				_handler.OnCreateGroup(HttpHelper.Instance.CheckSuccess(rawResults));
			} else {
				return HttpHelper.Instance.CheckSuccess(rawResults);
			}

			return false;
		}

		public bool AddUserToGroup(string userName, string groupID) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.AddUserToGroupThread(userName, groupID));
				workerThread.Start();
				return false;
			} else {
				return this.AddUserToGroupThread(userName, groupID);
			}
		}

		private bool AddUserToGroupThread(string userName, string groupID) {
			string requestUrl = "/group/" + groupID + "/users/" + userName;

			var rawResults = GBRequestService.Instance.PerformRequest<string> (requestUrl, HttpHelper.RequestTypes.Post, "");

			if (IsAsync()) {
				_handler.OnAddUserToGroup(HttpHelper.Instance.CheckSuccess (rawResults));
			} else {
				return HttpHelper.Instance.CheckSuccess (rawResults);
			}

			return false;
		}

		public bool RemoveUserFromGroup(string userName, string groupID) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.RemoveUserFromGroupThread(userName, groupID));
				workerThread.Start();
				return false;
			} else {
				return this.RemoveUserFromGroupThread(userName, groupID);
			}
		}

		private bool RemoveUserFromGroupThread(string userName, string groupID) {
			string requestUrl = "/group/" + groupID + "/users/" + userName;

			var rawResults = GBRequestService.Instance.PerformRequest<string> (requestUrl, HttpHelper.RequestTypes.Delete, "");

			if (IsAsync()) {
				_handler.OnRemoveUserFromGroup(HttpHelper.Instance.CheckSuccess (rawResults));
			} else {
				return HttpHelper.Instance.CheckSuccess (rawResults);
			}

			return false;
		}

		public List<Objects.GBUserObject> GetUsersForGroup(string groupID) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.GetUsersForGroupThread(groupID));
				workerThread.Start();
				return default(List<Objects.GBUserObject>);
			} else {
				return this.GetUsersForGroupThread(groupID);
			}
		}

		private List<Objects.GBUserObject> GetUsersForGroupThread(string groupID) {
			string requestUrl = "/group/" + groupID + "/users";
			
			var rawResults = GBRequestService.Instance.PerformRequest<string>(requestUrl);
			var users = GBRequestService.Instance.GetEntitiesFromJson(rawResults);

			if (IsAsync()) {
				_handler.OnGetUsersForGroup(MakeUserList (users));
			} else {
				return MakeUserList (users);
			}

			return default(List<Objects.GBUserObject>);
		}

		private List<Objects.GBUserObject> MakeUserList(Newtonsoft.Json.Linq.JToken users) {
			List<Objects.GBUserObject> results = new List<Objects.GBUserObject>();
			foreach (var usr in users)
			{
				results.Add(new Objects.GBUserObject { 
					uuid = (usr["uuid"] ?? "").ToString(),
					name = (usr["name"] ?? "").ToString(),
					username = (usr["username"] ?? "").ToString(),
					password = (usr["password"] ?? "").ToString(),
					lastname = (usr["lastname"] ?? "").ToString(),
					firstname = (usr["firstname"] ?? "").ToString(),
					title = (usr["title"] ?? "").ToString(),
					Email = (usr["Email"] ?? "").ToString(),
					tel = (usr["tel"] ?? "").ToString(),
					homePage = (usr["homepage"] ?? "").ToString(),
					bday = (usr["bday"] ?? "").ToString(),
					picture = (usr["picture"] ?? "").ToString(),
					url = (usr["url"] ?? "").ToString()
				});
			}

			return results;
		}

		private Objects.GBUserObject MakeUserInfo(Newtonsoft.Json.Linq.JToken userToken) {
			var user = userToken[0];
			Objects.GBUserObject result = new Objects.GBUserObject { 
				uuid = (user["uuid"] ?? "").ToString(),
				name = (user["name"] ?? "").ToString(),
				username = (user["username"] ?? "").ToString(),
				password = (user["password"] ?? "").ToString(),
				lastname = (user["lastname"] ?? "").ToString(),
				firstname = (user["firstname"] ?? "").ToString(),
				title = (user["title"] ?? "").ToString(),
				Email = (user["Email"] ?? "").ToString(),
				tel = (user["tel"] ?? "").ToString(),
				homePage = (user["homepage"] ?? "").ToString(),
				bday = (user["bday"] ?? "").ToString(),
				picture = (user["picture"] ?? "").ToString(),
				url = (user["url"] ?? "").ToString()
			};

			return result;
		}
	}
}
