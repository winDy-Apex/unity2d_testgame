/**
 * GBaaS API Interface For Unity SDK 가 제공하는 모든 기능의 최상위 인터페이스<br>
 * @note SetHandler 로 handler 를 지정하면 비동기(Async)로 동작합니다.
 * GBaaS 소스 파일은 외부 프로젝트에서 Unity Assets 로 
 * 자동으로 복사되어 생성되는 파일입니다.
 * GBaaSObject.cs 파일만 수정하시길 권하면 부득히 GBaaS 의 다른 소스를
 * 수정하여 사용하시는 경우 버전 관리에 각별히 유의 부탁드립니다.
 */
using System;
using System.Collections.Generic;
using GBaaS.io.Objects;
using GBaaS.io.Services;
using GBaaS.io.Utils;

namespace GBaaS.io {
	public enum Period { Alltime, Daily, Weekly, Monthly };
	public enum PushSendType { alldevices, devices, users, groups };
	public enum PushScheduleType { now, later };
	public enum ScoreOrder { DESC, ASC };

	/**
 	* 인터페이스 클래스 <br>
 	* @note 전체기능 목록에 해당 하는 인터페이스를 제공 하며 실제 기능은 해당 서비스를 호출하여 처리한다. <br>
 	*/
	public class GBaaSApi {
		GBaaSApiHandler _handler = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="GBaaS.io.GBaaSApi"/> class.
		/// 초기값이 필요하므로 불필요한 실수를 방지하기 위해 싱글톤으로 제공하지 않는다.
		/// </summary>
		/// <param name="gbaasUrl">GBaaS API를 사용하기 위한 기본 URL</param>
		public GBaaSApi(string gbaasUrl) {
			GBRequestService.Instance.SetGBaaSUrl(gbaasUrl);
		}

		/// <summary>
		/// 비동기 이벤트를 받을 핸들러를 지정한다.
		//  핸들러를 지정하면 Api 를 Async 로 호출한다.
		/// </summary>
		/// <param name="handler">비동기 이벤트를 받을 핸들러</param>
		public void SetHandler(GBaaSApiHandler handler) {
			_handler = handler;
			GBAchievementService.Instance.SetHandler(handler);
			GBScoreService.Instance.SetHandler(handler);
			GBUserService.Instance.SetHandler(handler);
			GBPushService.Instance.SetHandler(handler);
			GBNetService.Instance.SetHandler(handler);
		}

		/// <summary>
		/// 현재 Api 호출이 Async 로 되고 있는지 체크한다.
		/// </summary>
		/// <returns><c>true</c> Api 호출 상태가 비동기(Async)인 경우; 아니면, <c>false</c>.</returns>
		public bool IsAsync() {
			return (_handler != null);
		}

//********** For UserService ********** //
		/// <summary>
		/// Api 사용을 하기전 GBaaS 서버에 로그인 한다.
		/// 반드시 선행되어야 나머지 기능을 호출 할 수 있다.
		/// </summary>
		/// <param name="userName">사용자 아이디</param>
		/// <param name="password">사용자 암호</param>
		public bool Login(string userName, string password) {
			return GBUserService.Instance.Login (userName, password);
		}

		/// <summary>
		/// 페이스북 아이디를 이용하여 로그인 한다.
		/// </summary>
		/// <returns><c>true</c>, if with face book was logined, <c>false</c> otherwise.</returns>
		/// <param name="facebookToken">Facebook token.</param>
		public bool LoginWithFaceBook(string facebookToken) {
			return GBUserService.Instance.LoginWithFaceBook(facebookToken);
		}

		/// <summary>
		/// GBaaS 에 CreateUser 과정을 거치지 않고
		/// Device 에서 획득한 deviceID 또는 사용자 어플리케이션 단위로 생성한 UUID 등을
		/// 암묵적인 사용자 ID로 사용하여 로그인 한다.
		/// 이 경우 UpdateUserName 을 호출하여 사용자의 표시 이름은 별도로 수정할 수 있다.
		/// 단말에 설치된 앱 단위로 유니크한 유저키를 생성하는 방법은 아래의 링크를 참고로 한다.
		/// DeviceID 를 사용하여도 무방하다.
		/// http://blog.naver.com/PostView.nhn?blogId=huewu&logNo=110107222113
		/// </summary>
		/// <returns><c>true</c>, if without I was logined, <c>false</c> otherwise.</returns>
		/// <param name="uniqueUserKey">Unique user key.</param>
		public bool LoginWithoutID(string uniqueUserKey) {
			return GBUserService.Instance.LoginWithoutID(uniqueUserKey);
		}

		/// <summary>
		/// LoginWithoutID 와 쌍으로 사용한다.
		/// 임의로 로그인한 사용자에게 표시이름을 부여한다.
		/// </summary>
		/// <returns><c>true</c>, if user name was updated, <c>false</c> otherwise.</returns>
		/// <param name="userName">User name.</param>
		public bool UpdateUserName(string userName) {
			return GBUserService.Instance.UpdateUserName(userName);
		}

		/// <summary>
		/// Api 사용을 위한 로그인 세션을 종료한다.
		/// </summary>
		public void Logout() {
			GBUserService.Instance.Logout ();
		}

		/// <summary>
		/// 현재 로그인 상태를 확인한다.
		/// </summary>
		/// <returns><c>true</c> if this instance is login; otherwise, <c>false</c>.</returns>
		public bool IsLogin() {
			return GBUserService.Instance.IsLogin ();
		}

		/// <summary>
		/// 사용자를 생성한다. (가입처리)
		/// </summary>
		/// <returns>생성된 사용자 정보에 대한 UUID</returns>
		/// <param name="userModel">User model.</param>
		public string CreateUser(Objects.GBUserObject userModel) {
			return GBUserService.Instance.CreateUser (userModel);
		}

		/// <summary>
		/// 사용자 암호등 사용자 정보를 수정한다.
		/// </summary>
		/// <returns>수정된 사용자 정보에 대한 Json String, 수정 확인용도로만 사용한다.</returns>
		/// <param name="userModel">User model.</param>
		public string UpdateUser(Objects.GBUserObject userModel) {
			return GBUserService.Instance.UpdateUser (userModel);
		}

		/// <summary>
		/// Gets the user info.
		/// </summary>
		/// <returns>The user info.</returns>
		public Objects.GBUserObject GetUserInfo() {
			return GBUserService.Instance.GetUserInfo();
		}
			
		/// <summary>
		/// 전체 사용자 정보를 리스트로 가져온다.
		/// </summary>
		/// <returns>The user list.</returns>
		public List<Objects.GBUserObject> GetUserList() {
			return GBUserService.Instance.GetUserList ();
		}

		/// <summary>
		/// Follower 리스트를 가져온다.
		/// </summary>
		/// <returns>The followers.</returns>
		public List<Objects.GBUserObject> GetFollowers() {
			return GBUserService.Instance.GetFollowers ();
		}
		
		/// <summary>
		/// Following 하는 사용자 정보를 리스트로 가져온다.
		/// </summary>
		/// <returns>The following.</returns>
		public List<Objects.GBUserObject> GetFollowing() {
			return GBUserService.Instance.GetFollowing ();
		}

		/// <summary>
		/// 다른 사용자를 Follow 한다.
		/// </summary>
		/// <returns><c>true</c>, if user was followed, <c>false</c> otherwise.</returns>
		/// <param name="userModel">User model.</param>
		public bool FollowUser(Objects.GBUserObject userModel) {
			return GBUserService.Instance.FollowUser (userModel);
		}

		/// <summary>
		/// 사용자 그룹을 생성한다.
		/// </summary>
		/// <returns><c>true</c>, if group was created, <c>false</c> otherwise.</returns>
		/// <param name="groupModel">Group model.</param>
		public bool CreateGroup(Objects.GBGroupObject groupModel) {
			return GBUserService.Instance.CreateGroup (groupModel);
		}

		/// <summary>
		/// 사용자를 그룹에 추가한다.
		/// </summary>
		/// <returns><c>true</c>, if user to group was added, <c>false</c> otherwise.</returns>
		/// <param name="userName">User name.</param>
		/// <param name="groupID">Group I.</param>
		public bool AddUserToGroup(string userName, string groupID)  {
			return GBUserService.Instance.AddUserToGroup (userName, groupID);
		}

		/// <summary>
		/// 사용자를 그룹에서 제거한다.
		/// </summary>
		/// <returns><c>true</c>, if user from group was removed, <c>false</c> otherwise.</returns>
		/// <param name="userName">User name.</param>
		/// <param name="groupID">Group I.</param>
		public bool RemoveUserFromGroup(string userName, string groupID) {
			return GBUserService.Instance.RemoveUserFromGroup (userName, groupID);
		}

		/// <summary>
		/// 그룹에 속한 사용자 리스트를 가져온다.
		/// </summary>
		/// <returns>The users for group.</returns>
		/// <param name="groupID">Group I.</param>
		public List<Objects.GBUserObject> GetUsersForGroup(string groupID) {
			return GBUserService.Instance.GetUsersForGroup (groupID);
		}

//********** For ScoreService ********** //
		/// <summary>
		/// 점수 등록하기
		/// Score Object 는 Private 멤버인 UUID 를 가지고 있다.
		/// UUID 를 설정하여 요청하면 기존 항목을 Update 한다.
		/// </summary>
		/// <returns><c>true</c>, if score was added, <c>false</c> otherwise.</returns>
		/// <param name="score">Score.</param>
		public bool AddScore(Objects.GBScoreObject score) {
			return GBScoreService.Instance.AddScore(score);
		}

		/// <summary>
		/// 사용자의 UUID나 로그인 이름으로 점수를 가져온다.
		/// </summary>
		/// <returns>The score by UUID or name.</returns>
		/// <param name="uuidOrName">UUID or name.</param>
		public List<Objects.GBScoreObject> GetScoreByUuidOrName(string uuidOrName = "") {
			return GBScoreService.Instance.GetScoreByUuidOrName(uuidOrName);
		}

		/// <summary>
		/// 로그인된 사용자의 점수를 가져온다.
		/// </summary>
		/// <returns>The score.</returns>
		/// <param name="name">Name.</param>
		/// <param name="stage">Stage.</param>
		/// <param name="unit">Unit.</param>
		/// <param name="limit">Limit.</param>
		/// <param name="isMore">If set to <c>true</c> is more. for Continuos Query</param>
		/// <param name="period">점수를 구하는 기간을 지정한다. 주간, 일간 지원</param>
		/// <param name="weekstart">주간 단위로 점수를 구할 경우 시작하는 주의 첫 요일 지정</param>
		public List<Objects.GBScoreObject> GetScoreMore(string stage = "", string unit = "", int limit = 0, bool isMore = false, Period period = Period.Alltime, DayOfWeek weekstart = DayOfWeek.Sunday) {
			return GBScoreService.Instance.GetScoreMore(stage, unit, limit, isMore, period, weekstart);
		}

		/// <summary>
		/// 로그인된 사용자 점수를 Cursor 위치를 기준으로 가져온다.
		/// </summary>
		/// <returns>The score.</returns>
		/// <param name="name">Name.</param>
		/// <param name="stage">Stage.</param>
		/// <param name="unit">Unit.</param>
		/// <param name="limit">Limit.</param>
		/// <param name="cursor">Cursor.</param>
		/// <param name="period">점수를 구하는 기간을 지정한다. 주간, 일간 지원</param>
		/// <param name="weekstart">주간 단위로 점수를 구할 경우 시작하는 주의 첫 요일 지정</param>
		public List<Objects.GBScoreObject> GetScore(string stage = "", string unit = "", int limit = 0, string cursor = "", Period period = Period.Alltime, DayOfWeek weekstart = DayOfWeek.Sunday) {
			return GBScoreService.Instance.GetScore(stage, unit, limit, cursor, period, weekstart);
		}

		/// <summary>
		/// Gets the rank.
		/// </summary>
		/// <returns>The rank.</returns>
		/// <param name="stage">Stage.</param>
		/// <param name="unit">Unit.</param>
		/// <param name="scoreOrder">Score order.</param>
		/// <param name="period">Period.</param>
		/// <param name="rank">Rank.</param>
		/// <param name="range">Range.</param>
		public List<Objects.GBScoreObject> GetRank(string stage = "", string unit = "", 
			ScoreOrder scoreOrder = ScoreOrder.DESC, Period period = Period.Daily, int rank = 0, int range = 1) {
			return GBScoreService.Instance.GetRank(stage, unit, scoreOrder, period, rank, range);
		}

		/// <summary>
		/// 사용자 점수가 기록된 이력을 조회한다. (같은 Stage, 같은 Unit 의 정보를 이력으로 조회한다.)
		/// </summary>
		/// <returns>The score log.</returns>
		/// <param name="stage">Stage.</param>
		/// <param name="unit">Unit.</param>
		/// <param name="limit">Limit.</param>
		/// <param name="isMore">If set to <c>true</c> is more. for Continuos Query</param>
		/// <param name="period">점수를 구하는 기간을 지정한다. 주간, 일간 지원</param>
		/// <param name="weekstart">주간 단위로 점수를 구할 경우 시작하는 주의 첫 요일 지정</param>
		public List<Objects.GBScoreObject> GetScoreLogMore(string stage = "", string unit = "", int limit = 0, bool isMore = false, Period period = Period.Alltime, DayOfWeek weekstart = DayOfWeek.Sunday) {
			return GBScoreService.Instance.GetScoreLogMore(stage, unit, limit, isMore, period, weekstart);
		}

		/// <summary>
		/// 사용자 점수가 기록된 이력을 조회한다. (같은 Stage, 같은 Unit 의 정보를 이력으로 조회한다.)
		/// </summary>
		/// <returns>The score log.</returns>
		/// <param name="name">Name.</param>
		/// <param name="stage">Stage.</param>
		/// <param name="unit">Unit.</param>
		/// <param name="limit">Limit.</param>
		/// <param name="cursor">Cursor.</param>
		/// <param name="period">점수를 구하는 기간을 지정한다. 주간, 일간 지원</param>
		/// <param name="weekstart">주간 단위로 점수를 구할 경우 시작하는 주의 첫 요일 지정</param>
		public List<Objects.GBScoreObject> GetScoreLog(string stage = "", string unit = "", int limit = 0, string cursor = "", Period period = Period.Alltime, DayOfWeek weekstart = DayOfWeek.Sunday) {
			return GBScoreService.Instance.GetScoreLog(stage, unit, limit, cursor, period, weekstart);
		}

		/// <summary>
		/// Api 에서 시간값으로 사용하는 Timestamp 를 구하기 위한 유틸 메소드
		/// </summary>
		/// <returns>The time stamp.</returns>
		/// <param name="period">Period.</param>
		/// <param name="weekstart">Weekstart.</param>
		public string GetTimeStamp(Period period = Period.Alltime, DayOfWeek weekstart = DayOfWeek.Sunday) {
			return GBScoreService.Instance.GetTimeStamp (period, weekstart);
		}

//********** For AchievementService ********** //
		/// <summary>
		/// 서버를 통해서 등록하세요. Adds the user achievement.
		/// </summary>
		/// <returns><c>true</c>, if user achievement was added, <c>false</c> otherwise.</returns>
		/// <param name="achievement">Achievement.</param>
		public bool AddUserAchievement(GBObject achievement) {
			return GBAchievementService.Instance.AddUserAchievement(achievement);
		}

		/// <summary>
		/// 달성한 또는 달성 가능한 업적 목록 가져오기.
		/// </summary>
		/// <returns>The achievement.</returns>
		/// <param name="locale">Locale.</param>
		/// <param name="limit">Limit.</param>
		/// <param name="cursor">Cursor.</param>
		public List<Objects.GBAchievementObject> GetAchievement(string locale = "", int limit = 0, string cursor = "") {
			return GBAchievementService.Instance.GetAchievement(locale, limit, cursor);
		}

		/// <summary>
		/// 사용자의 UUID 또는 로그인 이름으로 업적 목록 조회
		/// </summary>
		/// <returns>The achievement by UUI dor name.</returns>
		/// <param name="uuidOrName">UUID or name.</param>
		/// <param name="locale">Locale.</param>
		public Objects.GBAchievementObject GetAchievementByUUIDorName(string uuidOrName, string locale = "") {
			return GBAchievementService.Instance.GetAchievementByUUIDorName(uuidOrName, locale);
		}

		/// <summary>
		/// 사용자가 달성한 업적 정보 수정 요청
		/// </summary>
		/// <returns>The achievement.</returns>
		/// <param name="uuid">UUID.</param>
		/// <param name="currentStepCount">Current step count.</param>
		/// <param name="isUnlocked">If set to <c>true</c> is unlocked.</param>
		/// <param name="locale">Locale.</param>
		public Objects.GBAchievementObject UpdateAchievement(string uuid, int currentStepCount, bool isUnlocked, string locale = "") {
			return GBAchievementService.Instance.UpdateAchievement(uuid, currentStepCount, isUnlocked, locale);
		}

//********** For PushService ********** //
		/// <summary>
		/// Push Notification 메시지를 전송한다.
		/// </summary>
		/// <returns><c>true</c>, if message was sent, <c>false</c> otherwise.</returns>
		/// <param name="message">Message.</param>
		/// <param name="scheduleDate">Schedule date.</param>
		/// <param name="deviceIds">Device identifiers.</param>
		/// <param name="groupPaths">Group paths.</param>
		/// <param name="userNames">User names.</param>
		/// <param name="sendType">Send type.</param>
		/// <param name="scheduleType">Schedule type.</param>
		public bool SendMessage(
			string message, string scheduleDate, string deviceIds, string groupPaths, string userNames, 
			PushSendType sendType, PushScheduleType scheduleType) {
			return GBPushService.Instance.SendMessage(message, scheduleDate, deviceIds, groupPaths, userNames, sendType, scheduleType);
		}

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
			return GBPushService.Instance.RegisterDevice(deviceModel, deviceOSVersion, devicePlatform, registeration_id);
		}

//********** For GameDataService ********** //
		// Game Data 서비스는 Custom Object 를
		// Key - Value 형태로 서비스 하며
		// 사용자의 게임 데이터를 저장하는 용도로 사용한다.
		// Value 는 저장 전에 암호화 되어 전송되며
		// Load 될 때 자동으로 복호화 된다.
		public bool GameDataSave(string key, string value) {
			return GBCollectionService.Instance.GameDataSave(key, value);
		}

		public string GameDataLoad(string key) {
			return GBCollectionService.Instance.GameDataLoad(key);
		}

		public bool ReceiptSave(GBReceiptObject receipt) {
			return receipt.Save();
		}

//********** For CollectionService ********** //
		/// <summary>
		/// Custom Collection Object 의 목록을 가져온다.
		/// </summary>
		/// <returns>The list.</returns>
		/// <param name="collectionName">Collection name.</param>
		public List<Objects.GBObject> GetList(string collectionName) {
			return GBCollectionService.Instance.GetList(collectionName);
		}

		/// <summary>
		/// Custom Collection Object 의 목록을 가져온다. (지정한 Geo Location 범위 내에서)
		/// </summary>
		/// <returns>The list in range.</returns>
		/// <param name="collectionName">Collection name.</param>
		/// <param name="meters">Meters.</param>
		/// <param name="latitude">Latitude.</param>
		/// <param name="longitude">Longitude.</param>
		public List<Objects.GBObject> GetListInRange(string collectionName, float meters, float latitude, float longitude) {
			return GBCollectionService.Instance.GetListInRange(collectionName, meters, latitude, longitude);
		}

		/// <summary>
		/// 지정한 Key 의 Value 에 해당하는 Custom Object 를 구한다.
		/// </summary>
		/// <returns>The object.</returns>
		/// <param name="objectName">Object name.</param>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		/// <param name="limit">Limit.</param>
		public List<Objects.GBObject> GetObject(string objectName, string key, string value, int limit = 1) {
			return GBCollectionService.Instance.GetObject(objectName, key, value, limit);
		}

		/// <summary>
		/// Custom Collection 을 생성한다.
		/// </summary>
		/// <returns><c>true</c>, if list was created, <c>false</c> otherwise.</returns>
		/// <param name="collectionName">Collection name.</param>
		/// <param name="list">List.</param>
		public bool CreateList(string collectionName, List<Objects.GBObject> list) {
			return GBCollectionService.Instance.CreateList(collectionName, list);
		}

//********** For RequestService ********** //
		/// <summary>
		/// Login 으로 대체한다. 이전 버전과 호환성 제공을 위해 인터페이스는 유지한다.
		/// </summary>
		/// <returns>The token.</returns>
		/// <param name="username">Username.</param>
		/// <param name="password">Password.</param>
		public string GetToken(string username, string password) {
			return GBRequestService.Instance.GetToken (username, password);
		}

		/// <summary>
		/// Looks up token.
		/// </summary>
		/// <returns>The up token.</returns>
		/// <param name="token">Token.</param>
		public string LookUpToken(string token) {
			return GBRequestService.Instance.LookUpToken (token);
		}

//********** For NetService ********** //
		public bool ConnectNetService(string IP, int port) {
			return GBNetService.Instance.Connect(IP, port);
		}

		public bool SessionIn(string userName, string appID) {
			return GBNetService.Instance.SessionIn(userName, appID);
		}

		public bool LobbyIn(string lobbyIP, int lobbyPort, string appID) {
			return GBNetService.Instance.LobbyIn(lobbyIP, lobbyPort, appID);
		}

		public bool RoomIn(int roomID, string title, string tag, string password, int maxUser) {
			return GBNetService.Instance.RoomIn(roomID, title, tag, password, maxUser);
		}

		public List<Objects.GBRoom> GetRoomList() {
			return GBNetService.Instance.GetRoomList();
		}

		public bool ChannelSend(string data, int dataType=0, bool echo=true) {
			return GBNetService.Instance.ChannelSend(data, dataType, echo);
		}
		 
		public int SendData(string jsonPacket) {
			return GBNetService.Instance.SendData(jsonPacket);
		}
	}
}
