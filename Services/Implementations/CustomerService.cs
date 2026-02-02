using RentailCarManagement.DTOs.Customer;
using RentailCarManagement.Models;
using RentailCarManagement.Repositories.Interfaces;
using RentailCarManagement.Services.Interfaces;

namespace RentailCarManagement.Services.Implementations;

/// <summary>
/// Customer Service Implementation
/// </summary>
public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomerResponse?> GetCustomerProfileAsync(Guid customerId)
    {
        var customer = await _unitOfWork.Customers.GetCustomerWithRentalsAsync(customerId);
        if (customer == null)
            return null;

        var stats = await _unitOfWork.Customers.GetCustomerStatsAsync(customerId);

        return MapToCustomerResponse(customer, stats.TotalRentals, stats.TotalSpent);
    }

    public async Task<CustomerResponse?> GetCustomerByUserIdAsync(Guid userId)
    {
        var customer = await _unitOfWork.Customers.GetCustomerByUserIdAsync(userId);
        if (customer == null)
            return null;

        var stats = await _unitOfWork.Customers.GetCustomerStatsAsync(customer.CustomerId);

        return MapToCustomerResponse(customer, stats.TotalRentals, stats.TotalSpent);
    }

    public async Task<CustomerResponse?> UpdateCustomerProfileAsync(Guid customerId, UpdateCustomerRequest request)
    {
        var customer = await _unitOfWork.Customers.GetCustomerByUserIdAsync(customerId);
        if (customer == null)
            return null;

        if (!string.IsNullOrEmpty(request.DriverLicenseNumber))
            customer.DriverLicenseNumber = request.DriverLicenseNumber;
        if (request.LicenseExpiryDate.HasValue)
            customer.LicenseExpiryDate = request.LicenseExpiryDate.Value;
        if (!string.IsNullOrEmpty(request.Address))
            customer.Address = request.Address;

        // Update user info if provided
        if (customer.User != null)
        {
            if (!string.IsNullOrEmpty(request.FullName))
                customer.User.FullName = request.FullName;
            if (!string.IsNullOrEmpty(request.Phone))
                customer.User.PhoneNumber = request.Phone;
        }

        _unitOfWork.Customers.Update(customer);
        await _unitOfWork.SaveChangesAsync();

        return await GetCustomerProfileAsync(customerId);
    }

    public async Task<(int TotalRentals, int CompletedRentals, decimal TotalSpent)> GetCustomerStatsAsync(Guid customerId)
    {
        var (totalRentals, totalSpent) = await _unitOfWork.Customers.GetCustomerStatsAsync(customerId);
        return (totalRentals, totalRentals, totalSpent);
    }

    public async Task<bool> ValidateDriverLicenseAsync(Guid customerId, DateTime rentalDate)
    {
        return await _unitOfWork.Customers.IsLicenseValidAsync(customerId, rentalDate);
    }

    private CustomerResponse MapToCustomerResponse(Customer customer, int totalRentals, decimal totalSpent)
    {
        return new CustomerResponse
        {
            CustomerId = customer.CustomerId,
            UserId = customer.UserId,
            FullName = customer.User?.FullName,
            Email = customer.User?.Email,
            Phone = customer.User?.PhoneNumber,
            DriverLicenseNumber = customer.DriverLicenseNumber,
            LicenseExpiryDate = customer.LicenseExpiryDate,
            Address = customer.Address,
            CreatedAt = customer.CreatedAt,
            TotalRentals = totalRentals,
            CompletedRentals = totalRentals,
            TotalSpent = totalSpent
        };
    }
}
