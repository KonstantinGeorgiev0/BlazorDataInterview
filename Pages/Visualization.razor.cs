using BlazorInterview.Models;
using BlazorInterview.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorInterview.Pages
{
    public partial class Visualization
    {
        private int Index = -1; //default value cannot be 0 -> first selectedindex is 0.
        public MudBlazor.ChartOptions Options = new();
        List<IPCData> listOfIpcData = [];
        List<IPCData> dataITLT = [];
        readonly List<ChartSeries> Series = [];
        readonly List<ChartSeries> Series2 = [];
        readonly string[] XAxisLabels = [];
        readonly int[] YAxisLabels = [];
        // create arrays to store utilization rates
        public double[] AvgValue = [];
        public double[] MaxValue = [];
        int maxCpuMHz = 0;
        readonly string IpcID = "";
        List<string> IpcList = [];
        public string selectedIPC { get; set; }
        readonly string csvFilePath = "sample-data/testcase_smart_applicator_V8.2_020823.zip.csv";

        protected override async Task OnInitializedAsync()
        {
            // load IPC data
            listOfIpcData = await DataService.LoadDataAsync(csvFilePath);
            // extract IPC IDs
            IpcList = listOfIpcData.Select(data => data.IPC).Distinct().ToList();
            // set default IPC
            selectedIPC = IpcList[0];
            // create a second visulization that has AvgValue and MaxValue of all IPCs with the same CPU MHz 5800
            var data5800 = listOfIpcData.Where(data => data.CpuMHz == 5800).ToList();
            Series2.Add(new ChartSeries { Name = "Average Utilization Rate", Data = data5800.Select(data => data.AvgValue).ToArray() });
            Series2.Add(new ChartSeries { Name = "Peak Utilization Rate", Data = data5800.Select(data => data.MaxValue).ToArray() });  
            
            // update visualization
            await UpdateVisualizationAsync(selectedIPC);
        }

        private async Task OnIpcSelectionChange(ChangeEventArgs e)
        {
            selectedIPC = e.Value.ToString();
            await UpdateVisualizationAsync(selectedIPC);
        }

        private async Task UpdateVisualizationAsync(string selectedIPC)
        {
            // filter data for selected IPC
            dataITLT = listOfIpcData.Where(data => data.IPC == selectedIPC).ToList();
            // extract average and peak utilization rates
            AvgValue = dataITLT.Select(data => data.AvgValue).ToArray();
            MaxValue = dataITLT.Select(data => data.MaxValue).ToArray();
            maxCpuMHz = dataITLT.Select(data => data.CpuMHz).Max();

            // update chart series
            Series.Clear();
            Series.Add(new ChartSeries { Name = "Average Utilization Rate", Data = AvgValue });
            Series.Add(new ChartSeries { Name = "Peak Utilization Rate", Data = MaxValue });
        }
    }
}