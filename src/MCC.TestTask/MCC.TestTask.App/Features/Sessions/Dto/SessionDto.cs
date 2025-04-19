using MCC.TestTask.Domain;

namespace MCC.TestTask.App.Features.Sessions.Dto;

public class SessionDto
{
    public Guid Id { get; set; }

    public string? LastIp { get; set; }

    public DateTime ExpiresAfter { get; set; }
}