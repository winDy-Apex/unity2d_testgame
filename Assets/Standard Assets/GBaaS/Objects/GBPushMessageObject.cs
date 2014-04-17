using System;

namespace GBaaS.io.Objects
{
	public class GBPushMessageObject : GBObject
	{
		public GBPushMessageObject() {
			this.SetEntityType("pushmessages");
		}

		public string message { get; set; }
		public string scheduleDate { get; set; }
		public string deviceIds { get; set; }
		public string groupPaths { get; set; }
		public string userNames { get; set; }
		public string sendType { get; set; }
		public string scheduleType { get; set; }
	}
}
