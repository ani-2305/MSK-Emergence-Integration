# Microsoft Semantic Kernel and Emergence AI Web Orchestrator Integration

## Requirements
.NET SDK 9.0 or later

## Project Setup Instructions:

### 1. Install the .NET SDK:
    On macOS: brew install --cask dotnet-sdk
    On Windows/Linux: Download from https://dotnet.microsoft.com/download


### 2. Clone this GitHub repository:
   
    git clone https://username:yourtoken@github.com/username/MSK-Emergence-Integration.git
   
    cd EmergenceWebOrchestrator


### 3. Set your Emergence API key:
   
    export EMERGENCE_API_KEY="YOUR_API_KEY_HERE"


### 4. Restore dependencies:

    dotnet restore


### 5. Build the project:
   
    dotnet build


### 6. Run the project:
    
    dotnet run


## Integration:
If you already have a custom orchestrator that you've developed with Microsoft Semantic Kernel, simply take the `EmergenceWebOrchestratorSkill.cs` file and import it into your existing project directory. Make sure to adjust the "namespace" accordingly. Now, to use the Emergence AI Web Orchestrator in your project, follow the steps below:

### 1. In your Orchestrator file (where you define your Orchestrator class), add the following in the header:
   
    using EmergenceWebOrchestrator;


### 2. Within your "InitializeSkills()" function add the following lines below your existing skills:
   
    // Initialize Emergence Web Orchestrator Skill
    string emergenceApiKey = Environment.GetEnvironmentVariable("EMERGENCE_API_KEY");
    var emergenceSkill = new EmergenceWebOrchestratorSkill(emergenceApiKey);
    _kernel.ImportPluginFromObject(emergenceSkill, "Emergence");


### 3. Within your "RunOrchestration(string prompt)" function add the following lines below your other skill usages:
   
    // Use Emergence Web Orchestrator
    var webAutomationFunction = _kernel.Plugins["Emergence"]["WebAutomationTool"];
    var finalResult = await _kernel.InvokeAsync(webAutomationFunction, new KernelArguments { ["prompt"] = intermediateResult ToString() });


### 7. Emergence AI's Web Orchestrator is now integrated within your custom Orchestrator!
