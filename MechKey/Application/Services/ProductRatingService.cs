using Application.Comoon;
using Application.Interfaces.IServices;
using AutoMapper;
using Domain.Entity;
using Domain.IRepositories;
using Microsoft.Extensions.Logging;
using Shared.ViewModels;

namespace Application.Services
{
    public class ProductRatingService : IProductRatingService
    {
        private readonly IProductRatingRepository<ProductRating> productRatingRepository;
        private readonly ILogger<ProductRatingService> logger;
        private readonly IMapper mapper;
        public ProductRatingService(IProductRatingRepository<ProductRating> productRatingRepository,
            ILogger<ProductRatingService> logger,
            IMapper mapper)
        {
            this.productRatingRepository = productRatingRepository;
            this.logger = logger;
            this.mapper = mapper;
        }
        public async Task<Result> AddAsync(ProductRatingViewModel model)
        {
            try
            {
                var entity = mapper.Map<ProductRating>(model);
                var result = await productRatingRepository.CreateAsync(entity);
                return Result.Success("Add rating success");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new Exception("Add rating failed");
            }
        }
    }
}
