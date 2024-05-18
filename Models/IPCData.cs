namespace BlazorInterview.Models
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

        // Calculated properties for utilization rates
        public double AverageUtilizationRate => (AvgValue / CpuMHz) * 100;
        public double PeakUtilizationRate => (MaxValue / CpuMHz) * 100;
    }
}
