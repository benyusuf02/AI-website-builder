using YDeveloper.Models.Ai;

namespace YDeveloper.Services.Ai
{
    public interface IAiCommandService
    {
        AiResponse ProcessCommand(string prompt);
    }
}
