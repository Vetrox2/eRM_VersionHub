using eRM_VersionHub.Services;

namespace eRM_VersionHub.Models
{
    public class ApiResponse<T>
    {
        public bool Success => Errors is null || Errors.Count() == 0;
        public T Data { get; set; }
        public List<string> Errors { get; set; }

        public ApiResponse()
        {
            Errors = new List<string>();
        }

        public static ApiResponse<T> SuccessResponse(T data)
        {
            return new ApiResponse<T>
            {
                Data = data
            };
        }

        public static ApiResponse<T> ErrorResponse(List<string> errors)
        {
            return new ApiResponse<T>
            {
                Errors = errors
            };
        }

        public override string ToString() => this.Serialize();
    }
}
