namespace YDeveloper.Models.Responses
{
    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public string? Detail { get; set; }
        public int StatusCode { get; set; }
        public string RequestId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class SuccessResponse<T>
    {
        public T Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public SuccessResponse(T data, string message = "İşlem başarılı")
        {
            Data = data;
            Message = message;
        }
    }
}
