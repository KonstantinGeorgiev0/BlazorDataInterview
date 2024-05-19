namespace BlazorInterview.Models
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public class IPCData()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
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

        public static int ParseCpuMHz(string cpuMHz)
        {
            // Clean and parse the CpuMHz field
            var cleanValue = cpuMHz.Split(' ').FirstOrDefault(v => int.TryParse(v, out _));
            return int.TryParse(cleanValue, out int result) ? result : 0;
        }
    }
}
