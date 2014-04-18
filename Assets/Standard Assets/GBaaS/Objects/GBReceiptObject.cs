using System;
using GBaaS.io.Services;
using GBaaS.io.Utils;

namespace GBaaS.io.Objects
{
	public class GBReceiptObject : GBObject
	{
		public string receiptCode { get; set; }
		public string receiptType { get; set; }
		public string userDID { get; set; }
		public string dayToUse { get; set; }
	}
}
