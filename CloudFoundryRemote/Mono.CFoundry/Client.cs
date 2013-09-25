using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Mono.CFoundry.Models;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Mono.CFoundry
{
	public class HttpErrorEventArgs : EventArgs
	{
		public string Message { get; private set; }

		public HttpErrorEventArgs(string message)
		{
			Message = message;
		}
	}

	public class Client
	{
		string _target;
		string _uaaTarget;
		string _authToken = null;
		string _authType = null;
		string _refreshToken = null;
		bool _trustAll = false;
		DateTime _tokenExpiresAt = DateTime.Now;

		public delegate void HttpErrorHandler(object sender, HttpErrorEventArgs e);
		public event HttpErrorHandler OnHttpError;

		public Client ()
		{
			_target = "https://api.run.pivotal.io";
			GetEndPointInfo ();
		}

		public Client (string target, bool trustAll = false)
		{
			_target = "https://" + target;
			_trustAll = trustAll;
			GetEndPointInfo ();
		}

		public string AuthToken { get { return _authToken; } }

		public bool Login(string username, string password)
		{
			string loginURL = _uaaTarget + "/oauth/token";
			WebHeaderCollection headers = new WebHeaderCollection ();

			headers.Add(HttpRequestHeader.Authorization, "Basic Y2Y6");

			var response = Post (loginURL, headers, "grant_type=password&username=" + username + "&password=" + password);

			if (response ["error"] != null)
				return false;

			SetTokenFromResponse (response);

			return true;
		}

		public List<Organization> GetOrgs()
		{
			var orgs = new List<Organization> ();
			var response = Get<JObject>(_target + "/v2/organizations?inline-relations-depth=0");
			foreach (var o in response["resources"]) orgs.Add (Organization.FromJToken(o));

			return orgs;
		}

		public string[] GetFolder(string appGuid, string path)
		{
			var response = Get<string> (_target + "/v2/apps/" + appGuid + "/instances/0/files" + path);

			List<string> entries = new List<string> ();

			foreach (Match match in Regex.Matches(response, @"([^\s]+)(.+)"))
				entries.Add (match.Groups [1].ToString ());

			return entries.ToArray();
		}


		public string GetFile(string appGuid, string path)
		{
			return Get<string> (_target + "/v2/apps/" + appGuid + "/instances/0/files" + path);
		}

		public List<Space> GetSpaces(string orgGuid)
		{
			var spaces = new List<Space> ();
			var response = Get<JObject> (_target + "/v2/organizations/" + orgGuid + "/spaces");
			foreach (var s in response["resources"]) spaces.Add (Space.FromJToken (s));

			return spaces;
		}

		public List<App> GetApps (string spaceGuid)
		{
			var apps = new List<App> ();
			var response = Get<JObject> (_target + "/v2/spaces/" + spaceGuid + "/summary");
			foreach (var a in response["apps"]) apps.Add (App.FromJToken (a));

			return apps;
		}

		public App GetApp (string guid)
		{
			var response = Get<JObject> (_target + "/v2/apps/" + guid + "/summary");
			return App.FromJObject(response);
		}

		public List<InstanceStats> GetInstanceStats (string appGuid)
		{
			List<InstanceStats> instanceStats = new List<InstanceStats> ();
			var response = Get<JObject> (_target + "/v2/apps/" +appGuid + "/stats");
			foreach (var instance in response) instanceStats.Add (InstanceStats.FromJTokenHash (instance));

			return instanceStats;
		}

		public JObject Scale(string appGuid, int memory, int instances)
		{
			string requestBody = "{\"instances\":" + instances + ",\"memory\":" + memory + "\"}";
			string url = _target + "/v2/apps/" + appGuid;

			return Put (url, requestBody);
		}

		public JObject Stop(string appGuid)
		{
			string requestBody = "{\"state\":\"STOPPED\"}";
			string url = _target + "/v2/apps/" + appGuid;

			return Put (url, requestBody);
		}

		public JObject Start(string appGuid)
		{
			string requestBody = "{\"console\":true,\"state\":\"STARTED\"}";
			string url = _target + "/v2/apps/" + appGuid;

			return Put (url, requestBody);
		}

		private void GetEndPointInfo() 
		{
			var response = Get<JObject> (_target + "/info");
			_uaaTarget = response["authorization_endpoint"].ToString();
		}

		private void RefreshToken() 
		{
			string loginURL = _uaaTarget + "/oauth/token";
			WebHeaderCollection headers = new WebHeaderCollection ();

			headers.Add(HttpRequestHeader.Authorization, "Basic Y2Y6");

			var response = HttpRequest<JObject> (loginURL, headers, "POST", "grant_type=refresh_token&refresh_token=" + _refreshToken);
			SetTokenFromResponse (response);
		}

		private void SetTokenFromResponse(JObject response)
		{
			_authToken = response ["access_token"].ToString ();
			_refreshToken = response ["refresh_token"].ToString ();
			_authType = response ["token_type"].ToString ();

			int expiresInSeconds = int.Parse (response ["expires_in"].ToString ());
			_tokenExpiresAt = DateTime.Now.AddSeconds (expiresInSeconds);

			Console.WriteLine ("Current token expires at " + _tokenExpiresAt);
		}

		private T Get<T>(string url, WebHeaderCollection headers = null) 
		{
			if ((DateTime.Now > _tokenExpiresAt) && (_refreshToken != null))
				RefreshToken ();

			if (headers == null)
				headers = new WebHeaderCollection ();
			if (_authToken != null)
				headers.Add (HttpRequestHeader.Authorization, _authType + " " + _authToken);

			return(HttpRequest<T>(url, headers, "GET"));
		}

		private JObject Post(string url, WebHeaderCollection headers, string body)
		{
			if ((DateTime.Now > _tokenExpiresAt) && (_refreshToken != null))
				RefreshToken ();

			return(HttpRequest<JObject>(url, headers, "POST", body));
		}

		private JObject Put(string url, string body, WebHeaderCollection headers = null)
		{
			if ((DateTime.Now > _tokenExpiresAt) && (_refreshToken != null))
				RefreshToken ();

			if (headers == null)
				headers = new WebHeaderCollection ();

			if (_authToken != null)
				headers.Add (HttpRequestHeader.Authorization, _authType + " " + _authToken);

			return(HttpRequest<JObject>(url, headers, "PUT", body));
		}

		private T HttpRequest<T>(string url, WebHeaderCollection headers, string method, string body = null)
		{

			WebRequest request = WebRequest.Create (url);
			request.Method = method;
			request.Headers = headers;

			ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;

			if ((method == "POST") || (method == "PUT")) {

				ASCIIEncoding encoding = new ASCIIEncoding ();
				byte[] byte1 = encoding.GetBytes (body);

				request.ContentType = "application/x-www-form-urlencoded";
				request.ContentLength = byte1.Length;

				Stream newStream = request.GetRequestStream ();

				newStream.Write (byte1, 0, byte1.Length);
				newStream.Close ();
			}

			Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

			HttpWebResponse response = null;
			try {
				response = (HttpWebResponse)request.GetResponse();
			}
			catch (System.Net.WebException e) {
				HttpErrorEventArgs args = new HttpErrorEventArgs (e.Message);
				if (OnHttpError != null)
					OnHttpError (this, args);

				if (typeof(T) == typeof(JObject)) {
					return (T)(object)JObject.Parse ("{error: \"" + e.Message + "\"}");
				} else {
					return (T)(object)e.Message;
				}
			}

			// Pipe the stream to a higher level stream reader with the required encoding format. 
			StreamReader readStream = new StreamReader( response.GetResponseStream(), encode );
			Char[] read = new Char[256];

			// Read 256 charcters at a time.     
			int count = readStream.Read( read, 0, 256 );
			StringBuilder sb = new StringBuilder ();

			while (count > 0) 
			{
				sb.Append(new String(read, 0, count));
				count = readStream.Read(read, 0, 256);
			}

			readStream.Close();
			response.Close(); 

			if (typeof(T) == typeof(JObject)) {
				return (T)(object)JObject.Parse (sb.ToString ());
			} else {
				return (T)(object)sb.ToString();
			}
		}

		private bool ValidateServerCertificate(
			object sender,
			X509Certificate certificate,
			X509Chain chain,
			SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None)
				return true;

			Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

			// Do not allow this client to communicate with unauthenticated servers. 
			return _trustAll;
		}
	}
}

