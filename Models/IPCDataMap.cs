using CsvHelper.Configuration;

namespace BlazorInterview.Models
/// <summary>
/// Map the CSV columns to the IPCData properties
/// </summary>
{
    public class IPCDataMap : ClassMap<IPCData>
    {
        public IPCDataMap()
        {
            Map(m => m.IPC).Name("IPC");
            Map(m => m.DataFactory).Name("Data Factory");
            Map(m => m.Time).Name("time").TypeConverterOption.Format("dd/MM/yyyy");
            Map(m => m.AvgValue).Name("AvgValue");
            Map(m => m.MinValue).Name("MinValue");
            Map(m => m.MaxValue).Name("MaxValue");
            Map(m => m.MetricID).Name("MetricId");
            Map(m => m.CpuMHz).Name("CpuMHz").Convert(args => IPCData.CleanCpuMHz(args.Row.GetField("CpuMHz")));
        }
    }
}