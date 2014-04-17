using System;

namespace GBaaS.io.Objects
{
	public class GBUserAchievementObject : GBObject
	{
		public string uuid { get; set; }
		public string name { get; set; }
		public string userName { get; set; }
		public string achievementId { get; set; }
		public int currentStepCount { get; set; }
		public bool isUnlocked { get; set; }
	}
}
