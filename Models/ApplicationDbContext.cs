using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RentailCarManagement.Models;

/// <summary>
/// Application DbContext với Identity Support
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid,
    IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    // DbSets for existing entities
    public virtual DbSet<Car> Cars { get; set; }
    public virtual DbSet<CarCategory> CarCategories { get; set; }
    public virtual DbSet<CarDocument> CarDocuments { get; set; }
    public virtual DbSet<CarImage> CarImages { get; set; }
    public virtual DbSet<ChatSession> ChatSessions { get; set; }
    public virtual DbSet<Commission> Commissions { get; set; }
    public virtual DbSet<Complaint> Complaints { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<Rental> Rentals { get; set; }
    public virtual DbSet<Review> Reviews { get; set; }
    public virtual DbSet<Supplier> Suppliers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure custom properties for Identity tables
        // DO NOT override table names or keys - base.OnModelCreating() already handles that
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Active");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        builder.Entity<ApplicationRole>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(256);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Configure existing entities
        ConfigureCarEntities(builder);
        ConfigureRentalEntities(builder);
        ConfigureUserRelatedEntities(builder);
    }

    private void ConfigureCarEntities(ModelBuilder builder)
    {
        builder.Entity<Car>(entity =>
        {
            entity.ToTable("cars");
            entity.HasKey(e => e.CarId);
            entity.HasIndex(e => e.LicensePlate).IsUnique();
            entity.Property(e => e.CarId).HasColumnName("car_id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.Brand).HasColumnName("brand").HasMaxLength(50);
            entity.Property(e => e.Model).HasColumnName("model").HasMaxLength(50);
            entity.Property(e => e.LicensePlate).HasColumnName("license_plate").HasMaxLength(20);
            entity.Property(e => e.PricePerDay).HasColumnName("price_per_day").HasPrecision(15, 2);
            entity.Property(e => e.PricePerHour).HasColumnName("price_per_hour").HasPrecision(15, 2);
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("Available");
            entity.Property(e => e.FuelType).HasColumnName("fuel_type").HasMaxLength(50);
            entity.Property(e => e.Transmission).HasColumnName("transmission").HasMaxLength(50);
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.Year).HasColumnName("year");
            entity.Property(e => e.Seats).HasColumnName("seats");
            entity.Property(e => e.IsApproved).HasColumnName("is_approved").HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Category).WithMany(p => p.Cars)
                .HasForeignKey(d => d.CategoryId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.Supplier).WithMany(p => p.Cars)
                .HasForeignKey(d => d.SupplierId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<CarCategory>(entity =>
        {
            entity.ToTable("car_categories");
            entity.HasKey(e => e.CategoryId);
            entity.Property(e => e.CategoryId).HasColumnName("category_id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        builder.Entity<CarImage>(entity =>
        {
            entity.ToTable("car_images");
            entity.HasKey(e => e.ImageId);
            entity.Property(e => e.ImageId).HasColumnName("image_id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.IsPrimary).HasColumnName("is_primary").HasDefaultValue(false);
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order").HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Car).WithMany(p => p.CarImages)
                .HasForeignKey(d => d.CarId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CarDocument>(entity =>
        {
            entity.ToTable("car_documents");
            entity.HasKey(e => e.DocumentId);
            entity.Property(e => e.DocumentId).HasColumnName("document_id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.DocumentType).HasColumnName("document_type").HasMaxLength(50);
            entity.Property(e => e.DocumentNumber).HasColumnName("document_number").HasMaxLength(100);
            entity.Property(e => e.DocumentUrl).HasColumnName("document_url");
            entity.Property(e => e.IssueDate).HasColumnName("issue_date");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.IsVerified).HasColumnName("is_verified").HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Car).WithMany(p => p.CarDocuments)
                .HasForeignKey(d => d.CarId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureRentalEntities(ModelBuilder builder)
    {
        builder.Entity<Rental>(entity =>
        {
            entity.ToTable("rentals");
            entity.HasKey(e => e.RentalId);
            entity.Property(e => e.RentalId).HasColumnName("rental_id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.TotalAmount).HasColumnName("total_amount").HasPrecision(15, 2);
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("Pending");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Car).WithMany(p => p.Rentals)
                .HasForeignKey(d => d.CarId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.Customer).WithMany(p => p.Rentals)
                .HasForeignKey(d => d.CustomerId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments");
            entity.HasKey(e => e.PaymentId);
            entity.HasIndex(e => e.RentalId).IsUnique();
            entity.Property(e => e.PaymentId).HasColumnName("payment_id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.RentalId).HasColumnName("rental_id");
            entity.Property(e => e.Amount).HasColumnName("amount").HasPrecision(15, 2);
            entity.Property(e => e.PaymentMethod).HasColumnName("payment_method").HasMaxLength(50);
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("Pending");
            entity.Property(e => e.PaidAt).HasColumnName("paid_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Rental).WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.RentalId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Review>(entity =>
        {
            entity.ToTable("reviews");
            entity.HasKey(e => e.ReviewId);
            entity.HasIndex(e => e.RentalId).IsUnique();
            entity.Property(e => e.ReviewId).HasColumnName("review_id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.RentalId).HasColumnName("rental_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.IsApproved).HasColumnName("is_approved").HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Rental).WithOne(p => p.Review)
                .HasForeignKey<Review>(d => d.RentalId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.Customer).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CustomerId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Commission>(entity =>
        {
            entity.ToTable("commissions");
            entity.HasKey(e => e.CommissionId);
            entity.HasIndex(e => e.RentalId).IsUnique();
            entity.Property(e => e.CommissionId).HasColumnName("commission_id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.RentalId).HasColumnName("rental_id");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.CommissionRate).HasColumnName("commission_rate").HasPrecision(5, 4);
            entity.Property(e => e.SystemAmount).HasColumnName("system_amount").HasPrecision(15, 2);
            entity.Property(e => e.SupplierAmount).HasColumnName("supplier_amount").HasPrecision(15, 2);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Rental).WithOne(p => p.Commission)
                .HasForeignKey<Commission>(d => d.RentalId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.Supplier).WithMany(p => p.Commissions)
                .HasForeignKey(d => d.SupplierId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureUserRelatedEntities(ModelBuilder builder)
    {
        builder.Entity<Customer>(entity =>
        {
            entity.ToTable("customers");
            entity.HasKey(e => e.CustomerId);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.Property(e => e.CustomerId).HasColumnName("customer_id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.DriverLicenseNumber).HasColumnName("driver_license_number").HasMaxLength(50);
            entity.Property(e => e.LicenseExpiryDate).HasColumnName("license_expiry_date");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne<ApplicationUser>().WithOne(u => u.Customer)
                .HasForeignKey<Customer>(d => d.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Supplier>(entity =>
        {
            entity.ToTable("suppliers");
            entity.HasKey(e => e.SupplierId);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CompanyName).HasColumnName("company_name").HasMaxLength(200);
            entity.Property(e => e.TaxCode).HasColumnName("tax_code").HasMaxLength(50);
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.BankAccount).HasColumnName("bank_account").HasMaxLength(50);
            entity.Property(e => e.BankName).HasColumnName("bank_name").HasMaxLength(100);
            entity.Property(e => e.IsVerified).HasColumnName("is_verified").HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne<ApplicationUser>().WithOne(u => u.Supplier)
                .HasForeignKey<Supplier>(d => d.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Complaint>(entity =>
        {
            entity.ToTable("complaints");
            entity.HasKey(e => e.ComplaintId);
            entity.Property(e => e.ComplaintId).HasColumnName("complaint_id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.RentalId).HasColumnName("rental_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(200);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Resolution).HasColumnName("resolution");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(50).HasDefaultValue("Pending");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ResolvedAt).HasColumnName("resolved_at");

            entity.HasOne(d => d.Rental).WithMany(p => p.Complaints)
                .HasForeignKey(d => d.RentalId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApplicationUser>().WithMany(u => u.Complaints)
                .HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ChatSession>(entity =>
        {
            entity.ToTable("chat_sessions");
            entity.HasKey(e => e.ChatSessionId);
            entity.Property(e => e.ChatSessionId).HasColumnName("chat_session_id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Subject).HasColumnName("subject").HasMaxLength(200);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.StartedAt).HasColumnName("started_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.EndedAt).HasColumnName("ended_at");

            entity.HasOne<ApplicationUser>().WithMany(u => u.ChatSessions)
                .HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Restrict);
        });

    }
}
