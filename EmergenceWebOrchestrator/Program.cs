using System;
using Microsoft.SemanticKernel;

namespace EmergenceWebOrchestrator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string? emergenceApiKey = Environment.GetEnvironmentVariable("EMERGENCE_API_KEY");
            if (string.IsNullOrWhiteSpace(emergenceApiKey))
            {
                Console.WriteLine("Please set the EMERGENCE_API_KEY environment variable.");
                return;
            }


            var kernel = Kernel.CreateBuilder().Build();
            var emergenceSkill = new EmergenceWebOrchestratorSkill(emergenceApiKey);
            var plugin = kernel.ImportPluginFromObject(emergenceSkill, "Emergence");

            Console.WriteLine("\n");
            Console.WriteLine("Enter your prompt:");
            string userPrompt = Console.ReadLine() ?? "";


            if (string.IsNullOrWhiteSpace(userPrompt))
            {
                Console.WriteLine("Prompt cannot be empty.");
                return;
            }


            try
            {
                var webAutomationFunction = plugin["WebAutomationTool"];

                Console.WriteLine("\n");
                Console.WriteLine("Processing your request. This may take a while...");
                var result = await kernel.InvokeAsync(webAutomationFunction, new KernelArguments { ["prompt"] = userPrompt });

                Console.WriteLine("\n");
                Console.WriteLine("Emergence's Final Result:");
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}