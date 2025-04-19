using FluentResults;
using MCC.TestTask.Domain;
using MCC.TestTask.Infrastructure;
using MCC.TestTask.Persistance;
using Microsoft.EntityFrameworkCore;

namespace MCC.TestTask.App.Features.Sessions;

public class SessionService
{
    private readonly BlogDbContext _blogDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionService(BlogDbContext blogDbContext, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _blogDbContext = blogDbContext;
    }

    private async Task DeleteExpiredSessions()
    {
        await _blogDbContext.Sessions
            .Where(s => s.ExpiresAfter < DateTime.UtcNow)
            .ExecuteDeleteAsync();
    }
    
    public async Task<Result<List<Session>>> GetSessions(Guid userId)
    {
        await DeleteExpiredSessions();
        
        return await _blogDbContext.Sessions.AsNoTracking().Where(s => s.UserId == userId).ToListAsync();
    }

    public async Task<Result<Session>> GetSession(Guid sessionId)
    {
        var session = await _blogDbContext.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        return session != null ? Result.Ok(session) : CustomErrors.NotFound("Session not found");
    }

    public async Task<Session> CreateNewSession(Guid userId, TimeSpan lifetime)
    {
        var sessionId = Guid.NewGuid();
        while (await _blogDbContext.Sessions.FindAsync(sessionId) != null)
            sessionId = Guid.NewGuid();

        // These two lines won't work if API is behind a proxy, so be mindful 
        var httpContext = _httpContextAccessor.HttpContext;
        var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();
        
        var session = new Session { Id = sessionId, UserId = userId, ExpiresAfter = DateTime.UtcNow.Add(lifetime), LastIp = ipAddress };
        await _blogDbContext.Sessions.AddAsync(session);
        return session;
    }

    public async Task<Result> DeleteSession(Guid sessionId, Guid userId)
    {
        var session = await _blogDbContext.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session == null || session.UserId != userId)
            return CustomErrors.NotFound("Session not found");

        _blogDbContext.Sessions.Remove(session);
        await _blogDbContext.SaveChangesAsync();
        
        return Result.Ok();
    }

    public async Task ClearSessions(Guid userId)
    {
        await _blogDbContext.Sessions.Where(s => s.UserId == userId).ExecuteDeleteAsync();
    }

    public async Task<Result> UpdateRefreshToken(Guid sessionId, string refreshToken, DateTime expiresAt)
    {
        var session = await _blogDbContext.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session == null)
            return CustomErrors.NotFound("Session not found");

        session.RefreshToken = refreshToken;
        session.ExpiresAfter = expiresAt;
        await _blogDbContext.SaveChangesAsync();
        
        return Result.Ok();
    }
}