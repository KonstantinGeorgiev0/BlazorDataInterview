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
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
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
    }
}