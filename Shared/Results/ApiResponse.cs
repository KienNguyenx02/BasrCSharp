namespace WebApplication1.Shared.Results
{
    public class ApiResponse<T>
    {
        public bool success { get; set; }
        public string? message { get; set; }
        public T? data { get; set; }

        public static ApiResponse<T> Ok(T data, string? message = null) => new() { success = true, data = data, message = message };
        public static ApiResponse<T> Fail(string message) => new() { success = false, message = message };
    }
}