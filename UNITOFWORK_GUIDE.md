# 📚 Hướng dẫn Sử dụng Unit of Work Pattern

## ❌ **VẤN ĐỀ BAN ĐẦU**

Project đã có UnitOfWork nhưng **KHÔNG được sử dụng đúng cách**:

```csharp
// ❌ SAI - Không dùng transaction
public async Task CreateRentalAsync(...)
{
    await _unitOfWork.Rentals.AddAsync(rental);
    await _unitOfWork.SaveChangesAsync();  // Lưu ngay
    
    // Nếu có lỗi ở đây → Data inconsistent!
    await _unitOfWork.Payments.AddAsync(payment);
    await _unitOfWork.SaveChangesAsync();
}

// ❌ SAI - Inject cả UnitOfWork VÀ DbContext
public class CarService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;  // ← Redundant!
}
```

---

## ✅ **GIẢI PHÁP - DÙNG UNITOFWORK ĐÚNG CÁCH**

### **1. Transactions cho Operations Phức Tạp**

```csharp
public async Task<RentalDetailResponse> CreateRentalAsync(CreateRentalRequest request)
{
    // ✅ BẮT ĐẦU TRANSACTION
    await _unitOfWork.BeginTransactionAsync();
    
    try
    {
        // Validate và thực hiện operations
        var car = await _unitOfWork.Cars.GetByIdAsync(request.CarId);
        if (car == null)
            throw new NotFoundException("Xe không tồn tại");

        var rental = new Rental { ... };
        await _unitOfWork.Rentals.AddAsync(rental);
        
        // Tạo payment tương ứng
        var payment = new Payment { ... };
        await _unitOfWork.Payments.AddAsync(payment);
        
        // ✅ LƯU TẤT CẢ MỘT LẦN
        await _unitOfWork.SaveChangesAsync();
        
        // ✅ COMMIT TRANSACTION
        await _unitOfWork.CommitAsync();
        
        return result;
    }
    catch
    {
        // ✅ ROLLBACK NẾU CÓ LỖI
        await _unitOfWork.RollbackAsync();
        throw;
    }
}
```

---

### **2. Simple Operations - Không Cần Transaction**

```csharp
// ✅ Operations đơn giản không cần transaction
public async Task<bool> UpdateCarStatusAsync(Guid carId, string status)
{
    var car = await _unitOfWork.Cars.GetByIdAsync(carId);
    if (car == null)
        return false;

    car.Status = status;
    car.UpdatedAt = DateTime.UtcNow;

    _unitOfWork.Cars.Update(car);
    await _unitOfWork.SaveChangesAsync();

    return true;
}
```

---

### **3. Loại Bỏ Direct DbContext Dependency**

#### ❌ **TRƯỚC:**
```csharp
public class CarService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;  // ← BAD!

    public CarService(IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;  // ← Redundant
    }

    public async Task<IEnumerable<CarCategoryDto>> GetCategoriesAsync()
    {
        return await _context.CarCategories  // ← Direct DB access
            .Where(c => c.IsActive == true)
            .ToListAsync();
    }
}
```

#### ✅ **SAU:**
```csharp
public class CarService
{
    private readonly IUnitOfWork _unitOfWork;

    public CarService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CarCategoryDto>> GetCategoriesAsync()
    {
        // ✅ Access through UnitOfWork.Context
        return await _unitOfWork.Context.CarCategories
            .Where(c => c.IsActive == true)
            .ToListAsync();
    }
}
```

---

## 🎯 **KHI NÀO DÙNG TRANSACTION?**

### ✅ **CẦN TRANSACTION:**

1. **Multi-table operations:**
   ```csharp
   // Tạo Rental + Payment + Commission
   await _unitOfWork.BeginTransactionAsync();
   try {
       await _unitOfWork.Rentals.AddAsync(rental);
       await _unitOfWork.Payments.AddAsync(payment);
       await _unitOfWork.SaveChangesAsync();
       await _unitOfWork.CommitAsync();
   } catch {
       await _unitOfWork.RollbackAsync();
       throw;
   }
   ```

2. **Business logic phức tạp:**
   ```csharp
   // Complete rental → Update car status → Create review
   await _unitOfWork.BeginTransactionAsync();
   try {
       rental.Status = "Completed";
       car.Status = "Available";
       await _unitOfWork.SaveChangesAsync();
       await _unitOfWork.CommitAsync();
   } catch {
       await _unitOfWork.RollbackAsync();
       throw;
   }
   ```

3. **Financial operations:**
   ```csharp
   // Payment → Refund → Update balance
   await _unitOfWork.BeginTransactionAsync();
   try {
       payment.Status = "Refunded";
       // ... more operations
       await _unitOfWork.SaveChangesAsync();
       await _unitOfWork.CommitAsync();
   } catch {
       await _unitOfWork.RollbackAsync();
       throw;
   }
   ```

### ❌ **KHÔNG CẦN TRANSACTION:**

1. **Single entity CRUD:**
   ```csharp
   // Chỉ update 1 entity
   car.Status = "Available";
   _unitOfWork.Cars.Update(car);
   await _unitOfWork.SaveChangesAsync();
   ```

2. **Read-only operations:**
   ```csharp
   // Chỉ query, không modify
   var cars = await _unitOfWork.Cars.GetAvailableCarsAsync(start, end);
   return cars;
   ```

---

## 📋 **BEST PRACTICES**

### **1. Một SaveChangesAsync cho mỗi transaction**

```csharp
// ❌ BAD - Multiple saves
await _unitOfWork.Rentals.AddAsync(rental);
await _unitOfWork.SaveChangesAsync();  // ← Save 1
await _unitOfWork.Payments.AddAsync(payment);
await _unitOfWork.SaveChangesAsync();  // ← Save 2

// ✅ GOOD - Single save
await _unitOfWork.Rentals.AddAsync(rental);
await _unitOfWork.Payments.AddAsync(payment);
await _unitOfWork.SaveChangesAsync();  // ← Save once
```

### **2. Always Rollback on Exception**

```csharp
// ✅ GOOD
await _unitOfWork.BeginTransactionAsync();
try
{
    // ... operations
    await _unitOfWork.SaveChangesAsync();
    await _unitOfWork.CommitAsync();
}
catch
{
    await _unitOfWork.RollbackAsync();  // ← IMPORTANT!
    throw;
}
```

### **3. Dispose Pattern**

```csharp
// ✅ GOOD - Service inject IUnitOfWork
// DI container will handle disposal

// ❌ BAD - Manual UnitOfWork creation
using var unitOfWork = new UnitOfWork(context);
// ... use unitOfWork
```

### **4. Prefer Repository Methods over Direct DbContext**

```csharp
// ❌ AVOID
var cars = await _unitOfWork.Context.Cars
    .Include(c => c.Category)
    .Where(c => c.Status == "Available")
    .ToListAsync();

// ✅ PREFER - Add to repository
public interface ICarRepository
{
    Task<IEnumerable<Car>> GetAvailableCarsWithDetailsAsync();
}
```

---

## 🔄 **TRANSACTION FLOW DIAGRAM**

```
┌─────────────────────────────────────┐
│   Begin Transaction                 │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│   Validate Business Rules           │
│   - Check car availability          │
│   - Validate dates                  │
│   - Check user permissions          │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│   Perform Database Operations       │
│   - Add/Update/Delete entities      │
│   - Multiple repositories           │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│   SaveChangesAsync()                │
│   (Single call at the end)          │
└──────────────┬──────────────────────┘
               │
        ┌──────┴──────┐
        │             │
        ▼             ▼
   ┌────────┐   ┌────────────┐
   │Success │   │  Exception │
   └───┬────┘   └─────┬──────┘
       │              │
       ▼              ▼
   ┌────────┐   ┌────────────┐
   │ Commit │   │  Rollback  │
   └────────┘   └────────────┘
```

---

## 📝 **CHECKLIST REFACTORING**

Khi refactor services để dùng UnitOfWork đúng:

- [ ] Remove `ApplicationDbContext` từ constructor
- [ ] Chỉ inject `IUnitOfWork`
- [ ] Identify operations cần transaction (multi-table, financial)
- [ ] Wrap trong `BeginTransaction / Commit / Rollback`
- [ ] Move `SaveChangesAsync()` về cuối, trước `CommitAsync()`
- [ ] Add try-catch với rollback
- [ ] Test thoroughly (happy path + error cases)

---

## 🚀 **EXAMPLE: Complete Rental Flow**

```csharp
public async Task<RentalDetailResponse> CompleteRentalAsync(Guid rentalId)
{
    await _unitOfWork.BeginTransactionAsync();
    
    try
    {
        // 1. Get rental
        var rental = await _unitOfWork.Rentals.GetRentalWithDetailsAsync(rentalId);
        if (rental == null)
            throw new NotFoundException("Rental not found");

        if (rental.Status != "Active")
            throw new BusinessException("Rental is not active");

        // 2. Complete rental
        rental.Status = "Completed";
        rental.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Rentals.Update(rental);

        // 3. Update car status
        var car = await _unitOfWork.Cars.GetByIdAsync(rental.CarId);
        car!.Status = "Available";
        car.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Cars.Update(car);

        // 4. Calculate commission
        var commissionRate = 0.15m;
        var systemAmount = rental.TotalAmount * commissionRate;
        var supplierAmount = rental.TotalAmount - systemAmount;

        var commission = new Commission
        {
            CommissionId = Guid.NewGuid(),
            RentalId = rentalId,
            SupplierId = car.SupplierId,
            CommissionRate = commissionRate,
            SystemAmount = systemAmount,
            SupplierAmount = supplierAmount,
            CreatedAt = DateTime.UtcNow
        };
        
        await _unitOfWork.Context.Commissions.AddAsync(commission);

        // 5. Save all changes once
        await _unitOfWork.SaveChangesAsync();

        // 6. Commit transaction
        await _unitOfWork.CommitAsync();

        return await GetRentalDetailsAsync(rentalId);
    }
    catch
    {
        await _unitOfWork.RollbackAsync();
        throw;
    }
}
```

---

## 🐛 **COMMON MISTAKES**

### **1. Forgetting to Commit**
```csharp
// ❌ BAD
await _unitOfWork.BeginTransactionAsync();
// ... operations
await _unitOfWork.SaveChangesAsync();
// Missing CommitAsync()!  ← Transaction still open!
```

### **2. Multiple SaveChangesAsync in Transaction**
```csharp
// ❌ BAD - Unnecessary complexity
await _unitOfWork.BeginTransactionAsync();
await _unitOfWork.SaveChangesAsync();  // ← Too early
// ... more operations
await _unitOfWork.SaveChangesAsync();  // ← Again?
await _unitOfWork.CommitAsync();
```

### **3. Not Handling Rollback**
```csharp
// ❌ BAD - No error handling
await _unitOfWork.BeginTransactionAsync();
// ... operations might fail
await _unitOfWork.CommitAsync();  // ← What if error occurs?
```

---

## 📚 **TÀI LIỆU LIÊN QUAN**

- Martin Fowler's Unit of Work: https://martinfowler.com/eaaCatalog/unitOfWork.html
- EF Core Transactions: https://learn.microsoft.com/en-us/ef/core/saving/transactions
- Repository Pattern: https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design

---

## ✅ **KẾT QUẢ SAU KHI REFACTOR**

✅ **Data consistency** - Transactions đảm bảo ACID  
✅ **Clean architecture** - Services không phụ thuộc trực tiếp vào DbContext  
✅ **Testable** - Dễ mock IUnitOfWork  
✅ **Maintainable** - Clear separation of concerns  
✅ **Rollback support** - Tự động rollback khi có lỗi  
✅ **Performance** - Single SaveChanges instead of multiple calls
