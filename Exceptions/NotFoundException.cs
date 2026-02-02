namespace RentailCarManagement.Exceptions;

/// <summary>
/// Exception khi không tìm thấy tài nguyên
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entityName, object key) 
        : base($"{entityName} với ID {key} không tồn tại")
    {
    }
}
