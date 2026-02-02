namespace RentailCarManagement.Exceptions;

/// <summary>
/// Exception khi ngày thuê không hợp lệ
/// </summary>
public class InvalidRentalDateException : BusinessException
{
    public InvalidRentalDateException(string message) : base(message)
    {
    }

    public InvalidRentalDateException(DateTime startDate, DateTime endDate)
        : base($"Ngày thuê không hợp lệ: từ {startDate:dd/MM/yyyy} đến {endDate:dd/MM/yyyy}")
    {
    }
}
