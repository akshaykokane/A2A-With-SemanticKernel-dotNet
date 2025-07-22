# A2A-With-SemanticKernel-dotNet

A comprehensive Agent-to-Agent (A2A) communication system built with Microsoft Semantic Kernel and .NET 9.0. This project demonstrates how to create, deploy, and orchestrate multiple AI agents that can communicate and collaborate to handle complex tasks like travel booking.

## 📋 Table of Contents

- [Project Overview](#project-overview)
- [Architecture & Development Approach](#architecture--development-approach)
- [Prerequisites](#prerequisites)
- [Step-by-Step Development Guide](#step-by-step-development-guide)
- [Running the Completed Project](#running-the-completed-project)
- [Testing the System](#testing-the-system)
- [Key Learning Points](#key-learning-points)
- [Troubleshooting](#troubleshooting)

## 🎯 Project Overview

This project implements an Agent-to-Agent communication system where:

1. **FlightBookingAgent (A2AServer)** - A specialized agent that handles flight booking operations using Azure AI Foundry
2. **TravelBookingAgent (A2AClient)** - A web-based orchestrator that communicates with multiple A2A agents to plan complete travel itineraries

### Key Technologies Used
- **.NET 9.0** - Latest .NET framework
- **Microsoft Semantic Kernel 1.60.0** - AI orchestration framework
- **Microsoft.SemanticKernel.Agents.A2A** - Agent-to-Agent communication protocol
- **SharpA2A.AspNetCore** - A2A protocol implementation for ASP.NET Core
- **Azure AI Foundry** - AI agent hosting and management platform
- **Azure OpenAI** - Large language model services

## ⚡ TLDR; on Steps

### Part 1: A2A Agent Server (Flight Booking Agent)

1. **Define Agent Helper**: Create `AgentHelper.cs` with `CreateFoundryHostAgentAsync()` to connect your Azure AI Foundry agent to the A2A protocol.
2. **Create Agent Card**: Build `AgentCardHelper.cs` defining your agent's capabilities (name, description, version, capabilities list).
3. **Configure Host Agent**: Use `A2AHostAgent` to wrap your Azure AI agent and expose it via the A2A protocol.
4. **Run Server**: Configure ASP.NET Core with `app.MapA2A()` to launch your A2A server, making your agent discoverable at endpoints like `/agent-card`.

### Part 2: A2A Client (Travel Planner Agent)

1. **Create A2A Helper**: Build `A2AHelper.cs` with `GetA2AAgent()` to fetch agent cards from remote A2A servers.
2. **Initialize A2A Client**: Create `A2AClient` and `A2ACardResolver` instances using HTTP client to communicate with agent servers.
3. **Register as SK Plugin**: Use `AgentKernelFunctionFactory.CreateFromAgent()` to convert A2A agents into Semantic Kernel functions.
4. **Integrate with Semantic Kernel**: Add A2A agent functions to your kernel's plugin collection via `CreatePluginFromA2AAgent()`.
5. **Orchestrate Agents**: Let Semantic Kernel automatically invoke A2A agents when processing user requests through natural language understanding.

## 🏗️ Architecture & Development Approach

### Development Philosophy
This project was developed following a **microservices agent architecture** where each agent is:
- **Specialized** - Handles specific domain tasks (flights, hotels, etc.)
- **Autonomous** - Can operate independently
- **Communicative** - Uses standardized A2A protocol for inter-agent communication
- **Discoverable** - Exposes agent cards describing capabilities

### System Architecture

```
┌─────────────────────┐     A2A Protocol     ┌─────────────────────┐
│  TravelBookingAgent │ ◄──────────────────► │ FlightBookingAgent  │
│     (Client)        │                      │     (Server)        │
│                     │                      │                     │
│ ┌─────────────────┐ │                      │ ┌─────────────────┐ │
│ │   Web UI        │ │                      │ │ Azure AI        │ │
│ │   (HTML/JS)     │ │                      │ │ Foundry Agent   │ │
│ └─────────────────┘ │                      │ └─────────────────┘ │
│ ┌─────────────────┐ │                      │ ┌─────────────────┐ │
│ │ Semantic Kernel │ │                      │ │ A2A Host Agent  │ │
│ │ Agent           │ │                      │ │                 │ │
│ └─────────────────┘ │                      │ └─────────────────┘ │
└─────────────────────┘                      └─────────────────────┘
```

## 🛠️ Prerequisites

Before starting development, ensure you have:

### Software Requirements
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- Visual Studio 2022 or Visual Studio Code
- Git for version control
- Azure CLI (optional, but recommended)

### Azure Services
- **Azure OpenAI Service** with GPT-4 deployment
- **Azure AI Foundry** (formerly Azure AI Studio) account
- Active Azure subscription

### Development Tools (Recommended)
- REST Client (Postman, Thunder Client, or similar)
- Browser developer tools for frontend debugging

## 📚 Step-by-Step Development Guide

### Phase 1: Project Setup and Structure

#### Step 1.1: Create Solution Structure
```bash
# Create main project directory
mkdir A2A-With-SemanticKernel-dotNet
cd A2A-With-SemanticKernel-dotNet

# Create server project structure
mkdir "FlightBookingAgent -A2AServer"
cd "FlightBookingAgent -A2AServer"
dotnet new sln -n "FlightBookingAgent -A2A Client"
dotnet new webapi -n A2AServer
dotnet sln add A2AServer/A2AServer.csproj

# Create client project structure
cd ..
mkdir "TravelBookingAgent - A2AClient"
cd "TravelBookingAgent - A2AClient"
dotnet new sln -n A2AClient
dotnet new webapi -n A2AClient
dotnet sln add A2AClient/A2AClient.csproj
```

#### Step 1.2: Install Required NuGet Packages

**For FlightBookingAgent (Server):**
```bash
cd "FlightBookingAgent -A2AServer/A2AServer"
dotnet add package Microsoft.AspNetCore.OpenApi --version 9.0.7
dotnet add package Microsoft.SemanticKernel --version 1.60.0
dotnet add package Microsoft.SemanticKernel.Agents.A2A --version 1.60.0-alpha
dotnet add package Microsoft.SemanticKernel.Agents.AzureAI --version 1.60.0-preview
dotnet add package SharpA2A.AspNetCore --version 0.2.2-preview.1
```

**For TravelBookingAgent (Client):**
```bash
cd "TravelBookingAgent - A2AClient/A2AClient"
dotnet add package Microsoft.AspNetCore.OpenApi --version 9.0.7
dotnet add package Microsoft.SemanticKernel --version 1.60.0
dotnet add package Microsoft.SemanticKernel.Agents.A2A --version 1.60.0-alpha
dotnet add package Microsoft.SemanticKernel.Agents.Core --version 1.60.0
dotnet add package Swashbuckle.AspNetCore --version 9.0.3
```

### Phase 2: Azure AI Foundry Agent Setup

#### Step 2.1: Create Azure AI Foundry Agent
1. Navigate to [Azure AI Foundry](https://ai.azure.com)
2. Create a new project
3. Navigate to "Agents" section
4. Create a new agent with:
   - **Name**: FlightBookingAgent
   - **Model**: GPT-4o
   - **Instructions**: "You are a specialized flight booking agent. Help users search for flights, compare prices, and book tickets. Always ask for departure city, destination, travel dates, and passenger count."
5. Note down the **Assistant ID** (e.g., `asst_E8h9axBS1S4CX7rZ551SxYT3`)
6. Note down the **Endpoint URL** (e.g., `https://ai-foundary-akshay.services.ai.azure.com/api/projects/firstProject`)

### Phase 3: FlightBookingAgent Server Development

#### Step 3.1: Create Agent Helper Classes

**Create `Agents/AgentCardHelper.cs`:**
```csharp
using SharpA2A.Core;

namespace A2AServer.Agents
{
    public class AgentCardHelper
    {
        public static AgentCard GetFlightBookingAgentCard()
        {
            return new AgentCard
            {
                Name = "FlightBookingAgent",
                Description = "Specialized agent for flight booking and search operations",
                Version = "1.0.0",
                Capabilities = new List<string>
                {
                    "flight_search",
                    "price_comparison", 
                    "booking_assistance"
                }
            };
        }
    }
}
```

**Create `Agents/AgentHelper.cs`:**
```csharp
using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.A2A;
using Microsoft.SemanticKernel.Agents.AzureAI;
using SharpA2A.Core;

namespace A2AServer.Agents
{
    public class AgentHelper
    {
        internal static async Task<A2AHostAgent> CreateFoundryHostAgentAsync(
            string agentType, 
            string modelId = null, 
            string endpoint = null, 
            string assistantId = null, 
            IEnumerable<KernelPlugin>? plugins = null)
        {
            // Create Azure AI client
            var agentsClient = new PersistentAgentsClient(endpoint, new AzureCliCredential());
            PersistentAgent definition = await agentsClient.Administration.GetAgentAsync(assistantId);

            // Create Azure AI Agent
            var agent = new AzureAIAgent(definition, agentsClient, plugins);

            // Get agent card based on type
            AgentCard agentCard = agentType.ToUpperInvariant() switch
            {
                "FLIGHTBOOKINGAGENT" => AgentCardHelper.GetFlightBookingAgentCard(),
                _ => throw new ArgumentException($"Unsupported agent type: {agentType}"),
            };

            return new A2AHostAgent(agent, agentCard);
        }
    }
}
```

#### Step 3.2: Configure Server Program.cs

**Update `Program.cs`:**
```csharp
using A2AServer.Agents;
using Microsoft.SemanticKernel.Agents.A2A;
using SharpA2A.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient().AddLogging();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure OpenAPI in development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Get agent type from command line arguments
var agentType = GetAgentTypeFromArgs(args);

A2AHostAgent? hostAgent = null;

switch (agentType.ToLowerInvariant())
{
    case "flightbookingagent":
        hostAgent = await AgentHelper.CreateFoundryHostAgentAsync(
            agentType: "FlightBookingAgent",
            modelId: "gpt-4o",
            endpoint: "YOUR_AZURE_AI_FOUNDRY_ENDPOINT",
            assistantId: "YOUR_ASSISTANT_ID",
            plugins: null);
        break;

    default:
        Console.WriteLine($"Unknown agent type: {agentType}");
        Console.WriteLine("Available agents: FlightBookingAgent");
        Environment.Exit(1);
        return;
}

// Map A2A endpoints
app.MapA2A(hostAgent!.TaskManager!, "");
app.UseHttpsRedirection();
app.Run();

// Helper method to parse command line arguments
static string GetAgentTypeFromArgs(string[] args)
{
    for (int i = 0; i < args.Length - 1; i++)
    {
        if (args[i] == "--agent" || args[i] == "-a")
        {
            return args[i + 1];
        }
    }
    
    Console.WriteLine("No agent specified. Use --agent parameter. Defaulting to FlightBookingAgent.");
    return "FlightBookingAgent";
}
```

#### Step 3.3: Configure Launch Settings

**Update `Properties/launchSettings.json`:**
```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5237",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "https://localhost:7024;http://localhost:5237",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### Phase 4: TravelBookingAgent Client Development

#### Step 4.1: Configure Application Settings

**Update `appsettings.json`:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "AzureOpenAI": {
    "ApiKey": "YOUR_AZURE_OPENAI_API_KEY",
    "Endpoint": "https://your-resource.openai.azure.com",
    "DeploymentName": "gpt-4-deployment-name",
    "ApiVersion": "2024-12-01-preview"
  }
}
```

#### Step 4.2: Create A2A Helper Service

**Create `Services/A2AHelper.cs`:**
```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.A2A;
using SharpA2A.Core;

namespace A2ADemo.Services
{
    public class A2AHelper
    {
        public static async Task CreatePluginFromA2AAgent(List<KernelFunction> a2aAgents, string[] agentUrls)
        {
            var createAgentTasks = agentUrls.Select(agentUrl => GetA2AAgent(agentUrl));
            var agents = await Task.WhenAll(createAgentTasks);
            var agentFunctions = agents.Select(agent => AgentKernelFunctionFactory.CreateFromAgent(agent)).ToList();
            var agentPlugin = KernelPluginFactory.CreateFromFunctions("AgentPlugin", agentFunctions);
            a2aAgents.AddRange(agentPlugin);
            Console.WriteLine($"A2A Plugin created with {a2aAgents.Count} functions");
        }

        public static async Task<A2AAgent> GetA2AAgent(string agentUri)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(agentUri),
                Timeout = TimeSpan.FromSeconds(60)
            };

            var client = new A2AClient(httpClient);
            var cardResolver = new A2ACardResolver(httpClient);
            AgentCard agentCard = await cardResolver.GetAgentCardAsync();
            
            if (agentCard == null)
            {
                throw new InvalidOperationException($"No agent card found at {agentUri}");
            }

            Console.WriteLine($"A2A Agent '{agentCard.Name}' found at {agentUri}");
            return new A2AAgent(client, agentCard!);
        }
    }
}
```

#### Step 4.3: Create Travel Planner Agent Service

**Create `Services/TravelPlannerAgentService.cs`:**
```csharp
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using System.Text;

namespace A2ADemo.Services
{
    public class TravelPlannerAgentService
    {
        private readonly ILogger<TravelPlannerAgentService> _logger;
        private readonly IConfiguration _configuration;
        private readonly ChatCompletionAgent _travelPlanningAgent;

        public TravelPlannerAgentService(ILogger<TravelPlannerAgentService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            // Load Azure OpenAI settings
            var apiKey = _configuration["AzureOpenAI:ApiKey"];
            var endpoint = _configuration["AzureOpenAI:Endpoint"];
            var deploymentName = _configuration["AzureOpenAI:DeploymentName"];

            // Create A2A agent functions
            List<KernelFunction> a2aAgents = new List<KernelFunction>();
            A2AHelper.CreatePluginFromA2AAgent(a2aAgents, new string[] { "http://localhost:5237" }).Wait();

            // Create Semantic Kernel with Azure OpenAI
            var kernelBuilder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);

            if (a2aAgents.Any())
            {
                kernelBuilder.Plugins.AddFromFunctions("A2AAgents", a2aAgents);
            }

            var kernel = kernelBuilder.Build();

            // Create travel planning agent
            _travelPlanningAgent = new ChatCompletionAgent()
            {
                Instructions = "You are a helpful travel planning assistant. You can help users plan their trips by coordinating with specialized agents for flights, hotels, and other travel services. When users ask about flights, use the available FlightBookingAgent to get flight information.",
                Name = "TravelPlanner",
                Kernel = kernel,
            };
        }

        public async Task<string> GetAgentResponseAsync(string userInput, ChatHistory chatHistory)
        {
            try
            {
                _logger.LogInformation("Processing user input: {UserInput}", userInput);
                
                var response = new StringBuilder();
                await foreach (var content in _travelPlanningAgent.InvokeAsync(chatHistory))
                {
                    response.Append(content.Content);
                }

                return response.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing agent response");
                return "Sorry, I encountered an error while processing your request. Please try again.";
            }
        }
    }
}
```

#### Step 4.4: Create Chat Controller

**Create `Controllers/ChatController.cs`:**
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using A2ADemo.Services;
using Microsoft.SemanticKernel.ChatCompletion;

namespace A2ADemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly TravelPlannerAgentService _agentService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(TravelPlannerAgentService agentService, ILogger<ChatController> logger)
        {
            _agentService = agentService;
            _logger = logger;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromForm] string userInput, [FromForm] string contextId = "default")
        {
            _logger.LogInformation("Received chat request: {UserInput}", userInput);

            ChatHistory chatHistory = new ChatHistory();
            chatHistory.AddUserMessage(userInput);
            var response = await _agentService.GetAgentResponseAsync(userInput, chatHistory);

            return Ok(new { response });
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");
            if (System.IO.File.Exists(filePath))
            {
                var htmlContent = System.IO.File.ReadAllText(filePath);
                return new ContentResult
                {
                    Content = htmlContent,
                    ContentType = "text/html"
                };
            }
            return NotFound("<h1>Error: index.html not found!</h1>");
        }
    }
}
```

#### Step 4.5: Create Web Interface

**Create `wwwroot/index.html`:**
```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Travel Planning Agent</title>
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #1e3c72 0%, #2a5298 100%);
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            margin: 0;
        }
        .container {
            background: rgba(255, 255, 255, 0.95);
            backdrop-filter: blur(10px);
            padding: 35px 45px;
            border-radius: 20px;
            box-shadow: 0 20px 40px rgba(0, 0, 0, 0.3);
            width: 100%;
            max-width: 600px;
        }
        h1 {
            text-align: center;
            color: #1e3c72;
            margin-bottom: 30px;
            font-size: 2.2em;
            font-weight: 600;
        }
        .chat-container {
            height: 400px;
            overflow-y: auto;
            border: 2px solid #e0e6ed;
            border-radius: 15px;
            padding: 20px;
            margin-bottom: 20px;
            background: #f8f9fa;
        }
        .message {
            margin-bottom: 15px;
            padding: 12px 18px;
            border-radius: 18px;
            max-width: 80%;
            word-wrap: break-word;
        }
        .user-message {
            background: #007bff;
            color: white;
            margin-left: auto;
            text-align: right;
        }
        .agent-message {
            background: #e9ecef;
            color: #333;
            margin-right: auto;
        }
        .input-container {
            display: flex;
            gap: 10px;
        }
        #userInput {
            flex: 1;
            padding: 15px 20px;
            border: 2px solid #ddd;
            border-radius: 25px;
            font-size: 16px;
            outline: none;
            transition: border-color 0.3s;
        }
        #userInput:focus {
            border-color: #007bff;
        }
        #sendButton {
            padding: 15px 30px;
            background: #007bff;
            color: white;
            border: none;
            border-radius: 25px;
            cursor: pointer;
            font-size: 16px;
            font-weight: 600;
            transition: background 0.3s;
        }
        #sendButton:hover {
            background: #0056b3;
        }
        #sendButton:disabled {
            background: #6c757d;
            cursor: not-allowed;
        }
        .loading {
            text-align: center;
            color: #666;
            font-style: italic;
        }
    </style>
</head>
<body>
    <div class="container">
        <h1>🌍 Travel Planning Agent</h1>
        <div id="chatContainer" class="chat-container">
            <div class="message agent-message">
                Hello! I'm your travel planning assistant. I can help you with flight bookings, hotel reservations, and complete travel itinerary planning. How can I assist you today?
            </div>
        </div>
        <div class="input-container">
            <input type="text" id="userInput" placeholder="Type your travel question here..." />
            <button id="sendButton">Send</button>
        </div>
    </div>

    <script>
        const chatContainer = document.getElementById('chatContainer');
        const userInput = document.getElementById('userInput');
        const sendButton = document.getElementById('sendButton');

        function addMessage(content, isUser = false) {
            const messageDiv = document.createElement('div');
            messageDiv.className = `message ${isUser ? 'user-message' : 'agent-message'}`;
            messageDiv.textContent = content;
            chatContainer.appendChild(messageDiv);
            chatContainer.scrollTop = chatContainer.scrollHeight;
        }

        function showLoading() {
            const loadingDiv = document.createElement('div');
            loadingDiv.className = 'loading';
            loadingDiv.textContent = 'Agent is thinking...';
            loadingDiv.id = 'loadingMessage';
            chatContainer.appendChild(loadingDiv);
            chatContainer.scrollTop = chatContainer.scrollHeight;
        }

        function removeLoading() {
            const loadingMessage = document.getElementById('loadingMessage');
            if (loadingMessage) {
                loadingMessage.remove();
            }
        }

        async function sendMessage() {
            const message = userInput.value.trim();
            if (!message) return;

            addMessage(message, true);
            userInput.value = '';
            sendButton.disabled = true;
            showLoading();

            try {
                const formData = new FormData();
                formData.append('userInput', message);

                const response = await fetch('/api/chat/chat', {
                    method: 'POST',
                    body: formData
                });

                const data = await response.json();
                removeLoading();
                addMessage(data.response);
            } catch (error) {
                removeLoading();
                addMessage('Sorry, I encountered an error. Please try again.');
                console.error('Error:', error);
            } finally {
                sendButton.disabled = false;
            }
        }

        sendButton.addEventListener('click', sendMessage);
        userInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                sendMessage();
            }
        });
    </script>
</body>
</html>
```

#### Step 4.6: Update Program.cs

**Update client `Program.cs`:**
```csharp
using A2ADemo.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<TravelPlannerAgentService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### Phase 5: Configuration and Security

#### Step 5.1: Set Up Azure Authentication
1. Install Azure CLI
2. Run `az login` to authenticate
3. Set the correct subscription: `az account set --subscription "your-subscription-id"`

#### Step 5.2: Configure User Secrets (Recommended for Development)
```bash
# In the client project directory
dotnet user-secrets init
dotnet user-secrets set "AzureOpenAI:ApiKey" "your-actual-api-key"
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://your-resource.openai.azure.com"
```

## 🚀 Running the Completed Project

### Step 1: Start the FlightBookingAgent Server
```bash
cd "FlightBookingAgent -A2AServer/A2AServer"
dotnet run -- --agent FlightBookingAgent
```

### Step 2: Start the TravelBookingAgent Client
```bash
cd "TravelBookingAgent - A2AClient/A2AClient"
dotnet run
```

### Step 3: Access the Application
- Open your browser to: `http://localhost:5223`
- The travel planning interface will be available

## 🧪 Testing the System

### Test Scenarios

1. **Basic Flight Search**:
   - Input: "I need to book a flight from New York to London for next month"
   - Expected: Agent should use FlightBookingAgent to search for flights

2. **Complex Travel Planning**:
   - Input: "Plan a 5-day trip to Paris including flights and hotel recommendations"
   - Expected: Agent should coordinate multiple aspects of travel planning

3. **Agent Communication**:
   - Monitor console logs to see A2A protocol communication between agents

### Debugging Tips

1. **Check Agent Connectivity**:
   ```bash
   curl http://localhost:5237/agent-card
   ```

2. **Monitor Logs**: Watch both server and client console outputs for A2A communication logs

3. **Test API Endpoints**:
   - Server: `http://localhost:5237/swagger` (if enabled)
   - Client: `http://localhost:5223/swagger`

## 📚 Key Learning Points

### A2A Protocol Implementation
- **Agent Cards**: Define agent capabilities and metadata
- **Task Managers**: Handle agent lifecycle and communication
- **Function Factories**: Convert agents into callable Semantic Kernel functions

### Semantic Kernel Integration
- **Plugin System**: A2A agents are integrated as Semantic Kernel plugins
- **Function Calling**: Agents can invoke each other through the kernel's function calling mechanism
- **Chat Completion**: Primary interface for user interaction

### Microservices Architecture
- **Service Discovery**: Agents discover each other through A2A protocol
- **Loose Coupling**: Each agent operates independently
- **Scalability**: New agents can be added without modifying existing ones

### Azure AI Integration
- **Azure AI Foundry**: Provides robust agent hosting and management
- **Azure OpenAI**: Powers natural language understanding and generation
- **Azure Authentication**: Secure service-to-service communication

## 🔧 Troubleshooting

### Common Issues

1. **Agent Connection Failed**
   - **Cause**: FlightBookingAgent server not running
   - **Solution**: Ensure server is started before client

2. **Azure Authentication Error**
   - **Cause**: Not logged into Azure CLI
   - **Solution**: Run `az login` and set correct subscription

3. **OpenAI API Errors**
   - **Cause**: Invalid API key or endpoint
   - **Solution**: Verify configuration in `appsettings.json` or user secrets

4. **Port Conflicts**
   - **Cause**: Ports 5237 or 5223 already in use
   - **Solution**: Update `launchSettings.json` with different ports

### Advanced Debugging

1. **Enable Detailed Logging**:
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Debug",
         "Microsoft.SemanticKernel": "Debug"
       }
     }
   }
   ```

2. **Monitor Network Traffic**: Use Fiddler or similar tools to inspect A2A HTTP communication

3. **Agent Card Validation**: Ensure agent cards are properly formatted and accessible

## 🎯 Next Steps

### Extending the System
1. **Add More Agents**: Create HotelBookingAgent, CarRentalAgent, etc.
2. **Implement Persistence**: Add database storage for conversations and bookings
3. **Add Authentication**: Implement user authentication and authorization
4. **Deploy to Azure**: Use Azure Container Apps or App Service for hosting
5. **Add Monitoring**: Implement Application Insights for telemetry

### Best Practices
- Use dependency injection for all services
- Implement proper error handling and retry policies
- Add comprehensive logging and monitoring
- Follow security best practices for API keys and authentication
- Write unit and integration tests

This comprehensive guide provides everything needed to understand, build, and extend the A2A-With-SemanticKernel-dotNet project from scratch.