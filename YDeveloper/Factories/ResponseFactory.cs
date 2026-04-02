namespace YDeveloper.Factories
{
    public interface IResponseFactory
    {
        Models.Api.ApiResponse<T> Success<T>(T data, string? message = null);
        Models.Api.ApiResponse<T> Error<T>(string message);
    }

    public class ResponseFactory : IResponseFactory
    {
        public Models.Api.ApiResponse<T> Success<T>(T data, string? message = null)
        {
            return Models.Api.ApiResponse<T>.SuccessResponse(data, message);
        }

        public Models.Api.ApiResponse<T> Error<T>(string message)
        {
            return Models.Api.ApiResponse<T>.ErrorResponse(message);
        }
    }
}
