namespace WebApplication1.Shared.ErrorCodes
{
    public class ErrorCode
    {
        public string Code { get; set; }
        public string Message { get; set; }

        public ErrorCode(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public static ErrorCode NotFound(string entityName) => new("NOT_FOUND", $"{entityName} not found.");
        public static ErrorCode ValidationError => new("VALIDATION_ERROR", "One or more validation errors occurred.");
        public static ErrorCode Unauthorized => new("UNAUTHORIZED", "Authentication failed. Please log in.");
        public static ErrorCode Forbidden => new("FORBIDDEN", "You do not have permission to perform this action.");
        public static ErrorCode InternalServerError => new("INTERNAL_SERVER_ERROR", "An unexpected internal server error occurred.");
    }
}