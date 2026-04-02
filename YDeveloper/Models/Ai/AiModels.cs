namespace YDeveloper.Models.Ai
{
    public class AiCommandRequest
    {
        public string Prompt { get; set; } = string.Empty;
        public string? Context { get; set; } // e.g., "Home", "Admin", "Editor"
    }

    public class AiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public AiActionType ActionType { get; set; }
        public object? Payload { get; set; }
    }

    public enum AiActionType
    {
        None,
        Redirect,           // Go to a URL
        StyleUpdate,        // Change CSS variables
        DomManipulation,    // Show/Hide/Move elements
        Toast,              // Show a notification
        ContentUpdate       // Update text content
    }

    public class AiStylePayload
    {
        public string Selector { get; set; } = ":root";
        public Dictionary<string, string> Styles { get; set; } = new Dictionary<string, string>();
    }

    public class AiRedirectPayload
    {
        public string Url { get; set; } = string.Empty;
    }
}
