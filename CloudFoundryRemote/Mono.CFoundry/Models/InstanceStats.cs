using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Mono.CFoundry.Models
{
	public class InstanceStats
	{
		public string InstanceId { get; set; }
		public string State { get; set; }
		public string Host { get; set; }
		public int Port { get; set; }
		public int Uptime { get; set; }
		public int MemoryQuota { get; set; }
		public int DiskQuota { get; set; }
		public int FDSQuota { get; set; }
		public string TimeUsage { get; set; }
		public float CPUUsage { get; set; }
		public int MemoryUsage { get; set; }
		public int DiskUsage { get; set; }

		public static InstanceStats FromJTokenHash(KeyValuePair<string, JToken> source) {

			var instanceStat = new InstanceStats () {
				InstanceId = source.Key,
				State = source.Value["state"].ToString()
			};

			if (instanceStat.State == "RUNNING") {
				instanceStat.Host = source.Value ["stats"] ["host"].ToString ();
				instanceStat.Port = int.Parse (source.Value ["stats"] ["port"].ToString ());
				instanceStat.Uptime = int.Parse (source.Value ["stats"] ["uptime"].ToString ());
				instanceStat.MemoryQuota = int.Parse (source.Value ["stats"] ["mem_quota"].ToString ());
				instanceStat.DiskQuota = int.Parse (source.Value ["stats"] ["disk_quota"].ToString ());
				instanceStat.FDSQuota = int.Parse (source.Value ["stats"] ["fds_quota"].ToString ());
				instanceStat.TimeUsage = source.Value ["stats"] ["usage"] ["time"].ToString ();
				instanceStat.CPUUsage = float.Parse (source.Value ["stats"] ["usage"] ["cpu"].ToString ());
				instanceStat.MemoryUsage = int.Parse (source.Value ["stats"] ["usage"] ["mem"].ToString ());
				instanceStat.DiskUsage = int.Parse (source.Value ["stats"] ["usage"] ["disk"].ToString ());
			}

			return instanceStat;
		}
	}
}

