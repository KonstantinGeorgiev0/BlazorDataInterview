using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using ItilityInterview.Models;

namespace ItilityInterview.Services
{
    public class DataService(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;

        // Loads data from a CSV file asynchronously
        public async Task<List<IPCData>> LoadDataAsync(string csvFilePath)
        {
            List<IPCData> ipcData = new List<IPCData>();

            try {
                // Get the CSV file stream from the provided URL
                var csvStream = await _httpClient.GetStreamAsync(csvFilePath);

                // Read the CSV file using CsvHelper library
                using var reader = new StreamReader(csvStream);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    BadDataFound = null,
                    HasHeaderRecord = true // skip the header row
                });

                // Skip the header row
                csv.Read();
                csv.ReadHeader();

                // Read the remaining rows and convert them to IPCData objects
                while (await csv.ReadAsync())
                {
                    var record = csv.GetRecord<IPCData>();
                    ipcData.Add(record);
                }

                // // Skip the first row (header) and convert the remaining CSV records to a list of PizzaAnalyzer objects
                // ipcData = csv.GetRecords<IPCData>().Skip(1).ToList();
                // Clean the data by removing invalid or inconsistent rows
                CleanData(ipcData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
            return ipcData;
        }
        private static void CleanData(List<IPCData> ipcData)
        {   
            // remove rows with missing values
            ipcData.RemoveAll(p => p.IPC == null || p.MetricID == null);
            ipcData.RemoveAll(p => p.AvgValue == 0 || p.MinValue == 0 || p.MaxValue == 0 || p.CpuMHz == 0);

            // ensure logical consistency
            ipcData.RemoveAll(p => p.AvgValue < p.MinValue || p.AvgValue > p.MaxValue);

            // remove rows with invalid time
            ipcData.RemoveAll(p => p.Time == DateTime.MinValue);

            // remove outliers
            int threshold = ipcData.Max(p => p.CpuMHz);
            ipcData.RemoveAll(p => p.AvgValue > threshold || p.MinValue > threshold || p.MaxValue > threshold);
        }
    }
}
