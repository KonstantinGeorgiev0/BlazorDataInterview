namespace BlazorInterview.Models
{
    public class IPCData()
    {
        public string IPC { get; set; }
        public int DataFactory { get; set; }
        public DateTime Time { get; set; }
        public double AvgValue { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public string MetricID { get; set; }
        public int CpuMHz { get; set; } 

        // Calculated properties for utilization rates
        public double AverageUtilizationRate => AvgValue / CpuMHz * 100;
        public double PeakUtilizationRate => MaxValue / CpuMHz * 100;

        public static int CleanCpuMHz(string cpuMHz)
        {
            // If white space is encountered, remove everything after it
            var cleanedCpuMHz = cpuMHz.Split()[0];
            return int.Parse(cleanedCpuMHz);
        }
        // if value is larger than the maxCpuMHz, remove it
        public static double CleanValues(double value, double maxCpuMHz)
        {
            return value <= maxCpuMHz ? value : 0;
        }
    }
}
