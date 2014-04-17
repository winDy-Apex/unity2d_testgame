using System;

namespace GBaaS.io.Objects
{
	public class GBAchievementObject
	{
		// parts from AchievementObject
		public string uuid { get; set; }
		public int incrementalCount { get; set; }
		public bool isMoreThanOnce { get; set; }
		public bool isHidden { get; set; }
		public int points { get; set; }

		// parts from AchievementLocaleObject
		public string achievementName { get; set; }
		public string preEarnedDescription { get; set; }
		public string earnedDescription { get; set; }

		// parts from UserAchievementObject
		public int currentStepCount { get; set; }
		public bool isUnlocked { get; set; }
	}
}
