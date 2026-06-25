using Application.Comoon;
using Application.Interfaces;
using Application.Interfaces.IApiClient.Redis;
using Domain.Entity;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Catalog.Categories.Commands;

public class DeleteCategoryCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRedisService _redisService;
    private readonly ILogger<DeleteCategoryHandler> _logger;

    public DeleteCategoryHandler(
        IApplicationDbContext dbContext,
        IRedisService redisService,
        ILogger<DeleteCategoryHandler> logger)
    {
        _dbContext = dbContext;
        _redisService = redisService;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        var entity = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (entity == null)
        {
            _logger.LogWarning("Category not found in {Method}. CategoryId: {CategoryId}", nameof(Handle), request.Id);
            throw new CategoryNotFoundException();
        }

        _dbContext.Categories.Remove(entity);
        await _dbContext.SaveChangesAsync(ct);
        await _redisService.RemoveByPrefixAsync("category", ct);

        return Result.Success("Delete category success");
    }
}
