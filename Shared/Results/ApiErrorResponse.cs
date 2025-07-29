using System.Collections.Generic;

namespace WebApplication1.Shared.Results
{
    public class ApiErrorResponse
    {
        public List<string> Errors { get; set; }

        public ApiErrorResponse()
        {
            Errors = new List<string>();
        }
    }
}