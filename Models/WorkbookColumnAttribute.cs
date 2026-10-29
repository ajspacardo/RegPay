namespace RegPay3.Models;

[AttributeUsage(AttributeTargets.Property)]
public sealed class WorkbookColumnAttribute(string header, string storageName, int order) : Attribute
{
    public string Header { get; } = header;
    public string StorageName { get; } = storageName;
    public int Order { get; } = order;
}
