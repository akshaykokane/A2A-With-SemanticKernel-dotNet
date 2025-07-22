

namespace A2ADemo.Services
{
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Agents;
    using Microsoft.SemanticKernel.Agents.A2A;
    using SharpA2A.Core;

    public class A2AHelper
    {

        #pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        public static async Task CreatePluginFromA2AAgent(List<KernelFunction> a2aAgents, string[] agentUrls)
        {
            var createAgentTasks = agentUrls.Select(agentUrl => GetA2AAgent(agentUrl));
            var agents = await Task.WhenAll(createAgentTasks);
            var agentFunctions = agents.Select(agent => AgentKernelFunctionFactory.CreateFromAgent(agent)).ToList();
            var agentPlugin = KernelPluginFactory.CreateFromFunctions("AgentPlugin", agentFunctions);
            a2aAgents.AddRange(agentPlugin);
            Console.WriteLine($"A2A Plugin created with {a2aAgents.Count} functions");
        }

        #pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

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

            Console.WriteLine($"A2A Agent '{agentCard}' found at {agentUri}");

            return new A2AAgent(client, agentCard!);
        }
    }
}