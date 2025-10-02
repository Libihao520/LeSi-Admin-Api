namespace LeSi.Admin.Contracts.ApiResponse;

public class ApiResponse<T>
{
    public int Code { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
    public bool Success => Code == 200;
    public object? Errors { get; set; }

    // 成功响应
    public ApiResponse(T? data, string message = "操作成功")
    {
        Code = 200;
        Data = data;
        Message = message;
    }

    // 失败响应
    public ApiResponse(int errorCode, string errorMessage, object? errors)
    {
        Code = errorCode;
        Message = errorMessage;
        Errors = errors;
    }
}