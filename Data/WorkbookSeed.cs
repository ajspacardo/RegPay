using System.Reflection;
using System.Text.Json;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using RegPay3.Models;

namespace RegPay3.Data;

public static class WorkbookSeed
{
    public static IEnumerable<PropertyInfo> WorkbookProperties(Type type) => type.GetProperties()
        .Where(p => p.Name != "Id" && p.GetCustomAttribute<WorkbookColumnAttribute>() is not null)
        .OrderBy(p => p.GetCustomAttribute<WorkbookColumnAttribute>()!.Order);

    public static async Task EnsureSeededAsync(PayrollDbContext db)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "SeedData.json");
        await using var stream = File.OpenRead(path);
        using var document = await JsonDocument.ParseAsync(stream);
        await SeedMissingRowsAsync(db.PayrollRegisters, document.RootElement.GetProperty("PAYREG"));
        await SeedMissingRowsAsync(db.OvertimeRegisters, document.RootElement.GetProperty("OTREG"));
        await db.SaveChangesAsync();
    }

    private static async Task SeedMissingRowsAsync<TEntity>(DbSet<TEntity> set, JsonElement rows)
        where TEntity : class, new()
    {
        var existing = await set.AsNoTracking().ToListAsync();
        var existingCounts = existing
            .GroupBy(RowSignature)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);
        var missing = new List<TEntity>();

        foreach (var seedRow in ReadRows<TEntity>(rows))
        {
            var signature = RowSignature(seedRow);
            if (existingCounts.TryGetValue(signature, out var count) && count > 0)
                existingCounts[signature] = count - 1;
            else
                missing.Add(seedRow);
        }

        if (missing.Count > 0) await set.AddRangeAsync(missing);
    }

    private static string RowSignature<TEntity>(TEntity row) where TEntity : class => string.Join("|",
        WorkbookProperties(typeof(TEntity)).Select(property =>
        {
            var value = property.GetValue(row);
            var text = value is IFormattable formattable
                ? formattable.ToString(null, CultureInfo.InvariantCulture) ?? string.Empty
                : value?.ToString() ?? string.Empty;
            var isNull = value is null || value is string stringValue && string.IsNullOrWhiteSpace(stringValue);
            return $"{(isNull ? -1 : text.Length)}:{text}";
        }));

    private static IEnumerable<T> ReadRows<T>(JsonElement rows) where T : class, new()
    {
        var properties = typeof(T).GetProperties().Where(p => p.Name != "Id").ToDictionary(p =>
            p.GetCustomAttribute<WorkbookColumnAttribute>()!.StorageName, StringComparer.OrdinalIgnoreCase);
        foreach (var row in rows.EnumerateArray())
        {
            var entity = new T();
            foreach (var value in row.EnumerateObject())
                if (properties.TryGetValue(value.Name, out var property))
                    property.SetValue(entity, WorkbookValue.Parse(value.Value.GetString(), property.PropertyType));
            yield return entity;
        }
    }
}
