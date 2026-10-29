using Microsoft.EntityFrameworkCore;
using RegPay3.Models;

namespace RegPay3.Data;

public sealed class PayrollDbContext(DbContextOptions<PayrollDbContext> options) : DbContext(options)
{
    public DbSet<PayrollRegister> PayrollRegisters => Set<PayrollRegister>();
    public DbSet<OvertimeRegister> OvertimeRegisters => Set<OvertimeRegister>();
    public DbSet<TimeAttendance> TimeAttendances => Set<TimeAttendance>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Configure<PayrollRegister>(modelBuilder, "PAYREG");
        Configure<OvertimeRegister>(modelBuilder, "OTREG");
        Configure<TimeAttendance>(modelBuilder, "TimeAttendance");
    }

    private static void Configure<TEntity>(ModelBuilder builder, string table) where TEntity : class
    {
        var entity = builder.Entity<TEntity>();
        entity.ToTable(table);
        entity.HasKey("Id");
        entity.Property<long>("Id")
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();
        foreach (var property in typeof(TEntity).GetProperties().Where(p => p.Name != "Id"))
        {
            var column = (RegPay3.Models.WorkbookColumnAttribute)Attribute.GetCustomAttribute(property, typeof(RegPay3.Models.WorkbookColumnAttribute))!;
            var clrType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            var sqlType = clrType == typeof(DateOnly) ? "date"
                : clrType == typeof(decimal) ? "numeric"
                : clrType == typeof(int) ? "integer"
                : "text";
            entity.Property(property.PropertyType, property.Name)
                .HasColumnName(column.StorageName)
                .HasColumnType(sqlType)
                .IsRequired(false);
        }
    }
}
