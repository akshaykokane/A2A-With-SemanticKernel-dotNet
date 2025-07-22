using A2AServer.Agents;
using A2AServer;
using Microsoft.SemanticKernel.Agents.A2A;
using SharpA2A.AspNetCore;
using SharpA2A.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient().AddLogging();
builder.Services.AddOpenApi();

var app = builder.Build();
var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
var logger = app.Logger;


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var agentType = GetAgentTypeFromArgs(args);

A2AHostAgent? hostAgent = null;

switch (agentType.ToLowerInvariant())
{
    case "flightbookingagent":
        hostAgent  = await AgentHelper.CreateFoundryHostAgentAsync(
            agentType: "FlightBookingAgent",
            modelId: "gpt-4o",
            endpoint: "<REPLACE_WITH_AI_FOUNDRY_ENDPOINT>",
            assistantId: "<REPLACE_WITH_ASSISTANT_ID>",
            plugins: null);
        break;

    default:
        Console.WriteLine($"Unknown agent type: {agentType}");
        Console.WriteLine("Available agents: TrainBookingAgent, FlightBookingAgent, HotelBookingAgent");
        Environment.Exit(1);
        return;
}


app.MapA2A(hostAgent!.TaskManager!, "");

app.UseHttpsRedirection();

app.Run();


static string GetAgentTypeFromArgs(string[] args)
{
    // Look for --agent parameter
    for (int i = 0; i < args.Length - 1; i++)
    {
        Console.WriteLine(args[i]);
        Console.WriteLine(args[i + 1]);
        if (args[i] == "--agent" || args[i] == "-a")
        {
            return args[i + 1];
        }
    }

    Console.WriteLine("No agent specified. Use --agent or -a parameter to specify agent type (TrainBookingAgent, FlightBookingAgent, HotelBookingAgent). Defaulting to 'echo'.");
    return "echo";
}