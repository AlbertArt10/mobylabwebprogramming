namespace MobyLabWebProgramming.Services.DataTransferObjects;

/// <summary>
/// This DTO is used to update a sport, the properties besides the id are nullable to indicate that they may not be updated if they are null.
/// </summary>
public record SportUpdateRecord(Guid Id, string? Name = null);
