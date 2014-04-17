using System;

namespace GBaaS.io.Objects
{
	public class GBAchievementInfoObject : GBObject
	{
		public string uuid { get; set; }
		public string name { get; set; }
		public int incrementalCount { get; set; }
		public bool isMoreThanOnce { get; set; }
		public bool isHidden { get; set; }
		public int points { get; set; }
		public int listOrder { get; set; }
		public int processStatus { get; set; }
	}
}
