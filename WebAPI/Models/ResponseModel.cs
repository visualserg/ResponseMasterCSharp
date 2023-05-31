namespace WebAPI.Models;

public class ResponseModel<T>
{
    public bool IsSuccess { get; set; }
    public T? Result { get; set; }
    public string? Message { get; set; }
    public IList<Error?>? ValidationErrors { get; set; }
}