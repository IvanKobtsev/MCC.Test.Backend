﻿using MCC.TestTask.Persistance;
using MCC.TestTask.Infrastructure;
using FluentResults;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace MCC.TestTask.App.Services.Mail;

public class MailJobService
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly BlogDbContext _blogDbContext;

    public MailJobService(IBackgroundJobClient backgroundJobClient, BlogDbContext blogDbContext)
    {
        _backgroundJobClient = backgroundJobClient;
        _blogDbContext = blogDbContext;
    }

    public async Task<Result> NotifySubscribersAboutNewPost(Guid postId)
    {
        var post = await _blogDbContext.Posts
            .Include(p => p.Community).ThenInclude(c => c!.Subscribers)
            .Include(p => p.Author)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == postId);

        if (post?.Community == null)
            return CustomErrors.ValidationError("Post not found or doesn't have a Community");

        var mailSubject = $"New post from {post.Author.FullName} in {post.Community!.Name}";

        var mailBody = post.Title + ":\n" + post.Description;

        foreach (var subscriber in post.Community.Subscribers)
            _backgroundJobClient.Enqueue<MailingService>(j =>
                j.SendMail(subscriber.FullName, subscriber.Email, mailSubject, mailBody));

        return Result.Ok();
    }
}