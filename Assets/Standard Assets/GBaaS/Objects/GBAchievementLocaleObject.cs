using System;

namespace GBaaS.io.Objects
{
	public class GBAchievementLocaleObject : GBObject
	{
		public string uuid { get; set; }
		public string name { get; set; }
		public string achievementId { get; set; }
		public string localeId { get; set; }
		public string achievementName { get; set; }
		public string preEarnedDescription { get; set; }
		public string earnedDescription { get; set; }
		public bool isDefaultLanguage { get; set; }
	}
}
