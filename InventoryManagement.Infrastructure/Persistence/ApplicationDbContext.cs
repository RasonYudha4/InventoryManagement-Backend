using System.Security.Claims;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Common;
using InventoryManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using InventoryManagement.Infrastructure.Identity;

namespace InventoryManagement.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<StockLevel> StockLevels => Set<StockLevel>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
        var userEmail = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email) ?? "system";
        var userRole = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role) ?? "";
        var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "";
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = userId;
            } 
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = userId;    
            }
        }

        var auditEntries = BuildAuditEntries(userId, userEmail, userRole, ip);

        var result = await base.SaveChangesAsync(ct);

        if (auditEntries.Any())
        {
            AuditLogs.AddRange(auditEntries);
            await base.SaveChangesAsync(ct);
        }

        return result;
    }

    private List<AuditLog> BuildAuditEntries(string userId, string email, string role, string ip)
    {
        var entries = new List<AuditLog>();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog) continue;
            if (entry.State is EntityState.Detached or EntityState.Unchanged) continue;

            var audit = new AuditLog
            {
                TableName = entry.Metadata.GetTableName() ?? entry.Entity.GetType().Name,
                Action = entry.State.ToString().ToUpper(),
                RecordId = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString(),
                UserId = userId,
                UserEmail = email,
                UserRole = role,
                IpAddress = ip,
                Timestamp = DateTime.UtcNow
            };

            if (entry.State == EntityState.Modified)
            {
                var oldVals = new Dictionary<string, object?>();
                var newVals = new Dictionary<string, object?>();
                var changed = new List<string>();

                foreach (var prop in entry.Properties.Where(p => p.IsModified))
                {
                    oldVals[prop.Metadata.Name] = prop.OriginalValue;
                    newVals[prop.Metadata.Name] = prop.CurrentValue;
                    changed.Add(prop.Metadata.Name);
                }

                audit.OldValues = JsonSerializer.Serialize(oldVals);
                audit.NewValues = JsonSerializer.Serialize(newVals);
                audit.ChangedColumns = string.Join(", ", changed);
            }
            else if (entry.State == EntityState.Added)
            {
                var newVals = entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
                audit.NewValues = JsonSerializer.Serialize(newVals);
            }
            else if (entry.State == EntityState.Deleted)
            {
                var oldVals = entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);
                audit.OldValues = JsonSerializer.Serialize(oldVals);
            }

            entries.Add(audit);
        }

        return entries;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
   
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        builder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
        builder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
        builder.Entity<Supplier>().HasQueryFilter(s => !s.IsDeleted);
        builder.Entity<Warehouse>().HasQueryFilter(w => !w.IsDeleted);
    
        // 1. Don't let deleting a Product delete historical PO Lines
        builder.Entity<PurchaseOrderLine>()
            .HasOne(pol => pol.Product)
            .WithMany(p => p.PurchaseOrderLines)
            .HasForeignKey(pol => pol.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // 2. Don't let deleting a Supplier delete historical POs
        builder.Entity<PurchaseOrder>()
            .HasOne(po => po.Supplier)
            .WithMany(s => s.PurchaseOrders)
            .HasForeignKey(po => po.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        // 3. Don't let deleting a Product delete historical Stock Transactions
        builder.Entity<StockTransaction>()
            .HasOne(st => st.Product)
            .WithMany(p => p.Transactions)
            .HasForeignKey(st => st.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}