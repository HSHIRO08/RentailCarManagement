namespace RentailCarManagement.Exceptions;

/// <summary>
/// Exception khi xe không khả dụng
/// </summary>
public class CarNotAvailableException : BusinessException
{
    public Guid CarId { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    public CarNotAvailableException(Guid carId, DateTime startDate, DateTime endDate)
        : base($"Xe không khả dụng từ {startDate:dd/MM/yyyy} đến {endDate:dd/MM/yyyy}")
    {
        CarId = carId;
        StartDate = startDate;
        EndDate = endDate;
    }
}
