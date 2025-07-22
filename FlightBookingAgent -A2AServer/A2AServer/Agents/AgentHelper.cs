
namespace A2AServer.Agents
{
    using Azure.AI.Agents.Persistent;
    using Azure.Identity;
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Agents.A2A;
    using Microsoft.SemanticKernel.Agents.AzureAI;
    using SharpA2A.Core;

    public class AgentHelper
    {
        internal static async Task<A2AHostAgent> CreateFoundryHostAgentAsync(string agentType, string modelId = null, string endpoint = null, string assistantId = null, IEnumerable<KernelPlugin>? plugins = null)
        {
            var agentsClient = new PersistentAgentsClient(endpoint, new AzureCliCredential());
            PersistentAgent definition = await agentsClient.Administration.GetAgentAsync(assistantId);

#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            var agent = new AzureAIAgent(definition, agentsClient, plugins);
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            AgentCard agentCard = agentType.ToUpperInvariant() switch
            {
                "FLIGHTBOOKINGAGENT" => AgentCardHelper.GetFlightBookingAgentCard(),
                _ => throw new ArgumentException($"Unsupported agent type: {agentType}"),
            };

            return new A2AHostAgent(agent, agentCard);
        }
    }
}
