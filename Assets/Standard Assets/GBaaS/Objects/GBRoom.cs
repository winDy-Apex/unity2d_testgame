using System;
using GBaaS.io.Services;
using GBaaS.io.Utils;

namespace GBaaS.io.Objects
{
	public class GBRoom : GBObject
	{
		public int roomID { get; set; }
		public string title { get; set; }
		public string tag { get; set; }
		public string owner { get; set; }
		public bool isAvailable { get; set; }
		public bool isLocked { get; set; }
		public int userCount { get; set; }
		public int maxUser { get; set; }
	}
}
