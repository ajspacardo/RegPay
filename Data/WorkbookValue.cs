using System.Globalization;

namespace RegPay3.Data;

public static class WorkbookValue
{
    public static object? Parse(string? value, Type propertyType)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var type = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        if (type == typeof(string)) return value;
        if (type == typeof(decimal)) return decimal.Parse(value, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture);
        if (type == typeof(int))
        {
            var number = decimal.Parse(value, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture);
            return decimal.ToInt32(number);
        }
        if (type == typeof(DateOnly))
        {
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var serial))
                return DateOnly.FromDateTime(DateTime.FromOADate(serial));
            return DateOnly.Parse(value, CultureInfo.InvariantCulture);
        }
        throw new NotSupportedException($"Unsupported workbook field type: {propertyType.Name}");
    }
}
