namespace NetCoreCrud.Api.Models
{
    public class CommonResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public CommonResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
