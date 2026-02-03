# PROMPT CHI TIẾT: CẬP NHẬT DỰ ÁN ASP.NET CORE API - HỆ THỐNG CHO THUÊ XE Ô TÔ

## I. THÔNG TIN DỰ ÁN HIỆN TẠI

### Tên dự án: RentalCarManagement
### Công nghệ: ASP.NET Core Web API
### Chủ đề: Hệ thống quản lý cho thuê xe ô tô

---

## II. CẤU TRÚC DỰ ÁN HIỆN TẠI (Đã phân tích từ Solution Explorer)

### 1. **Models Layer** (RentalCarManagement.Models)
Các entity chính đã có:

#### **Core Entities:**
- `Car.cs` - Thông tin xe
- `CarCategory.cs` - Danh mục xe (sedan, SUV, ...)
- `CarDocument.cs` - Giấy tờ xe (đăng kiểm, bảo hiểm, ...)
- `CarImage.cs` - Hình ảnh xe
- `Rental.cs` - Thông tin thuê xe
- `Payment.cs` - Thanh toán
- `Review.cs` - Đánh giá
- `Customer.cs` - Khách hàng
- `Supplier.cs` - Nhà cung cấp/Chủ xe

#### **Support Entities:**
- `ChatMessage.cs` - Tin nhắn chat
- `ChatSession.cs` - Phiên chat
- `Commission.cs` - Hoa hồng
- `Complaint.cs` - Khiếu nại

#### **Identity Entities:**
- `AspNetUsers.cs`
- `AspNetRoles.cs`
- `AspNetUserClaims.cs`
- `AspNetUserLogins.cs`
- `AspNetUserTokens.cs`
- `AspNetRoleClaims.cs`

#### **DbContext:**
- `AppDbContext.cs` - Database context với EF Core

---

## III. YÊU CẦU CẬP NHẬT VÀ CẢI THIỆN

### A. PHÂN TÍCH VÀ ĐÁNH GIÁ

**Bạn (AI Assistant) hãy:**

1. **Đọc kỹ và phân tích toàn bộ code hiện tại:**
   - Xem xét chi tiết từng file trong Models
   - Kiểm tra các relationships (1-1, 1-n, n-n)
   - Đánh giá cấu trúc database schema
   - Xác định các properties, data annotations, navigation properties

2. **Xác định các vấn đề và thiếu sót:**
   - Missing properties quan trọng
   - Relationships chưa đúng hoặc thiếu
   - Data validation chưa đầy đủ
   - Business logic chưa được cover

---

### B. CẢI THIỆN MODELS LAYER

**Yêu cầu cụ thể cho từng Entity:**

#### 1. **Car Model** - Bổ sung đầy đủ
```
Cần có các thuộc tính:
- Thông tin cơ bản: Brand, Model, Year, Color, LicensePlate, VIN
- Thông số kỹ thuật: EngineType, FuelType, Transmission, Seats, Mileage
- Trạng thái: Status (Available, Rented, Maintenance, Retired)
- Giá cả: DailyRate, WeeklyRate, MonthlyRate, Deposit
- Vị trí: CurrentLocation, ParkingAddress
- Audit: CreatedDate, UpdatedDate, IsDeleted
- Relationships với: CarCategory, CarImages, CarDocuments, Rentals, Reviews
```

#### 2. **Rental Model** - Hoàn thiện
```
Thuộc tính cần có:
- RentalCode (unique)
- StartDate, EndDate, ActualReturnDate
- PickupLocation, ReturnLocation
- TotalDays, TotalAmount, DepositAmount
- Status: Pending, Confirmed, Active, Completed, Cancelled
- PaymentStatus: Unpaid, Partial, Paid, Refunded
- DriverInfo: DriverName, DriverLicense, DriverPhone
- SpecialRequests (GPS, child seat, ...)
- CancellationReason, CancellationDate
- Relationships: Customer, Car, Payments, Reviews
```

#### 3. **Payment Model**
```
Cần bổ sung:
- PaymentCode, PaymentMethod (Cash, Card, Transfer, Wallet)
- PaymentType (Deposit, Rental, Extra, Refund)
- Amount, PaymentDate, TransactionId
- PaymentGateway, PaymentStatus
- Notes
```

#### 4. **Review Model**
```
Thuộc tính:
- Rating (1-5), Comment
- ReviewDate, IsVerified
- ReviewType (Car, Service, Driver)
- Helpful count (upvote/downvote)
- Response từ Supplier
```

#### 5. **Customer Model**
```
Đầy đủ thông tin:
- Personal: FullName, Email, Phone, DateOfBirth, Gender
- Address: Street, City, District, Ward
- Documents: IDCard, DriverLicense, DriverLicenseExpiry
- Status: Active, Suspended, Blocked
- Loyalty: MembershipLevel, LoyaltyPoints
- Preferences: PreferredCarType, PreferredPaymentMethod
```

#### 6. **Supplier Model** (Chủ xe/Đối tác)
```
Thông tin:
- Company or Individual
- BusinessLicense, TaxCode
- BankAccount info
- Commission rate
- Rating, TotalCars
- Status: Active, Pending, Suspended
```

#### 7. **CarDocument Model**
```
Các loại giấy tờ:
- DocumentType (Registration, Insurance, Inspection, Ownership)
- DocumentNumber, IssueDate, ExpiryDate
- IssuedBy, Status
- FilePath (lưu file scan)
```

#### 8. **Commission Model**
```
Tính hoa hồng:
- CommissionType (Percentage, Fixed)
- Rate/Amount
- CalculatedAmount
- PaymentStatus, PaymentDate
- Liên kết với Rental và Supplier
```

---

### C. BỔ SUNG CÁC ENTITY MỚI (Nếu chưa có)

#### 1. **Promotion/Coupon**
```csharp
- CouponCode, DiscountType, DiscountValue
- ValidFrom, ValidTo
- MinRentalDays, MaxDiscount
- UsageLimit, UsedCount
- ApplicableCarCategories
```

#### 2. **Insurance**
```csharp
- InsuranceType (Basic, Premium, Full)
- Coverage, Price
- Terms and Conditions
```

#### 3. **MaintenanceRecord**
```csharp
- Car reference
- MaintenanceType, Description
- MaintenanceDate, Cost
- NextMaintenanceDate, NextMaintenanceMileage
- PerformedBy
```

#### 4. **Notification**
```csharp
- User reference
- Title, Message, Type
- IsRead, SentDate
- RelatedEntityType, RelatedEntityId
```

#### 5. **RentalExtension**
```csharp
- Original Rental reference
- ExtendedDays, NewEndDate
- AdditionalAmount
- RequestDate, ApprovalStatus
```

#### 6. **Damage Report**
```csharp
- Rental reference
- ReportedBy, ReportDate
- DamageType, Description, Severity
- EstimatedCost, ActualCost
- Photos, Status
```

#### 7. **BlockedDate**
```csharp
- Car reference
- StartDate, EndDate
- Reason (Maintenance, Reserved, Holiday)
```

---

### D. DATABASE RELATIONSHIPS - XÁC ĐỊNH RÕ RÀNG

**Hãy implement đầy đủ:**

1. **One-to-Many:**
   - CarCategory → Cars
   - Car → CarImages
   - Car → CarDocuments
   - Car → Rentals
   - Car → Reviews
   - Customer → Rentals
   - Rental → Payments
   - Supplier → Cars

2. **Many-to-Many:**
   - Car ↔ Insurance (CarInsurances)
   - Rental ↔ Promotion (RentalPromotions)

3. **One-to-One:**
   - Review → Rental (mỗi rental có 1 review)

**Cấu hình Fluent API trong AppDbContext:**
- Cascade delete rules
- Index cho các trường tìm kiếm thường xuyên
- Default values
- Check constraints
- Unique constraints

---

### E. DATA ANNOTATIONS VÀ VALIDATION

**Mỗi model phải có:**

```csharp
[Required]
[StringLength(max, MinimumLength = min)]
[EmailAddress]
[Phone]
[Range(min, max)]
[RegularExpression("pattern")]
[Display(Name = "...")]
[DataType(DataType.Date/Currency/...)]
[Compare("OtherProperty")]
[CreditCard]
[Url]
```

**Custom Validations:**
- Validate StartDate < EndDate
- Validate DriverLicenseExpiry > RentalStartDate
- Validate Age >= 21 for driver
- Validate Car availability

---

### G. DTOs (Data Transfer Objects)

**Tạo folder DTOs với:**

1. **Request DTOs:**
   - CreateCarRequest, UpdateCarRequest
   - CreateRentalRequest, UpdateRentalRequest
   - CreatePaymentRequest
   - CreateReviewRequest
   - LoginRequest, RegisterRequest

2. **Response DTOs:**
   - CarResponse, CarDetailResponse
   - RentalResponse, RentalDetailResponse
   - CustomerResponse
   - PaymentResponse
   - ReviewResponse

3. **Filter/Search DTOs:**
   - CarSearchCriteria (location, price range, dates, category, features)
   - RentalFilterCriteria
   - ReviewFilterCriteria

---

### H. REPOSITORY PATTERN

**Tạo Infrastructure Layer với:**

1. **Generic Repository:**
```csharp
IRepository<T>
- GetAll(), GetById(), Add(), Update(), Delete()
- Find(expression), GetWithInclude()
```

2. **Specific Repositories:**
   - ICarRepository: GetAvailableCars(), SearchCars(), GetCarsByCategory()
   - IRentalRepository: GetActiveRentals(), GetRentalHistory(), CheckAvailability()
   - IPaymentRepository: GetPaymentsByRental(), GetPaymentHistory()
   - ICustomerRepository: GetCustomerWithRentals(), GetLoyaltyInfo()

3. **Unit of Work:**
```csharp
IUnitOfWork
- Cars, Rentals, Payments, Customers, Reviews...
- SaveChanges(), BeginTransaction(), Commit(), Rollback()
```

---

### I. SERVICE LAYER

**Business Logic Services:**

1. **CarService:**
   - SearchAvailableCars()
   - GetCarDetails()
   - CheckCarAvailability(carId, startDate, endDate)
   - UpdateCarStatus()
   - CalculateRentalPrice(carId, days)

2. **RentalService:**
   - CreateRental()
   - ConfirmRental()
   - StartRental()
   - CompleteRental()
   - CancelRental()
   - ExtendRental()
   - CalculateTotalAmount()

3. **PaymentService:**
   - ProcessPayment()
   - ProcessRefund()
   - VerifyPayment()
   - GetPaymentHistory()

4. **ReviewService:**
   - CreateReview()
   - VerifyReview()
   - RespondToReview()
   - CalculateAverageRating()

5. **NotificationService:**
   - SendRentalConfirmation()
   - SendPaymentReminder()
   - SendReturnReminder()

---

### J. CONTROLLERS

**Các API endpoints cần có:**

#### 1. CarsController
```
GET /api/cars - Search cars with filters
GET /api/cars/{id} - Get car details
GET /api/cars/available - Get available cars
GET /api/cars/categories - Get car categories
POST /api/cars - Create car (Supplier/Admin)
PUT /api/cars/{id} - Update car
DELETE /api/cars/{id} - Delete car
GET /api/cars/{id}/reviews - Get car reviews
```

#### 2. RentalsController
```
POST /api/rentals - Create rental booking
GET /api/rentals/{id} - Get rental details
PUT /api/rentals/{id}/confirm - Confirm rental
PUT /api/rentals/{id}/start - Start rental
PUT /api/rentals/{id}/complete - Complete rental
PUT /api/rentals/{id}/cancel - Cancel rental
POST /api/rentals/{id}/extend - Extend rental
GET /api/rentals/my-rentals - Get customer's rentals
```

#### 3. PaymentsController
```
POST /api/payments - Process payment
GET /api/payments/{id} - Get payment details
POST /api/payments/{id}/refund - Process refund
GET /api/rentals/{rentalId}/payments - Get rental payments
```

#### 4. ReviewsController
```
POST /api/reviews - Create review
GET /api/reviews/{id} - Get review
PUT /api/reviews/{id}/verify - Verify review (Admin)
POST /api/reviews/{id}/respond - Supplier response
```

#### 5. CustomersController
```
GET /api/customers/profile - Get current customer
PUT /api/customers/profile - Update profile
GET /api/customers/rentals - Rental history
GET /api/customers/loyalty - Loyalty points
```

#### 6. AuthController
```
POST /api/auth/register - Register
POST /api/auth/login - Login
POST /api/auth/refresh-token - Refresh JWT
POST /api/auth/forgot-password
POST /api/auth/reset-password
```

---

### K. AUTHENTICATION & AUTHORIZATION

**Implement:**

1. JWT Authentication
2. Role-based Authorization (Admin, Customer, Supplier, Staff)
3. Claim-based Authorization
4. Custom Authorization Policies:
   - CanManageCar (chỉ chủ xe hoặc admin)
   - CanModifyRental (chỉ customer của rental đó)
   - CanProcessPayment

---

### L. ADVANCED FEATURES

**Bổ sung các tính năng nâng cao:**

1. **Search & Filtering:**
   - Full-text search
   - Filter by: price, location, category, features, rating
   - Sorting options
   - Pagination

2. **Pricing Engine:**
   - Dynamic pricing based on demand
   - Weekend/holiday rates
   - Long-term rental discounts
   - Seasonal pricing

3. **Availability System:**
   - Real-time availability check
   - Blocked dates management
   - Buffer time between rentals

4. **Notification System:**
   - Email notifications (SendGrid/SMTP)
   - SMS notifications (Twilio)
   - Push notifications
   - In-app notifications

5. **File Management:**
   - Image upload (car photos, documents)
   - Document storage (Azure Blob/AWS S3)
   - Image optimization

6. **Reporting:**
   - Revenue reports
   - Rental statistics
   - Popular cars
   - Customer insights

7. **Caching:**
   - Redis for frequently accessed data
   - Memory cache for categories, settings

8. **Background Jobs:**
   - Send scheduled notifications
   - Update car status
   - Calculate commissions
   - Clean up expired bookings

---

### M. ERROR HANDLING & LOGGING

1. **Global Exception Handler**
2. **Custom Exception Classes:**
   - CarNotAvailableException
   - InvalidRentalDateException
   - PaymentFailedException
   - UnauthorizedAccessException

3. **Logging (Serilog):**
   - Log all API requests
   - Log errors with stack trace
   - Log business operations

---

### N. API DOCUMENTATION

1. **Swagger/OpenAPI:**
   - Detailed API docs
   - Request/Response examples
   - Authentication setup

2. **XML Documentation Comments**

---

### O. TESTING (Optional nhưng recommended)

1. **Unit Tests:**
   - Services layer tests
   - Repository tests
   - Validation tests

2. **Integration Tests:**
   - API endpoint tests
   - Database integration tests

---

## IV. OUTPUT MONG MUỐN

### Kết quả cuối cùng cần có:

📁 **Models/**
  - Tất cả entities đầy đủ với properties, validations, relationships
  - Enums
  - Configuration classes (Fluent API)

📁 **DTOs/**
  - Request DTOs
  - Response DTOs
  - Filter/Search DTOs

📁 **Repositories/**
  - IRepository<T>
  - Specific repository interfaces
  - Repository implementations
  - IUnitOfWork

📁 **Services/**
  - Service interfaces
  - Service implementations
  - Business logic

📁 **Controllers/**
  - Các API controllers với full CRUD operations
  - Proper HTTP status codes
  - Model validation

📁 **Middleware/**
  - Error handling
  - Logging
  - Authentication

📁 **Migrations/**
  - Database migration files

📄 **appsettings.json**
  - Connection strings
  - JWT settings
  - External services config

📄 **Program.cs**
  - Dependency injection setup
  - Middleware configuration
  - Authentication/Authorization setup
