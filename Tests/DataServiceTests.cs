using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using Xunit;
using Moq;
using BlazorInterview.Services;
using BlazorInterview.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Globalization;

namespace BlazorInterview.Tests;

public class DataServiceTests 
{
    private readonly Mock<HttpClient> _httpClientMock;
    private readonly DataService _dataService;

    public DataServiceTests()
    {
        _httpClientMock = new Mock<HttpClient>();
        _dataService = new DataService(_httpClientMock.Object);
    }

    [Fact]
    public async Task LoadDataAsync_ShouldReturnListOfIPCData()
    {
        // Arrange
        var expectedData = new List<IPCData>
        {
            new IPCData { 
                IPC = "TestIPC",
                DataFactory = 1,
                Time = new DateTime(2024, 6, 4),
                AvgValue = 1500.05,
                MaxValue = 2500.55,
                MinValue = 1000.14,
                MetricID = "TestMetricID",
                CpuMHz = 2500 },

            // new IPCData {
            //     IPC = "TestIPCAvgLess",
            //     DataFactory = 1,
            //     Time = new DateTime(2024, 6, 5),
            //     AvgValue = 900.05,
            //     MaxValue = 2500.55,
            //     MinValue = 1000.14,
            //     MetricID = "TestMetricID",
            //     CpuMHz = 2500 },

            // new IPCData {
            //     IPC = "TestIPCAvgMore",
            //     DataFactory = 1,
            //     Time = new DateTime(2024, 6, 6),
            //     AvgValue = 2500.05,
            //     MaxValue = 2000.55,
            //     MinValue = 1000.14,
            //     MetricID = "TestMetricID",
            //     CpuMHz = 2500 },

            // new IPCData {
            //     IPC = "TestIPCMaxMore",
            //     DataFactory = 1,
            //     Time = new DateTime(2024, 6, 7),
            //     AvgValue = 1500.05,
            //     MaxValue = 2500.55,
            //     MinValue = 1000.14,
            //     MetricID = "TestMetricID",
            //     CpuMHz = 2000 },
        };
        // Convert expectedData to CSV format
        var csvContent = new StringBuilder();
        var writer = new StringWriter(csvContent);
        var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csvWriter.WriteRecords(expectedData);
        csvWriter.Flush();

        // Create a MemoryStream from the CSV content
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent.ToString()));

        // Setup the HttpClient mock to return the stream when GetStreamAsync is called
        _httpClientMock.Setup(client => client.GetStreamAsync(It.IsAny<string>())).ReturnsAsync(stream);

        // Act
        var result = await _dataService.LoadDataAsync("test-path"); // Keep "test-path" as is

        // Assert
        Assert.Equal(expectedData, result);
    }
}