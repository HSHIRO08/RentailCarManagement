namespace RentailCarManagement.Exceptions;

/// <summary>
/// Exception khi dữ liệu không hợp lệ
/// </summary>
public class ValidationException : Exception
{
    public List<string> Errors { get; }

    public ValidationException(string message) : base(message)
    {
        Errors = new List<string> { message };
    }

    public ValidationException(List<string> errors) : base("Dữ liệu không hợp lệ")
    {
        Errors = errors;
    }

    public ValidationException(string message, List<string> errors) : base(message)
    {
        Errors = errors;
    }
}
