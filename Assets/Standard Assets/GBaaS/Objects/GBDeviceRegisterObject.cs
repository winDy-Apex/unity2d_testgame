using System;

namespace GBaaS.io.Objects
{
	public class GBDeviceRegisterObject : GBObject
	{
		public GBDeviceRegisterObject() {
			this.SetEntityType("devices");
		}

		public string deviceModel { get; set; }
		public string deviceOSVersion { get; set; }
		public string devicePlatform { get; set; }
		public string registration_id { get; set; }
	}
}
