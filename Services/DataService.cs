using BlazorInterview.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

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
            List<IPCData> listOfIpcData = [];
            try
            {
                // Load the CSV data from the provided Uri and parse it into a list of IPCData objects
                // Use get request to get the csv file
                var csvData = await _httpClient.GetStreamAsync(csvFilePath);
                using var reader = new StreamReader(csvData);
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
                using CsvReader csv = new(reader, config);
                csv.Context.RegisterClassMap<IPCDataMap>();
                listOfIpcData = csv.GetRecords<IPCData>().ToList();
                NormalizeCpuMHz(listOfIpcData);
            }
            catch (Exception ex)
            {
                // handle the exception 
                Console.WriteLine($"An error occurred while loading data: {ex.Message}");
            }
            return listOfIpcData;
        }
        // clean data by removing outliers
        public static void CleanData(List<IPCData> ipcData)
        {
            var groupedData = ipcData.GroupBy(d => d.IPC);

            foreach (var group in groupedData)
            {
                var avgValue = group.Average(d => d.AvgValue);
                var q1 = group.OrderBy(d => d.AvgValue).ElementAt((int)(group.Count() * 0.25));
                var q3 = group.OrderByDescending(d => d.AvgValue).ElementAt((int)(group.Count() * 0.75));

                var iqr = q3.AvgValue - q1.AvgValue;
                var lowerBound = q1.AvgValue - 1.5 * iqr;
                var upperBound = q3.AvgValue + 1.5 * iqr;

                var cleanedData = group.Where(d => d.AvgValue >= lowerBound && d.AvgValue <= upperBound).ToList();

                ipcData.RemoveAll(d => d.IPC == group.Key);
                ipcData.AddRange(cleanedData);
            }
        }

        private static List<IPCData> RemoveOutliers(List<IPCData> data, Func<IPCData, double> selector)
        {
            // var values = data.Select(selector).ToList();
            // var mean = values.Average();
            // Console.WriteLine($"Mean: {mean}");
            // var stdDev = Math.Sqrt(values.Average(v => Math.Pow(v - mean, 2)));
            // Console.WriteLine($"Standard Deviation: {stdDev}");

            // // Define threshold for outlier
            // double threshold = 3.0; // Typical threshold for Z-score

            // return data.Where(d => Math.Abs(selector(d) - mean) / stdDev <= threshold).ToList();

            // group by IPC
            var groupedData = data.GroupBy(d => d.IPC);
            foreach (var group in groupedData)
            {
                var maxCpuMHz = group.Select(d => d.CpuMHz).Max();
                var values = group.Select(selector).ToList();
                // if a value is larger than 2 * maxCpuMHz, remove it
                var cleanedData = group.Where(d => selector(d) <= 2 * maxCpuMHz).ToList();
                data.RemoveAll(d => d.IPC == group.Key);
                data.AddRange(cleanedData);
            }
            return data;
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