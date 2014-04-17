using System;
using GBaaS.io.Services;
using GBaaS.io.Utils;

namespace GBaaS.io.Objects
{
	public class GBObject
	{
		private 	string _entityType 	= "";
		protected 	string _UUID 		= "";
		protected 	Newtonsoft.Json.Linq.JToken _jsonToken;

		public GBObject () {}

		public void SetUUID(string uuid) {
			_UUID = uuid;
		}

		public string GetUUID() {
			return _UUID;
		}

		public void SetJsonToken(Newtonsoft.Json.Linq.JToken jsonToken) {
			_jsonToken = jsonToken;
		}

		public Newtonsoft.Json.Linq.JToken GetJsonToken() {
			return _jsonToken;
		}

		public bool Save() { 
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/" + GetEntityType(), HttpHelper.RequestTypes.Post, this);
			return HttpHelper.Instance.CheckSuccess(rawResults);
		}

		public bool Update() { 
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/" + GetEntityType() + "/" + _UUID, HttpHelper.RequestTypes.Put, this);
			return HttpHelper.Instance.CheckSuccess(rawResults);
		}

		public bool Delete() { 
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/" + GetEntityType() + "/" + _UUID, HttpHelper.RequestTypes.Delete, null);
			return HttpHelper.Instance.CheckSuccess(rawResults);
		}

		public bool Load() { return false; }

		public void SetEntityType(string entityType) {
			_entityType = entityType;
		}

		public String GetEntityType() {
			if (_entityType.Length > 0) {
				return _entityType.ToLower();
			}

			try {
				string 	s 				= this.GetType().Name;
				string 	prefixString	= "GB";
				if(s.IndexOf(prefixString) < 0) {
					return s.ToLower(); // GB로 시작하지 않으면(Custom Object) 이름 그대로를 리턴한다.
				}
				int 	prefix 			= s.IndexOf(prefixString) + prefixString.Length;
				int 	postfix 		= s.IndexOf("Object");

				return s.Substring(prefix, postfix - prefix).ToLower();
			} catch {
				return "";
			}
		}

		public bool SetLocation(float latitude, float longitude) {
			GBLocationObject location = new GBLocationObject {
				latitude = latitude,
				longitude = longitude
			};

			var rawResults = GBRequestService.Instance.PerformRequest<string>("/" + GetEntityType() + "/" + GetUUID(), HttpHelper.RequestTypes.Put, location, "location");
			return HttpHelper.Instance.CheckSuccess(rawResults);
		}
	}
}
