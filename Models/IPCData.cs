using System.ComponentModel.DataAnnotations;

namespace ItilityInterview.Models
{
	public class IPCData
	{
		[Required]
		[Key]
		public string IPC { get; set; }
		public int DataFactory { get; set; }
		public DateTime Time { get; set; }
		public double AvgValue { get; set; }
		public double MinValue { get; set; }
		public double MaxValue { get; set; }
		[Required]
		public string MetricID { get; set; }
		public int CpuMHz { get; set; } 
	}
}
