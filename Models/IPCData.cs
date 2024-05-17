using System.ComponentModel.DataAnnotations;

namespace ItilityInterview.Models
{
	public class IPCData
	{
		public string IPC { get; set; }
		public int DataFactory { get; set; }
		public DateTime Time { get; set; }
		public double AvgValue { get; set; }
		public double MinValue { get; set; }
		public double MaxValue { get; set; }
		public string MetricID { get; set; }
		public int CpuMHz { get; set; } 

		public IPCData(string ipc, int dataFactory, DateTime time, double avgValue, double minValue, double maxValue, string metricID, int cpuMHz)
		{
			IPC = ipc;
			DataFactory = dataFactory;
			Time = time;
			AvgValue = avgValue;
			MinValue = minValue;
			MaxValue = maxValue;
			MetricID = metricID;
			CpuMHz = cpuMHz;
		}
	}
}
