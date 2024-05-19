using BlazorInterview.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace BlazorInterview.Services
{
    public class DataService
    {
        private readonly HttpClient _httpClient;

        public DataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<IPCData>> LoadDataAsync(string csvFilePath)
        {
            List<IPCData> listOfIpcData = new List<IPCData>();
            try
            {
                // Load the CSV data from the provided Uri and parse it into a list of IPCData objects
                // Use get request to get the csv file
                var csvData = await _httpClient.GetStreamAsync(csvFilePath);
                using (var reader = new StreamReader(csvData))
                {
                    var config = new CsvConfiguration(CultureInfo.GetCultureInfo("fr-FR"))
                    {
                        HasHeaderRecord = true,
                        HeaderValidated = null,
                        Delimiter = ";",
                        IgnoreBlankLines = true,
                        TrimOptions = TrimOptions.Trim,
                        BadDataFound = null,
                        PrepareHeaderForMatch = args => args.Header.ToLower(),
                    };
                    using (CsvReader csv = new CsvReader(reader, config))
                    {
                        csv.Context.RegisterClassMap<IPCDataMap>();
                        listOfIpcData = csv.GetRecords<IPCData>().ToList();
                        NormalizeCpuMHz(listOfIpcData);
                    }
                }
            }
            catch (Exception ex)
            {
                // handle the exception 
                Console.WriteLine($"An error occurred while loading data: {ex.Message}");
            }
            return listOfIpcData;
        }
        // normalize CpuMHz values
        public static void NormalizeCpuMHz(List<IPCData> ipcData)
        {
            // group the data by IPC
            var groupedData = ipcData.GroupBy(d => d.IPC);

            foreach (var group in groupedData)
            {
                // find the most frequent CpuMHz value for the IPC
                var mostFrequentCpuMHz = group
                    .GroupBy(d => d.CpuMHz)
                    .OrderByDescending(d => d.Count())
                    .First()
                    .Key;

                // update the CpuMHz value for each entry in the group
                foreach (var entry in group)
                {
                    entry.CpuMHz = mostFrequentCpuMHz;
                }
            }
        }
        public static bool CheckMetricIDs(List<IPCData> ipcDataList)
        {
            if (ipcDataList.Count == 0)
            {
                return false; 
            }
            // Check if all the entries have the same MetricID
            var firstMetricID = ipcDataList[0].MetricID;
            return ipcDataList.All(x => x.MetricID == firstMetricID);
        }
    }
}