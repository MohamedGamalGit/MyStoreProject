// See https://aka.ms/new-console-template for more information
using System.Net.Http.Json;

Console.WriteLine("Hello, World!");

using var client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:8080/engine-rest/");

var payload = new
{
    variables = new
    {
        userId = new { value = "123" },
        days = new { value = 5 }
    }
};

// بدء عملية workflow باسم LeaveRequest
var response = await client.PostAsJsonAsync("/process-definition/key/LeaveRequest/start", payload);

response.EnsureSuccessStatusCode();

Console.WriteLine("Process started successfully");

