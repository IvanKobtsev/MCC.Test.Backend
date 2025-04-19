using System.Linq.Expressions;
using MCC.TestTask.App.Features.Communities.Dto;
using MCC.TestTask.App.Features.Users.Dto;
using MCC.TestTask.Domain;
using NeinLinq;

namespace MCC.TestTask.App.Features.Sessions.Dto;

public static class SessionDtoConverters
{
    private static readonly Lazy<Func<Session, SessionDto>> ToDtoCompiled = new(ToDto().Compile());

    [InjectLambda]
    public static SessionDto ToDto(this Session session)
    {
        return ToDtoCompiled.Value(session);
    }

    public static Expression<Func<Session, SessionDto>> ToDto()
    {
        return session => new SessionDto
        {
            Id = session.Id,
            LastIp = session.LastIp,
            ExpiresAfter = session.ExpiresAfter,
        };
    }
}