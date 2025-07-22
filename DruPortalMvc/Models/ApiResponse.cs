namespace DruPortalMvc.Models
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public object Error { get; set; }
    }
}
