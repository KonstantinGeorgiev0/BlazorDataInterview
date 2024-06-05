using BlazorInterview.Models;
using BlazorInterview.Services;
using CsvHelper;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Moq.Protected;

namespace BlazorInterview.Tests;

public class Tests
{
    private Mock<HttpClient> _httpClientMock;
    private DataService _dataService;

    /// <summary>
    /// Sets up the test environment before each test case.
    /// Create a mock version of HttpClient and pass it to the DataService constructor.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _httpClientMock = new Mock<HttpClient>();
        _dataService = new DataService(_httpClientMock.Object);
    }

    [Test]
    public async Task LoadDataAsync_ShouldReturnListOfIPCData()
    {
        // Arrange
        var expectedData = new List<IPCData>
        {
            new() {
                IPC = "TestIPC",
                DataFactory = 1,
                AvgValue = 1500,
                MaxValue = 2200,
                MinValue = 1000,
                MetricID = "TestMetricID",
                CpuMHz = 2500
            }
        };

        // string to be returned from the HttpClient
        var csvData = "IPC;DataFactory;AvgValue;MaxValue;MinValue;MetricID;CpuMHz\n" + 
                            "TestIPC;1;1500.05;2500.55;1000.14;TestMetricID;2500";
        // create a MemoryStream from the CSV data
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));
        // create a HttpResponseMessage with the MemoryStream as content
        var httpResponse = new HttpResponseMessage
        { 
            StatusCode = HttpStatusCode.OK,
            Content = new StreamContent(stream) 
        };

        // _httpClientMock
        //     .Protected()
        //     .Setup<Task<HttpResponseMessage>>(
        //           "SendAsync",
        //           ItExpr.IsAny<HttpRequestMessage>(),
        //           ItExpr.IsAny<CancellationToken>())
        //    .ReturnsAsync(httpResponse);

        // Mock the SendAsync method to return the HttpResponseMessage
        _httpClientMock
            .Setup(handler => handler.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(httpResponse);

        // // Mock the GetStreamAsync method to return the MemoryStream containing the CSV data
        // _httpClientMock
        //     .Setup(handler => handler.GetStreamAsync(It.IsAny<string>()))
        //     .ReturnsAsync(() => stream);

        // _httpMessageHandlerMock
        //     .Protected()
        //     .Setup<Task<HttpResponseMessage>>(
        //         "SendAsync",
        //         ItExpr.IsAny<HttpRequestMessage>(),
        //         ItExpr.IsAny<CancellationToken>()
        //     )
        //     .ReturnsAsync(response);

        // Act
        var result = await _dataService.LoadDataAsync("test.csv");
        // Print the result 
        Console.WriteLine("Result:\n");
        foreach (var ipcData in result)
        {
            Console.WriteLine($"IPC: {ipcData.IPC}, DataFactory: {ipcData.DataFactory}, AvgValue: {ipcData.AvgValue}, " +
                            $"MaxValue: {ipcData.MaxValue}, MinValue: {ipcData.MinValue}, MetricID: {ipcData.MetricID}, " +
                            $"CpuMHz: {ipcData.CpuMHz}\n");
        }

        // Assert
        Assert.That(result, Is.Not.Null);
        // Assert.That(result, Is.EquivalentTo(expectedData));
    }

    /// <summary>
    /// Tests the CheckForWrongData method to ensure it correctly identifies wrong data in the IPCData list.
    /// Returns true if any of the IPCData objects in the list have missing or invalid data.
    /// </summary>
    [Test]
    public void CheckForWrongDataTest()
    {
        // Arrange
        var ipcDataList = new List<IPCData>
        {
            new() { IPC = "TestIPC", DataFactory = 1, Time = new DateTime(2024, 6, 4), AvgValue = 1500.05, MaxValue = 2200, MinValue = 1000.14, MetricID = "TestMetricID", CpuMHz = 2500 },
            new() { IPC = "TestIPC2", DataFactory = 0 },
            new() { IPC = "TestIPC3", AvgValue = -50 },
            new() { IPC = "TestIPC4", MinValue = -50 },
            new() { IPC = "TestIPC5", MaxValue = -50 },
            new() { IPC = "TestIPC6", MetricID = "" },
            new() { IPC = "TestIPC7", CpuMHz = 0 }
        };
        // Act
        var result = _dataService.CheckForWrongData(ipcDataList);
        // Assert
        Assert.That(result);
    }

    /// <summary>
    /// Tests the functionality of removing missing data from the IPCData list.
    /// </summary>
    [Test]
    public void RemoveMissingDataTest()
    {
        // Arrange
        var ipcDataList = new List<IPCData>
        {
            new() { IPC = "TestIPC", DataFactory = 1, Time = new DateTime(2024, 6, 4), AvgValue = 1500.05, MaxValue = 2200, MinValue = 1000.14, MetricID = "TestMetricID", CpuMHz = 2500 },
            new() { IPC = "TestIPC2", DataFactory = 0 },
            new() { IPC = "TestIPC3", AvgValue = -50 },
            new() { IPC = "TestIPC4", MinValue = -50 },
            new() { IPC = "TestIPC5", MaxValue = -50 },
            new() { IPC = "TestIPC6", MetricID = "" },
            new() { IPC = "TestIPC7", CpuMHz = 0 }
        };
        // Act
        var result = _dataService.RemoveMissingData(ipcDataList);
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].IPC, Is.EqualTo("TestIPC"));
        });
    }

    /// <summary>
    /// Test method to verify the functionality of removing IPCs based on a time period.
    /// </summary>
    [Test]
    public void RemoveIPCsBasedOnTimePeriodTest()
    {
        // Arrange
        var ipcDataList = new List<IPCData>
        {
            new() { IPC = "TestIPC" },
            new() { IPC = "TestIPC" },
            new() { IPC = "TestIPC2" },
            new() { IPC = "TestIPC3" }
        };
        // Act
        var result = _dataService.RemoveIPCsBasedOnTimePeriod(ipcDataList, 2);
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.All(d => d.IPC == "TestIPC"));
        });
    }

    /// <summary>
    /// Removes IPCData objects from the given list that have an average value outside the range of their minimum and maximum values.
    /// </summary>
    /// <param name="ipcDataList">The list of IPCData objects.</param>
    /// <returns>The filtered list of IPCData objects.</returns>
    [Test]
    public void RemoveAvgValuesTest()
    {
        // Arrange
        var ipcDataList = new List<IPCData>
        {
            new() { AvgValue = 1000, MinValue = 500, MaxValue = 1500 },
            new() { AvgValue = 2000, MinValue = 500, MaxValue = 1500 },
            new() { AvgValue = 100, MinValue = 500, MaxValue = 1500 }
        };
        // Act
        var result = _dataService.RemoveAvgValues(ipcDataList);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.All(d => d.AvgValue >= d.MinValue && d.AvgValue <= d.MaxValue));
            Assert.That(result, Has.Count.EqualTo(1));
        });
    }

    /// <summary>
    /// Tests the normalization of CpuMHz values in the IPCData list.
    /// </summary>
    /// <remarks>
    /// This test ensures that the NormalizeCpuMHz method correctly normalizes the CpuMHz values in the IPCData list.
    /// The method should modify the CpuMHz values in the list so that they are all equal to the first element's CpuMHz value.
    /// </remarks>
    [Test]
    public void NormalizeCpuMHzTest()
    {
        // Arrange
        var ipcDataList = new List<IPCData>
        {
            new() { CpuMHz = 2500 },
            new() { CpuMHz = 3000 },
            new() { CpuMHz = 2000 }
        };
        
        // Act
        _dataService.NormalizeCpuMHz(ipcDataList);

        // Assert that all values are the same
        Assert.That(ipcDataList.All(ipcData => ipcData.CpuMHz == ipcDataList[0].CpuMHz));
    }
    
    /// <summary>
    /// Tests the CheckMetricIDs method to ensure it returns true when all IPCData objects have the same MetricID.
    /// </summary>
    [Test]
    public void CheckMetricIDs_ShouldReturnTrueForSameMetricIDs()
    {
        // Arrange
        var ipcDataList = new List<IPCData>
        {
            new() { MetricID = "TestMetricID" },
            new() { MetricID = "TestMetricID" },
            new() { MetricID = "TestMetricID" }
        };

        // Act
        var result = _dataService.CheckMetricIDs(ipcDataList);

        // Assert
        Assert.That(result);
    }

    /// <summary>
    /// Tests the CheckMetricIDs method to verify that it returns false when given a list of IPCData objects with different MetricIDs.
    /// </summary>
    [Test]
    public void CheckMetricIDs_ShouldReturnFalseForDifferentMetricIDs()
    {
        // Arrange
        var ipcDataList = new List<IPCData>
        {
            new() { MetricID = "TestMetricID" },
            new() { MetricID = "TestMetricID2" },
            new() { MetricID = "TestMetricID" }
        }; 

        // Act
        var result = _dataService.CheckMetricIDs(ipcDataList);

        // Assert
        Assert.That(!result);
    }
}