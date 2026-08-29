namespace MobyLabWebProgramming.Services.DataTransferObjects;

/// <summary>
/// This DTO is used to add a sport, note that it doesn't have an id property because the id should be added by the application.
/// </summary>
public class SportAddRecord
{
    public string Name { get; set; } = null!;
}
