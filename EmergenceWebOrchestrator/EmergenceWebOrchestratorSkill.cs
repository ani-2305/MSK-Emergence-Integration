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

        // Matches the "em-orchestrator" endpoint
        private const string BaseUrl = "https://api.emergence.ai/v0/orchestrators/em-orchestrator/workflows";

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
            // JSON payload with "prompt"
            var payload = new { prompt };
            var requestBody = JsonSerializer.Serialize(payload);

            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(BaseUrl, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            // The returned JSON has "workflowId" at the top level
            var workflowId = doc.RootElement.GetProperty("workflowId").GetString();
            return workflowId ?? throw new Exception("No workflowId found in response.");
        }

        private async Task<string> PollWorkflow(string workflowId)
        {
            var fullUrl = $"{BaseUrl}/{workflowId}";
            int pollCount = 1;

            Console.Write("Searching: 1");

            while (true)
            {
                var response = await _httpClient.GetAsync(fullUrl);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonString);

                // In Python you do: data_obj = response_json["data"]
                // Then data_obj["status"], data_obj["output"]
                var dataElement = doc.RootElement.GetProperty("data");
                var status = dataElement.GetProperty("status").GetString() ?? "UNKNOWN";

                if (status == "SUCCESS")
                {
                    Console.WriteLine();
                    // "output" is just a string
                    var outputStr = dataElement.GetProperty("output").GetString();
                    return outputStr ?? "No output found in successful workflow.";
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

                await Task.Delay(10000);  // Poll every 10 seconds
                pollCount++;
                Console.Write($"\rSearching: {pollCount}");
            }
        }
    }
}
