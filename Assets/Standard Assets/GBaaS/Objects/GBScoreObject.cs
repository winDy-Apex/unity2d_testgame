using System;
using GBaaS.io.Services;
using GBaaS.io.Utils;

namespace GBaaS.io.Objects
{
	public class GBScoreObject : GBObject
	{
		public GBScoreObject() {
			this.SetEntityType("leaderboard");
		}

		public string displayName { get; set; }
		public string username { get; set; }
		public string stage { get; set; }
		public int score { get; set; }
		public string unit { get; set; }
		public int rank { get; set; }
	}
}
