using System;

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
	}
}

