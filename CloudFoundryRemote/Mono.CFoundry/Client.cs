using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using Mono.CFoundry.Models;

namespace Mono.CFoundry
{
	public class Client
	{
		string _target;
		string _uaaTarget;
		string _authToken = null;
		string _authType = null;
		string _refreshToken = null;
		DateTime _tokenExpiresAt = DateTime.Now;

		public Client ()
		{
			_target = "https://api.run.pivotal.io";
			GetEndPointInfo ();
		}

		public Client (string target)
		{
			_target = "https://" + target;
			GetEndPointInfo ();
		}

		public string AuthToken { get { return _authToken; } }

		public void Login(string username, string password)
		{
			string loginURL = _uaaTarget + "/oauth/token";
			WebHeaderCollection headers = new WebHeaderCollection ();

			headers.Add(HttpRequestHeader.Authorization, "Basic Y2Y6");

			var response = Post (loginURL, headers, "grant_type=password&username=" + username + "&password=" + password);
			SetTokenFromResponse (response);
		}

		public List<Organization> GetOrgs()
		{
			var orgs = new List<Organization> ();
			var response = Get(_target + "/v2/organizations?inline-relations-depth=0");
			foreach (var o in response["resources"]) orgs.Add (Organization.FromJToken(o));

			return orgs;
		}

		public List<Space> GetSpaces(string orgGuid)
		{
			var spaces = new List<Space> ();
			var response = Get (_target + "/v2/organizations/" + orgGuid + "/spaces");
			foreach (var s in response["resources"]) spaces.Add (Space.FromJToken (s));

			return spaces;
		}

		public List<App> GetApps (string spaceGuid)
		{
			var apps = new List<App> ();
			var response = Get (_target + "/v2/spaces/" + spaceGuid + "/summary");
			foreach (var a in response["apps"]) apps.Add (App.FromJToken (a));

			return apps;
		}

		public App GetApp (string guid)
		{
			var response = Get (_target + "/v2/apps/" + guid + "/summary");
			return App.FromJObject(response);
		}

		public List<InstanceStats> GetInstanceStats (string appGuid)
		{
			List<InstanceStats> instanceStats = new List<InstanceStats> ();
			var response = Get (_target + "/v2/apps/" +appGuid + "/stats");
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
			var response = Get(_target + "/info");
			_uaaTarget = response["authorization_endpoint"].ToString();
		}

		private void RefreshToken() 
		{
			string loginURL = _uaaTarget + "/oauth/token";
			WebHeaderCollection headers = new WebHeaderCollection ();

			headers.Add(HttpRequestHeader.Authorization, "Basic Y2Y6");

			var response = HttpRequest (loginURL, headers, "POST", "grant_type=refresh_token&refresh_token=" + _refreshToken);
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

		private JObject Get(string url, WebHeaderCollection headers = null) 
		{
			if ((DateTime.Now > _tokenExpiresAt) && (_refreshToken != null))
				RefreshToken ();

			if (headers == null)
				headers = new WebHeaderCollection ();
			if (_authToken != null)
				headers.Add (HttpRequestHeader.Authorization, _authType + " " + _authToken);

			return(HttpRequest(url, headers, "GET"));
		}

		private JObject Post(string url, WebHeaderCollection headers, string body)
		{
			if ((DateTime.Now > _tokenExpiresAt) && (_refreshToken != null))
				RefreshToken ();

			return(HttpRequest(url, headers, "POST", body));
		}

		private JObject Put(string url, string body, WebHeaderCollection headers = null)
		{
			if ((DateTime.Now > _tokenExpiresAt) && (_refreshToken != null))
				RefreshToken ();

			if (headers == null)
				headers = new WebHeaderCollection ();

			if (_authToken != null)
				headers.Add (HttpRequestHeader.Authorization, _authType + " " + _authToken);

			return(HttpRequest(url, headers, "PUT", body));
		}

		private JObject HttpRequest(string url, WebHeaderCollection headers, string method, string body = null)
		{

			WebRequest request = WebRequest.Create (url);
			request.Method = method;
			request.Headers = headers;

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
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

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

			var jObj = JObject.Parse (sb.ToString ());
			Console.WriteLine (jObj);
			return jObj;
		}
	}
}

