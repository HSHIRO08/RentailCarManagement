namespace RentailCarManagement.Exceptions;

/// <summary>
/// Exception khi không có quyền truy cập
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException() : base("Bạn không có quyền thực hiện hành động này")
    {
    }
}
