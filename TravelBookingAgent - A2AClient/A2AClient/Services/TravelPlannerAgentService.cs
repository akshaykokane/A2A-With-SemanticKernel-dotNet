using System.Threading.Tasks;
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

            // Load settings from configuration or environment
            var apiKey = _configuration["AzureOpenAI:ApiKey"];
            var endpoint = _configuration["AzureOpenAI:Endpoint"];
            var deploymentName = _configuration["AzureOpenAI:DeploymentName"];
            var apiVersion = _configuration["AzureOpenAI:ApiVersion"] ?? "2024-12-01-preview";
            List<KernelFunction> a2aAgents = new List<KernelFunction>();
            A2AHelper.CreatePluginFromA2AAgent(a2aAgents, new string[] { "http://localhost:5237" }).Wait();

            Kernel kernel = Kernel.CreateBuilder()
                    .AddAzureOpenAIChatCompletion(
                        apiKey: apiKey,
                        endpoint: endpoint,
                        deploymentName: deploymentName,
                        apiVersion: apiVersion)
                    .Build();

            kernel.ImportPluginFromFunctions("A2APlugin", a2aAgents);
            kernel.FunctionInvocationFilters.Add(new ConsoleOutputFunctionInvocationFilter());

            _travelPlanningAgent = new ChatCompletionAgent()
            {
                Name = "TravelPlanner",
                Instructions = "You are a helpful travel planning assistant. Use the provided tools to assist users with their travel plans.",
                Kernel = kernel,
                Arguments = new KernelArguments(new PromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }),

            };

        }

        public async Task<string> GetAgentResponseAsync(string userInput, ChatHistory chatHistory)
        {

            _logger.LogInformation("Received user input: {UserInput}", userInput);

            AgentThread thread = new ChatHistoryAgentThread(chatHistory);

            StringBuilder result = new StringBuilder();
            await foreach (ChatMessageContent response in _travelPlanningAgent.InvokeAsync(thread))
            {
                result.Append(response.Content);
            }
            return result.ToString();
        }
    }
}