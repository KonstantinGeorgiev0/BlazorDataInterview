using System.ComponentModel.DataAnnotations;

namespace ItilityInterview.Models
{
	public class IPCData(string ipc, int dataFactory, DateTime time, double avgValue, double minValue, double maxValue, string metricID, int cpuMHz)
    {
        public string IPC { get; set; } = ipc;
        public int DataFactory { get; set; } = dataFactory;
        public DateTime Time { get; set; } = time;
        public double AvgValue { get; set; } = avgValue;
        public double MinValue { get; set; } = minValue;
        public double MaxValue { get; set; } = maxValue;
        public string MetricID { get; set; } = metricID;
        public int CpuMHz { get; set; } = cpuMHz;
    }
}
