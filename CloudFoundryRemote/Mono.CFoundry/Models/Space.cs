using System;
using Newtonsoft.Json.Linq;

namespace Mono.CFoundry.Models
{
	public class Space
	{
		public string Guid { get; set; }
		public string Name { get; set; }
		public string AppsURL { get; set; }

		public Space ()
		{

		}

		public static Space FromJToken(JToken source) 
		{
			return new Space () {
				Guid = source["metadata"]["guid"].ToString(),
				Name = source["entity"]["name"].ToString()
			};
		}
	}
}

