using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using GBaaS.io.Utils;

namespace GBaaS.io.Services
{
	/// <summary>
	/// 서버와 연결 정보를 가지고 있으며 서버 요청시 URL path 생성등을 담당한다.
	/// </summary>
	class GBRequestService : GBService<GBRequestService>
	{
		private string _gbaasUrl { get; set; }

		public GBRequestService() {}

		public void SetGBaaSUrl(string gbaasUrl) {
			this._gbaasUrl = gbaasUrl;
		}

		public string GetToken(string username, string password) {
			var reqString = string.Format("/token/?grant_type=password&username={0}&password={1}", username, password);
			var rawResults = HttpHelper.Instance.PerformGet(GBRequestService.Instance.BuildPath(reqString));

			if (rawResults.Length > 0) {
				var results = JObject.Parse (rawResults);
				return results ["access_token"].ToString ();
			} else {
				return "";
			}
		}

		public string LookUpToken(string token) {
			var reqString = "/users/me/?access_token=" + token;
			var rawResults = PerformRequest<string>(reqString);
			var entitiesResult = GetEntitiesFromJson(rawResults);

			return entitiesResult[0]["username"].ToString();
		}

		/// <summary>
		/// Performs a Get agianst the UserGridUrl + provided path
		/// </summary>
		/// <typeparam name="retrunT">Return Type</typeparam>
		/// <param name="path">Sub Path Of the Get Request</param>
		/// <returns>Object of Type T</returns>
		public retrunT PerformRequest<retrunT>(string path)
		{
			return PerformRequest<retrunT>(path, HttpHelper.RequestTypes.Get, null);
		}

		/// <summary>
		/// Performs a Request agianst the UserGridUrl + provided path
		/// </summary>
		/// <typeparam name="retrunT">Return Type</typeparam>
		/// <param name="path">Sub Path Of the Get Request</param>
		/// <returns>Object of Type T</returns>
		public retrunT PerformRequest<retrunT>(string path, HttpHelper.RequestTypes method, object data, string jsonParent = "")
		{
			string requestPath = BuildPath(path);
			return HttpHelper.Instance.PerformJsonRequest<retrunT>(requestPath, method, data, jsonParent);
		}

		public JToken GetEntitiesFromJson(string rawJson) {
			if (string.IsNullOrEmpty(rawJson) != true) {
				var objResult = JObject.Parse(rawJson);
				return objResult.SelectToken("entities");
			}
			return null;
		}

		// Get Cursor String (for continuous Query)
		public string GetValueFromJson(string key, string rawJson) {
			if (string.IsNullOrEmpty(rawJson) != true) {
				var objResult = JObject.Parse(rawJson);
				JToken token = objResult.SelectToken(key);
				if (token != null) {
					return token.ToString();
				}
			}
			return "";
		}

		/// <summary>
		/// Combines The UserGridUrl abd a provided path - checking to emsure proper http formatting
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private string BuildPath(string path)
		{
			StringBuilder sbResult = new StringBuilder();
			sbResult.Append(this._gbaasUrl);

			if (this._gbaasUrl.EndsWith("/") != true)
			{
				sbResult.Append("/");
			}

			if (path.StartsWith("/"))
			{
				path = path.TrimStart('/');
			}

			sbResult.Append(path);

			return sbResult.ToString();
		}
	}
}
