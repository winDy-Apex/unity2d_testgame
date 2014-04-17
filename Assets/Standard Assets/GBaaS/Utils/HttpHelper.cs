using Krystalware.UploadHelper;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace GBaaS.io.Utils
{
	class HttpHelper : Singleton<HttpHelper>
	{
		public enum RequestTypes { Get, Post, Put, Delete }
		
		public string _accessToken { get; set; }

		#region Get
		public string PerformGet(string url)
		{
			try {
				WebRequest req = WebRequest.Create(url);
				WebResponse resp = req.GetResponse();
				StreamReader sr = new StreamReader(resp.GetResponseStream());
				return sr.ReadToEnd().Trim();
			} catch { // HTTP Call Fail or No Result Set
				return "";
			}
		}
		#endregion

		#region Post

		public ReturnT PerformPost<ReturnT>(string url)
		{
			return PerformPost<object, ReturnT>(url, new object());
		}

		public ReturnT PerformPost<PostT, ReturnT>(string url, PostT postData)
		{
			return PerformPost<PostT, ReturnT>(url, postData, new NameValueCollection());
		}

		public ReturnT PerformPost<PostT, ReturnT>(string url, PostT postData, NameValueCollection files)
		{
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

			NameValueCollection nvpPost;
			if (typeof(PostT) == typeof(NameValueCollection))
			{
				nvpPost = postData as NameValueCollection;
			}
			else
			{
				nvpPost = ObjectToNameValueCollection<PostT>(postData);
			}

			List<UploadFile> postFiles = new List<UploadFile>();
			foreach (var fKey in files.AllKeys)
			{
				FileStream fs = File.OpenRead(files[fKey]);
				postFiles.Add(new UploadFile(fs, fKey, files[fKey], "application/octet-stream"));
			}

			var response = HttpUploadHelper.Upload(req, postFiles.ToArray(), nvpPost);

			using (Stream s = response.GetResponseStream())
				using (StreamReader sr = new StreamReader(s))
			{
				var responseJson = sr.ReadToEnd();
				if (typeof(ReturnT) == typeof(string))
				{
					return (ReturnT)Convert.ChangeType(responseJson, typeof(ReturnT));
				}

				return fastJSON.JSON.Instance.ToObject<ReturnT>(responseJson);
			}
		}

		//Converts an object to a name value collection (for posts)
		private NameValueCollection ObjectToNameValueCollection<T>(T obj)
		{
			NameValueCollection results = new NameValueCollection();

			var oType = typeof(T);
			foreach (var prop in oType.GetProperties())
			{
				string pVal = "";
				try
				{
					pVal = oType.GetProperty(prop.Name).GetValue(obj, null).ToString();
				}
				catch { }
				results[prop.Name] = pVal;
			}

			return results;
		}

		#endregion

		#region JSON Request

		public ReturnT PerformJsonRequest<ReturnT>(string url, RequestTypes method, object postData, string jsonParent = "")
		{
			//Initilize the http request
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
			req.ContentType = "application/json";
			req.Method = Enum.GetName(typeof(RequestTypes), method).ToUpper();
			if (_accessToken != null && _accessToken.Length > 0) {
				req.Headers.Add ("Authorization: Bearer " + _accessToken);
			}

			//If posting data - serialize it to a json object
			if (method != RequestTypes.Get && postData != null)
			{
				StringBuilder sbJsonRequest = new StringBuilder();
				var T = postData.GetType();
				foreach (var prop in T.GetProperties())
				{
					if (NativeTypes.Contains(prop.PropertyType) && prop.GetValue(postData, null) != null)
					{
						//Console.Out.WriteLine(prop.PropertyType.ToString());
						if (prop.PropertyType.ToString().CompareTo("System.Boolean") == 0) {
							object value = prop.GetValue(postData, null);
							String valueString = "";
							if (value != null) {
								valueString = value.ToString().ToLower();
							}

							sbJsonRequest.AppendFormat("\"{0}\":{1},", prop.Name, valueString);
						} else if (prop.PropertyType.ToString().CompareTo("System.Int32") == 0 || prop.PropertyType.ToString().CompareTo("System.Single") == 0) {
							//sbJsonRequest.AppendFormat("\"{0}\":{1},", prop.Name.ToLower(), prop.GetValue(postData, null));
							sbJsonRequest.AppendFormat("\"{0}\":{1},", prop.Name, prop.GetValue(postData, null));
						} else {
							//sbJsonRequest.AppendFormat("\"{0}\":\"{1}\",", prop.Name.ToLower(), prop.GetValue(postData, null));
							sbJsonRequest.AppendFormat("\"{0}\":\"{1}\",", prop.Name, prop.GetValue(postData, null));
						}
					}
				}

				using (var sWriter = new StreamWriter(req.GetRequestStream()))
				{
					if (jsonParent.Length > 0) {
						sWriter.Write("{\"" + jsonParent + "\": {" + sbJsonRequest.ToString().TrimEnd(',') + "} }");
					} else {
						sWriter.Write("{" + sbJsonRequest.ToString().TrimEnd(',') + "}");
					}
				}
			}

			//Submit the Http Request
			string responseJson = "";
			try
			{
				using (var wResponse = req.GetResponse())
				{
					StreamReader sReader = new StreamReader(wResponse.GetResponseStream());
					responseJson = sReader.ReadToEnd();
				}
			}
			catch (WebException ex)
			{
				using (WebResponse response = ex.Response)
				{
					StreamReader sReader = new StreamReader(response.GetResponseStream());
					responseJson = sReader.ReadToEnd();
				}
			}

			if (typeof(ReturnT) == typeof(string))
			{
				return (ReturnT)Convert.ChangeType(responseJson, typeof(ReturnT));
			}

			return fastJSON.JSON.Instance.ToObject<ReturnT>(responseJson);
		}

		//Approved Types for serialization
		public List<Type> NativeTypes
		{
			get
			{
				var approvedTypes = new List<Type>();

				approvedTypes.Add(typeof(int));
				approvedTypes.Add(typeof(Int32));
				approvedTypes.Add(typeof(Int64));
				approvedTypes.Add(typeof(string));
				approvedTypes.Add(typeof(DateTime));
				approvedTypes.Add(typeof(double));
				approvedTypes.Add(typeof(decimal));
				approvedTypes.Add(typeof(float));
				approvedTypes.Add(typeof(List<>));
				approvedTypes.Add(typeof(bool));
				approvedTypes.Add(typeof(Boolean));

				approvedTypes.Add(typeof(int?));
				approvedTypes.Add(typeof(Int32?));
				approvedTypes.Add(typeof(Int64?));
				approvedTypes.Add(typeof(DateTime?));
				approvedTypes.Add(typeof(double?));
				approvedTypes.Add(typeof(decimal?));
				approvedTypes.Add(typeof(float?));
				approvedTypes.Add(typeof(bool?));
				approvedTypes.Add(typeof(Boolean?));

				return approvedTypes;
			}
		}

		#endregion

		public bool CheckSuccess(string result) {
			return (result.IndexOf ("error") == -1);
		}

		public bool SafeConvertBoolean(string booleanString) {
			return booleanString.ToLower().Equals("true");
		}
	}
}
