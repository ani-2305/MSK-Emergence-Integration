using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using System.ComponentModel;


namespace EmergenceWebOrchestrator
{
    public class EmergenceWebOrchestratorSkill
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BaseUrl = "https://api.emergence.ai/v0/orchestrators/em-web-automation/workflows";


        public EmergenceWebOrchestratorSkill(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("apikey", _apiKey);
        }


        [KernelFunction]
        [Description("Web Automation tool")]
        public async Task<string> WebAutomationTool(string prompt)
        {
            try
            {
                var workflowId = await CreateWorkflow(prompt);
                return await PollWorkflow(workflowId);
            }
            catch (Exception e)
            {
                return $"An error occurred while performing the web automation: {e.Message}";
            }
        }


        private async Task<string> CreateWorkflow(string prompt)
        {
            var payload = new { prompt };
            var requestBody = JsonSerializer.Serialize(payload);


            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(BaseUrl, content);
            response.EnsureSuccessStatusCode();


            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);


            var workflowId = doc.RootElement.GetProperty("workflowId").GetString();
            return workflowId ?? throw new Exception("No workflowId found in response.");
        }


        private async Task<string> PollWorkflow(string workflowId)
        {
            var fullUrl = $"{BaseUrl}/{workflowId}";
            int pollCount = 0;


            Console.Write("Searching: 1");


            while (true)
            {
                var response = await _httpClient.GetAsync(fullUrl);
                response.EnsureSuccessStatusCode();


                var jsonString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonString);
                
                var dataObj = doc.RootElement.GetProperty("data");
                var status = dataObj.GetProperty("status").GetString();


                if (status == "SUCCESS")
                {
                    Console.WriteLine();
                    var output = dataObj.GetProperty("output");
                    var result = output.GetProperty("result").GetString();
                    return result ?? "No result found in successful workflow.";
                }
                else if (status == "FAILED")
                {
                    Console.WriteLine();
                    return "Workflow failed. Check Emergence logs for details.";
                }
                else if (status == "TIMEOUT")
                {
                    Console.WriteLine();
                    return "Workflow timed out.";
                }

                await Task.Delay(10000);
                pollCount++;
                Console.Write($"\rSearching: {pollCount}");
            }
        }
    }
}