using System.Collections.Generic;

namespace GBaaS.io {
	public abstract class GBaaSApiHandler {
		public virtual void OnResult(object result) {}

		// For Achievement Service
		public virtual void OnGetAchievement(List<Objects.GBAchievementObject> result) {}
		public virtual void OnGetAchievementByUUIDorName(Objects.GBAchievementObject result) {}
		public virtual void OnUpdateAchievement(Objects.GBAchievementObject result) {}

		// For Score(Leaderboard) Service
		public virtual void OnAddScore(bool result) {}
		public virtual void OnGetScoreByUuidOrName(List<Objects.GBScoreObject> result) {}
		public virtual void OnGetScore(List<Objects.GBScoreObject> result) {}
		public virtual void OnGetRank(List<Objects.GBScoreObject> result) {}
		public virtual void OnGetScoreLog(List<Objects.GBScoreObject> result) {}
		
		// For User Service
		public virtual void OnLogin(bool result) {}
		public virtual void OnLoginWithFaceBook(bool result) {}
		public virtual void OnLoginWithoutID(bool result) {}
		public virtual void OnCreateUser(string result) {}
		public virtual void OnUpdateUser(string result) {}
		public virtual void OnUpdateUserName(bool result) {}
		public virtual void OnGetUserInfo(Objects.GBUserObject result) {}
		public virtual void OnGetUserList(List<Objects.GBUserObject> result) {}
		public virtual void OnGetFollowers(List<Objects.GBUserObject> result) {}
		public virtual void OnGetFollowing(List<Objects.GBUserObject> result) {}
		public virtual void OnFollowUser(bool result) {}
		public virtual void OnCreateGroup(bool result) {}
		public virtual void OnAddUserToGroup(bool result) {}
		public virtual void OnRemoveUserFromGroup(bool result) {}
		public virtual void OnGetUsersForGroup(List<Objects.GBUserObject> result) {}

		// For Push Service
		public virtual void OnSendMessage(bool result) {}
		public virtual void OnRegisterDevice(bool result) {}

		// For GameData Service
		public virtual void OnGameDataSave(bool result) {}
		public virtual void OnGameDataLoad(string result) {}

		// For Collection Service
		public virtual void OnGetList(List<Objects.GBObject> result) {}
		public virtual void OnGetListInRange(List<Objects.GBObject> result) {}
		public virtual void OnGetObject(List<Objects.GBObject> result) {}
		public virtual void OnCreateList(bool result) {}

		// For Net Service
		public virtual void OnReceiveData(string recvPacket) {}
	}
}
