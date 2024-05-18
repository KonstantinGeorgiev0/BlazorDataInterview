using BlazorInterview.Models;

namespace BlazorInterview.Services
{
    public class DataService(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<List<IPCData>> LoadDataAsync(string csvFilePath)
        {
            List<IPCData> listOfIpcData = [];

            try
            {
                string csvIpcData = await _httpClient.GetStringAsync(csvFilePath);
                List<string> dataRows = [.. csvIpcData.Split("\n")];

                for (int i = 0; i < dataRows.Count; i++)
                {
                    if (i > 0 && !string.IsNullOrWhiteSpace(dataRows[i])) // Skip the header and empty rows
                    {
                        List<string> ipcStringList = [.. dataRows[i].Split(";")];

                        IPCData ipcData = new(
                            ipcStringList[0],
                            int.Parse(ipcStringList[1]),
                            DateTime.Parse(ipcStringList[2]),
                            double.Parse(ipcStringList[3].Replace(',', '.')),
                            double.Parse(ipcStringList[4].Replace(',', '.')),
                            double.Parse(ipcStringList[5].Replace(',', '.')),
                            ipcStringList[6],
                            int.Parse(ipcStringList[7])
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
