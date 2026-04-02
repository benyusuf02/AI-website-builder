using YDeveloper.Constants;

namespace YDeveloper.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message = null) 
            : base(message ?? ErrorMessages.NotFound) { }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message = null) 
            : base(message ?? ErrorMessages.Unauthorized) { }
    }

    public class ValidationException : Exception
    {
        public List<string> Errors { get; }

        public ValidationException(List<string> errors) 
            : base(ErrorMessages.ValidationFailed)
        {
            Errors = errors;
        }

        public ValidationException(string error) 
            : base(ErrorMessages.ValidationFailed)
        {
            Errors = new List<string> { error };
        }
    }
}
