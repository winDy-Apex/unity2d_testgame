using System;

namespace GBaaS.io
{
	public class GBLobbyObject
	{
		public string uuid { get; set; }
		public string appId { get; set; }
		public string title { get; set; }
		public string tag { get; set; }
		public int userCount { get; set; }
		public int maxUser { get; set; }
		public string ip { get; set; }
		public int port { get; set; }
	}
}
