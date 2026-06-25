using Application.Comoon;
using Application.Interfaces.IApiClient.Redis;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Catalog.Products.Commands;

public class DeleteProductCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IProductRepository<Product> _repository;
    private readonly IRedisService _redisService;
    private readonly ILogger<DeleteProductHandler> _logger;

    public DeleteProductHandler(
        IProductRepository<Product> repository,
        IRedisService redisService,
        ILogger<DeleteProductHandler> logger)
    {
        _repository = repository;
        _redisService = redisService;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(request.Id, ct);
        if (entity == null)
        {
            _logger.LogWarning("Product not found in {Method}. ProductId: {ProductId}", nameof(Handle), request.Id);
            throw new ProductNotFoundException();
        }

        await _repository.DeleteAsync(entity, ct);
        await _redisService.RemoveByPrefixAsync("product", ct);

        return Result.Success("Delete product success");
    }
}
