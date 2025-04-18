using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using MCC.TestTask.App.Features.Posts;
using MCC.TestTask.App.Features.Posts.Dto;
using MCC.TestTask.App.Services.Mail;
using MCC.TestTask.App.Utils.Pagination;
using MCC.TestTask.Domain;
using MCC.TestTask.Persistance;
using Microsoft.EntityFrameworkCore;
using Moq;
using NeinLinq;

namespace MCC.TestTask.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class PostServiceBenchmark
{
    private BlogDbContext _context;
    private MailJobService _mailJobService;
    private PostService _postService;
    private readonly string _connectionString;

    public PostServiceBenchmark()
    {
        _connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres;";
    }

    [GlobalSetup]
    public void GlobalSetup()
    {
        var options = new DbContextOptionsBuilder<BlogDbContext>()
            .UseNpgsql(_connectionString)
            .WithLambdaInjection().Options;
        
        var mockJobClient = new Mock<IBackgroundJobClient>();
        mockJobClient.Setup(x => x.Create(It.IsAny<Job>(), It.IsAny<EnqueuedState>()))
            .Returns("job-id");

        _context = new BlogDbContext((options as DbContextOptions<BlogDbContext>)!);
        _mailJobService = new MailJobService(mockJobClient.Object, _context);
        _postService = new PostService(_context, _mailJobService);
        
        _context.Database.EnsureCreated();
    }

    [Benchmark]
    public async Task GetAllUsersAsync_100_Filtered()
    {
        await _postService.GetAllPostsAsync(null, new PostListFilter
        {
            MaxReadingTime = 8,
            TagIds = [],
        }, null, new PaginationModel
        {
            Page = 1,
            Size = 100,
        });
    }
    
    [Benchmark]
    public async Task GetAllUsersAsync_100_DoubleFiltered()
    {
        await _postService.GetAllPostsAsync(null, new PostListFilter
        {
            MaxReadingTime = 8,
            MinReadingTime = 4,
            TagIds = [],
        }, null, new PaginationModel
        {
            Page = 1,
            Size = 100,
        });
    }
    
    [Benchmark]
    public async Task GetAllUsersAsync_100_NoFilters()
    {
        await _postService.GetAllPostsAsync(null, new PostListFilter
        {
            TagIds = [],
        }, null, new PaginationModel
        {
            Page = 1,
            Size = 100,
        });
    }
    
    [Benchmark]
    public async Task GetAllUsersAsync_500_Filtered()
    {
        await _postService.GetAllPostsAsync(null, new PostListFilter
        {
            MaxReadingTime = 8,
            TagIds = [],
        }, null, new PaginationModel
        {
            Page = 1,
            Size = 500,
        });
    }
    
    [Benchmark]
    public async Task GetAllUsersAsync_500_DoubleFiltered()
    {
        await _postService.GetAllPostsAsync(null, new PostListFilter
        {
            MaxReadingTime = 8,
            MinReadingTime = 4,
            TagIds = [],
        }, null, new PaginationModel
        {
            Page = 1,
            Size = 500,
        });
    }
    
    [Benchmark]
    public async Task GetAllUsersAsync_500_NoFilters()
    {
        await _postService.GetAllPostsAsync(null, new PostListFilter
        {
            TagIds = [],
        }, null, new PaginationModel
        {
            Page = 1,
            Size = 500,
        });
    }
    
    [Benchmark]
    public async Task GetAllUsersAsync_1000_Filtered()
    {
        await _postService.GetAllPostsAsync(null, new PostListFilter
        {
            MaxReadingTime = 8,
            TagIds = [],
        }, null, new PaginationModel
        {
            Page = 1,
            Size = 1000,
        });
    }
    
    [Benchmark]
    public async Task GetAllUsersAsync_1000_DoubleFiltered()
    {
        await _postService.GetAllPostsAsync(null, new PostListFilter
        {
            MaxReadingTime = 8,
            MinReadingTime = 4,
            TagIds = [],
        }, null, new PaginationModel
        {
            Page = 1,
            Size = 1000,
        });
    }
    
    [Benchmark]
    public async Task GetAllUsersAsync_1000_NoFilters()
    {
        await _postService.GetAllPostsAsync(null, new PostListFilter
        {
            TagIds = [],
        }, null, new PaginationModel
        {
            Page = 1,
            Size = 1000,
        });
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        _context.Dispose();
    }
}