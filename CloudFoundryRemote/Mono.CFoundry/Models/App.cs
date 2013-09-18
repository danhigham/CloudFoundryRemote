using System;

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

		public App ()
		{
		}
	}
}


//	"guid": "bd6fe78a-0cfe-4d4d-b394-5b2675f220b2",
//	"urls": [
//	    "blog.high.am",
//	    "high.am"
//	    ],
//	"routes": [
//	    {
//		"guid": "c7f8873f-add5-41cf-bcb8-7f0bca7e153e",
//		"host": "blog",
//		"domain": {
//			"guid": "bb47dd93-2108-4c02-ad7a-1db55d748a67",
//			"name": "high.am"
//		}
//	    },
//	    {
//		"guid": "e9ec9eed-5caf-4827-aada-48d55c7acced",
//		"host": "",
//		"domain": {
//			"guid": "bb47dd93-2108-4c02-ad7a-1db55d748a67",
//			"name": "high.am"
//		}
//	    }
//	    ],
//	"service_count": 0,
//	"service_names": [],
//	"running_instances": 1,
//	"name": "blog",
//	"production": false,
//	"space_guid": "7e84e041-153f-4fe9-ab4b-2462cef86263",
//	"stack_guid": "50688ae5-9bfc-4bf6-a4bf-caadb21a32c6",
//	"buildpack": null,
//	"detected_buildpack": "Ruby/Rack",
//	"environment_json": {},
//	"memory": 128,
//	"instances": 1,
//	"disk_quota": 1024,
//	"state": "STARTED",
//	"version": "fa706051-f0c3-43c6-80cb-117f35813e06",
//	"command": null,
//	"console": true,
//	"debug": null,
//	"staging_task_id": "b21c5d491effdf495eda1f77b5bb0257"
