using FluentResults.Extensions;
using MCC.TestTask.App.Features.Sessions.Dto;
using MCC.TestTask.App.Services.Auth;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCC.TestTask.App.Features.Sessions;

[Route("api/session")]
[Authorize]
public class SessionsController : ControllerBase
{
    private readonly SessionService _sessionService;
    private readonly UserAccessor _userAccessor;

    public SessionsController(SessionService sessionService, UserAccessor userAccessor)
    {
        _sessionService = sessionService;
        _userAccessor = userAccessor;
    }

    [HttpGet]
    public async Task<ActionResult<List<SessionDto>>> GetSessions()
    {
        return await _userAccessor.GetUserId()
            .Bind(userId => _sessionService.GetSessions(userId).Map(sl => sl.Select(s => s.ToDto())))
            .ToActionResult();
    }

    [HttpGet("current")]
    public async Task<ActionResult<SessionDto>> GetCurrentSession()
    {
        return await _userAccessor.GetSessionId()
            .Bind(sessionId => _sessionService.GetSession(sessionId).Map(s => s.ToDto()))
            .ToActionResult();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> EndSession(Guid id)
    {
        return await _userAccessor.GetUserId()
            .Bind(userId => _sessionService.DeleteSession(id, userId))
            .ToActionResult();
    }
}