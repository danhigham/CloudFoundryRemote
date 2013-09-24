using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Mono.CFoundry.Models
{
	public class App
	{
		public string Guid { get; set; }
		public string Name { get; set; }
		public string[] Urls { get; set; }
		public int Memory { get; set; }
		public int Instances { get; set; }
		public int DiskQuota { get; set; }
		public string State { get; set; }
		public string DetectedBuildpack { get; set; }

		public static App FromJToken(JToken source) 
		{
			App app = new App () {
				Name = source["name"].ToString(),
				Memory = int.Parse(source["memory"].ToString()),
				Instances = int.Parse(source["instances"].ToString()),
				DiskQuota = int.Parse(source["disk_quota"].ToString()),
				State = source["state"].ToString(),
				DetectedBuildpack = source["detected_buildpack"].ToString()
			};

			if (source ["guid"] != null)
				app.Guid = source ["guid"].ToString ();

			if (source ["urls"] != null)
				app.Urls = source ["urls"].ToObject<string[]> ();

			return app;
		}

		public static App FromJObject(JObject source)
		{
			App app = new App () {
				Guid = source["guid"].ToString(),
				Name = source["name"].ToString(),
//				Urls = source["urls"].ToObject<string[]>(),
				Memory = int.Parse(source["memory"].ToString()),
				Instances = int.Parse(source["instances"].ToString()),
				DiskQuota = int.Parse(source["disk_quota"].ToString()),
				State = source["state"].ToString(),
				DetectedBuildpack = source["detected_buildpack"].ToString()
			};

			List<string> urls = new List<string> ();

			foreach (var route in source["routes"]) 
				urls.Add (route ["host"] + "." + route ["domain"] ["name"]);

			app.Urls = urls.ToArray ();
			return app;
		}
	}
}