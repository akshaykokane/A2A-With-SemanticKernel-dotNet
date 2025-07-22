using System.Threading.Tasks;
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
            _logger.LogInformation("Received chat request: {UserInput} with context ID: {ContextId}", userInput, contextId);

            ChatHistory chatHistory = new ChatHistory();
            chatHistory.AddUserMessage(userInput);
            var response = await _agentService.GetAgentResponseAsync(userInput, chatHistory);

            _logger.LogInformation("Flight booking agent response: {Response}", response);

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