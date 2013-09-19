using System;
using Newtonsoft.Json.Linq;

namespace Mono.CFoundry.Models
{
	public class Organization
	{
		public string Guid { get; set; }
		public string Name { get; set; }
		public string SpacesURL { get; set; }

		public static Organization FromJToken(JToken source) 
		{
			return new Organization () {
				Guid = source["metadata"]["guid"].ToString(),
				Name = source["entity"]["name"].ToString(),
				SpacesURL = source["entity"]["spaces_url"].ToString()
			};
		}
	}
}

