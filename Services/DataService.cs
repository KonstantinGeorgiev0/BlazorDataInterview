using BlazorInterview.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace BlazorInterview.Services
{
    /// <summary>
    /// Service class for loading and manipulating IPCData.
    /// </summary>
    public class DataService
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance used for making HTTP requests.</param>
        public DataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Loads the IPCData from a CSV file asynchronously.
        /// </summary>
        /// <param name="csvFilePath">The path to the CSV file.</param>
        /// <returns>A list of IPCData objects.</returns>
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

        /// <summary>
        /// Normalizes the CpuMHz values for each IPCData object in the list.
        /// </summary>
        /// <param name="ipcData">The list of IPCData objects.</param>
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

        /// <summary>
        /// Checks if all the IPCData objects in the list have the same MetricID.
        /// </summary>
        /// <param name="ipcDataList">The list of IPCData objects.</param>
        /// <returns>True if all the entries have the same MetricID, otherwise false.</returns>
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