# Microsoft Semantic Kernel and Emergence AI Web Orchestrator Integration

## Requirements
.NET SDK 9.0 or later

## Project Setup Instructions:

### 1. Install the .NET SDK:
    On macOS: brew install --cask dotnet-sdk
    
    Or, on macOS(x64) for VSCode: Download from https://download.visualstudio.microsoft.com/download/pr/2bda19b1-6389-4520-8e5e-363172398741/662eee446961503151bb78c29997933e/dotnet-sdk-9.0.102-osx-x64.pkg
    
    Or, on macOS(ARM64) for VSCode: Download from https://download.visualstudio.microsoft.com/download/pr/96489126-b9ba-414a-a2d0-d8c5b61a22be/fe047e117e9cc43738ba2222f4769da2/dotnet-sdk-9.0.102-osx-arm64.pkg
    
    On Windows/Linux: Download from https://dotnet.microsoft.com/download


### 2. Clone this GitHub repository:
   
    git clone https://<your token>@github.com/<your username>/MSK-Emergence-Integration.git
   
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
