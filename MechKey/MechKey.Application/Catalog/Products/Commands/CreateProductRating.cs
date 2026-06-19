using Application.Comoon;
using Application.Interfaces;
using Domain.Entity;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.ViewModels.Product;

namespace Application.Catalog.Products.Commands;

public class CreateProductRatingCommand : IRequest<Result>
{
    public int Stars { get; init; }
    public string Comment { get; init; } = "";
    public Guid ProductId { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; } = "";
}

public class CreateProductRatingValidator
{
    public static void Validate(CreateProductRatingCommand command)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        if (command.Stars < 1 || command.Stars > 5)
            throw new ProductRatingInvalidDataException("Stars must be between 1 and 5");

        if (string.IsNullOrWhiteSpace(command.Comment))
            throw new ProductRatingInvalidDataException("Comment cannot be empty");

        if (Guid.Equals(command.ProductId, Guid.Empty))
            throw new ProductRatingInvalidDataException("ProductId is required");

        if (Guid.Equals(command.UserId, Guid.Empty))
            throw new ProductRatingInvalidDataException("UserId is required");

        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ProductRatingInvalidDataException("User name is required");
    }
}

public class CreateProductRatingHandler : IRequestHandler<CreateProductRatingCommand, Result>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<CreateProductRatingHandler> _logger;

    public CreateProductRatingHandler(
        IApplicationDbContext dbContext,
        ILogger<CreateProductRatingHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result> Handle(CreateProductRatingCommand request, CancellationToken ct)
    {
        CreateProductRatingValidator.Validate(request);

        var product = await _dbContext.Products
            .Include(p => p.ProductRatings)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, ct);

        if (product is null)
            throw new ProductNotFoundException();

        var rating = new ProductRating
        {
            Stars = request.Stars,
            Comment = request.Comment,
            RatedAt = DateTime.UtcNow,
            ProductId = request.ProductId,
            UserId = request.UserId
        };

        product.AddRating(rating);

        _dbContext.ProductRatings.Add(rating);
        await _dbContext.SaveChangesAsync(ct);

        return Result.Success("Add rating success");
    }
}
