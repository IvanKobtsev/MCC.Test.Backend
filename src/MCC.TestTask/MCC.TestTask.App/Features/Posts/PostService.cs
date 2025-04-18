﻿using MCC.TestTask.App.Features.Posts.Dto;
using MCC.TestTask.App.Services.Mail;
using MCC.TestTask.App.Utils.Pagination;
using MCC.TestTask.Domain;
using MCC.TestTask.Domain.Extensions;
using MCC.TestTask.Persistance;
using MCC.TestTask.Infrastructure;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace MCC.TestTask.App.Features.Posts;

// [x] - get all posts
// [x] - create a post
// [x] - get a post
// [x] - like a post
// [x] - dislike a post
public class PostService
{
    private readonly BlogDbContext _blogDbContext;
    private readonly MailJobService _mailJobService;

    public PostService(BlogDbContext blogDbContext, MailJobService mailJobService)
    {
        _blogDbContext = blogDbContext;
        _mailJobService = mailJobService;
    }

    public async Task<Result<PostPagedListDto>> GetAllPostsAsync(Guid? userId,
        PostListFilter filter,
        PostSorting? sorting,
        PaginationModel pagination)
    {
        var userValidationResult = userId.HasValue
            ? await CheckUserExistsAsync(userId.Value)
            : Result.Ok();

        if (!userValidationResult.IsSuccess)
            return userValidationResult;

        var communityValidationResult = filter.CommunityId.HasValue
            ? await CheckCommunityExistsAsync(filter.CommunityId.Value)
            : Result.Ok();

        if (!communityValidationResult.IsSuccess)
            return communityValidationResult;

        var query = _blogDbContext.Posts.ReadableByUser(userId);

        if (!string.IsNullOrEmpty(filter.Author))
            query = query.Where(x => x.Author.FullName.Contains(filter.Author));

        if (filter.TagIds.Count > 0)
        {
            var existingTagsCount = await _blogDbContext.Tags.CountAsync(t => filter.TagIds.Contains(t.Id));

            if (existingTagsCount != filter.TagIds.Distinct().Count())
                return Result.Fail(new ValidationError("Invalid tag id"));

            query = query.Where(p => p.Tags.Any(t => filter.TagIds.Contains(t.Id)));
        }

        if (filter.MinReadingTime.HasValue)
            query = query.Where(x => x.ReadingTime >= filter.MinReadingTime.Value);

        if (filter.MaxReadingTime.HasValue)
            query = query.Where(x => x.ReadingTime <= filter.MaxReadingTime.Value);

        if (filter.OnlyMyCommunities.HasValue)
            query = query.Where(p =>
                p.Community != null && (
                    p.Community.Creator.Id == userId
                    || p.Community.Administrators.Any(a => a.Id == userId)
                    || p.Community.Subscribers.Any(u => u.Id == userId)));

        if (filter.CommunityId.HasValue)
            query = query.Where(x => x.CommunityId == filter.CommunityId.Value);

        var totalCount = await query.CountAsync();

        query = sorting switch
        {
            PostSorting.CreateAsc => query.OrderBy(p => p.CreatedAt),
            PostSorting.CreateDesc => query.OrderByDescending(p => p.CreatedAt),
            PostSorting.LikeAsc => query.OrderBy(p => p.LikedBy.Count),
            PostSorting.LikeDesc => query.OrderByDescending(p => p.LikedBy.Count),
            null => query,
            _ => throw new ArgumentOutOfRangeException(nameof(sorting), sorting, null)
        };

        return new PostPagedListDto
        {
            Posts = await query.Select(p=> p.ToDto(userId))
                .Paginate(pagination)
                .ToListAsync(),
            Pagination = pagination.ToDto(totalCount)
        };
    }

    public async Task<Result<PostFullDto>> GetPostAsync(Guid postId, Guid? userId)
    {
        return await (userId.HasValue ? await CheckUserExistsAsync(userId.Value) : Result.Ok<Guid?>(null)).Bind(
            async Task<Result<PostFullDto>> (_) =>
            {
                var query = userId.HasValue
                    ? _blogDbContext.Posts
                        .ReadableByUser(userId)
                    : _blogDbContext.Posts;

                var postFullDto = await query
                    .ReadableByUser(userId)
                    .Select(p => p.ToFullDto(userId))
                    .FirstOrDefaultAsync(p => p.Id == postId);

                return postFullDto != null
                    ? postFullDto
                    : CustomErrors.NotFound("Post not found");
            });
    }

    public async Task<Result<Guid>> CreatePostAsync(Guid authorId, Guid? communityId, CreatePostModel model)
    {
        if (communityId.HasValue)
        {
            var community = await _blogDbContext.Communities.Include(c => c.Administrators)
                .FirstOrDefaultAsync(c => c.Id == communityId);

            if (community == null)
                return CustomErrors.NotFound("Community not found");

            if (!(community.CreatorId == authorId || community.Administrators.Any(a => a.Id == authorId)))
                return CustomErrors.NotFound("User is not able to post in the community");
        }

        var author = await _blogDbContext.Users.FirstOrDefaultAsync(a => a.Id == authorId);

        if (author is null)
            return CustomErrors.NotFound("User not found");

        var post = new Post
        {
            CommunityId = communityId,
            AuthorId = authorId,
            CreatedAt = DateTime.UtcNow,
            Title = model.Title,
            Description = model.Description,
            ReadingTime = model.ReadingTime,
            ImageUrl = model.Image,
            AddressObjectId = model.AddressId
        };

        _blogDbContext.Posts.Add(post);

        if (model.Tags.Any())
        {
            var existingTags = await _blogDbContext.Tags.Where(t => model.Tags.Contains(t.Id)).ToListAsync();

            if (existingTags.Count != model.Tags.Distinct().Count())
                return Result.Fail(new ValidationError("Invalid tag id"));

            _blogDbContext.Entry(post).Collection(p => p.Tags).CurrentValue = existingTags;
        }

        await _blogDbContext.SaveChangesAsync();

        if (communityId == null) return post.Id;
        
        var result = await _mailJobService.NotifySubscribersAboutNewPost(post.Id);
        
        if (result.IsFailed) return result;
        return post.Id;
    }

    public async Task<Result> LikePostAsync(Guid postId, Guid userId)
    {
        var user = _blogDbContext.Users.FirstOrDefault(u => u.Id == userId);
        if (user is null)
            return CustomErrors.NotFound("User not found");

        var post = _blogDbContext.Posts.Include(p => p.LikedBy)
            .ReadableByUser(userId)
            .FirstOrDefault(p => p.Id == postId);
        if (post is null)
            return CustomErrors.NotFound("Post not found");

        if (post.LikedBy.Contains(user))
            return Result.Fail(new ValidationError("User already liked this post."));

        post.LikedBy.Add(user);
        await _blogDbContext.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result> DislikePostAsync(Guid postId, Guid userId)
    {
        var user = _blogDbContext.Users.FirstOrDefault(u => u.Id == userId);
        if (user is null)
            return CustomErrors.NotFound("User not found");

        var post = _blogDbContext.Posts
            .Include(p => p.LikedBy)
            .ReadableByUser(userId)
            .FirstOrDefault(p => p.Id == postId);
        if (post is null)
            return CustomErrors.NotFound("Post not found");

        if (!post.LikedBy.Contains(user))
            return Result.Fail(new ValidationError("User did not like this post."));

        post.LikedBy.Remove(user);
        await _blogDbContext.SaveChangesAsync();

        return Result.Ok();
    }

    private async Task<Result> CheckCommunityExistsAsync(Guid communityId)
    {
        return Result.OkIf(
            await _blogDbContext.Communities.AnyAsync(c => c.Id == communityId),
            new NotFoundError("Community not found"));
    }

    private async Task<Result> CheckUserExistsAsync(Guid userId)
    {
        return Result.OkIf(
            await _blogDbContext.Users.AnyAsync(u => u.Id == userId),
            new NotFoundError("Non-existent user"));
    }
}