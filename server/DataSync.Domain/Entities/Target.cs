namespace DataSync.Domain.Entities;

public class Target
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Connection { get; set; } = string.Empty;
    public string Status { get; set; } = "inactive";
}

