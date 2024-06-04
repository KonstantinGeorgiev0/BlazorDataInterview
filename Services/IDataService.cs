using BlazorInterview.Models;

namespace BlazorInterview.Services;

public interface IDataService
{
    Task<List<IPCData>> LoadDataAsync(string csvFilePath);
    bool CheckForWrongData(List<IPCData> ipcData);
    List<IPCData> RemoveMissingData(List<IPCData> ipcData);
    List<IPCData> RemoveIPCsBasedOnTimePeriod(List<IPCData> ipcData, int minTime);
    List<IPCData> RemoveAvgValues(List<IPCData> ipcData);
    void NormalizeCpuMHz(List<IPCData> ipcData);
    bool CheckMetricIDs(List<IPCData> ipcData);
}