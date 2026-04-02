using Microsoft.AspNetCore.Mvc;
using YDeveloper.Models.Ai;
using YDeveloper.Services.Ai;

namespace YDeveloper.Controllers.Api
{
    [Route("api/ai/[action]")]
    [ApiController]
    public class AiCommandController : ControllerBase
    {
        private readonly IAiCommandService _aiService;

        public AiCommandController(IAiCommandService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost]
        public IActionResult Command([FromBody] AiCommandRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Prompt))
            {
                return BadRequest("Prompt is required.");
            }

            var response = _aiService.ProcessCommand(request.Prompt);
            return Ok(response);
        }
    }
}
