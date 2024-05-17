using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ItilityInterview.Models;

namespace ItilityInterview.Services
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
                string csvIpcData = await _httpClient.GetStringAsync(csvFilePath);
                List<string> dataRows = csvIpcData.Split("\n").ToList();

                for (int i = 0; i < dataRows.Count; i++)
                {
                    if (i > 0 && !string.IsNullOrWhiteSpace(dataRows[i])) // Skip the header and empty rows
                    {
                        List<string> ipcStringList = dataRows[i].Split(";").ToList();

                        IPCData ipcData = new IPCData(
                            ipcStringList[0],
                            Int32.Parse(ipcStringList[1]),
                            DateTime.Parse(ipcStringList[2]),
                            double.Parse(ipcStringList[3].Replace(',', '.')),
                            double.Parse(ipcStringList[4].Replace(',', '.')),
                            double.Parse(ipcStringList[5].Replace(',', '.')),
                            ipcStringList[6],
                            Int32.Parse(ipcStringList[7])
                        );

                        listOfIpcData.Add(ipcData);
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
