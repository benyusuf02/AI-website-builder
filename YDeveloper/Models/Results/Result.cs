namespace YDeveloper.Models.Results
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();

        public static Result Success(string message = "İşlem başarılı") => new() { IsSuccess = true, Message = message };
        public static Result Failure(string message) => new() { IsSuccess = false, Message = message };
        public static Result Failure(List<string> errors) => new() { IsSuccess = false, Errors = errors };
    }

    public class Result<T> : Result
    {
        public T? Data { get; set; }

        public static Result<T> Success(T data, string message = "İşlem başarılı") 
            => new() { IsSuccess = true, Data = data, Message = message };
        
        public static new Result<T> Failure(string message) 
            => new() { IsSuccess = false, Message = message };
    }
}
