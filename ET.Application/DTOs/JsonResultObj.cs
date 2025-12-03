namespace ET.Application.DTOs;

public class JsonResultObj
{
    public bool IsSuccess { get; set; }
    public int? ErrorCode { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }

    public JsonResultObj(object data)
    {
        IsSuccess = true;
        Data = data;
    }

    public JsonResultObj(int errorCode, string message)
    {
        IsSuccess = false;
        ErrorCode = errorCode;
        Message = message;
    }
}
