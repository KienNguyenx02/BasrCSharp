namespace WebApplication1.Shared.ErrorCodes
{
    public class ErrorCode
    {
        public string Code { get; set; }
        public string Message { get; set; }

        public static ErrorCode NotFound(string field) => new ErrorCode { Code = "404", Message = $"{field} not found" };
        public static ErrorCode ValidationError(string msg) => new ErrorCode { Code = "400", Message = msg };
        public static ErrorCode ServerError(string msg) => new ErrorCode { Code = "500", Message = msg };
    }
}
