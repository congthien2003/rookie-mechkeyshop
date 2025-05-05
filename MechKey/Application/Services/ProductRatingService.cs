using Application.Comoon;
using Application.Interfaces.IServices;
using AutoMapper;
using Domain.Entity;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Common;
using Shared.ViewModels.Product;

namespace Application.Services
{
    public class ProductRatingService : IProductRatingService
    {
        private readonly IProductRepository<Product> productRepository;
        private readonly IProductRatingRepository<ProductRating> productRatingRepository;
        private readonly IApplicationUserRepository<ApplicationUser> applicationUserRepository;
        private readonly ILogger<ProductRatingService> logger;
        private readonly IMapper mapper;
        public ProductRatingService(IProductRatingRepository<ProductRating> productRatingRepository,
            ILogger<ProductRatingService> logger,
            IMapper mapper,
            IApplicationUserRepository<ApplicationUser> applicationUserRepository,
            IProductRepository<Product> productRepository)
        {
            this.productRatingRepository = productRatingRepository;
            this.logger = logger;
            this.mapper = mapper;
            this.applicationUserRepository = applicationUserRepository;
            this.productRepository = productRepository;
        }
        public async Task<Result> AddAsync(ProductRatingModel model)
        {
            ProductRating entity = mapper.Map<ProductRating>(model);

            Product product = await productRepository.GetByIdAsync(entity.ProductId);
            product.AddRating(entity);

            await productRepository.UpdateAsync(product);
            //var result = await productRatingRepository.CreateAsync(entity);
            return Result.Success("Add rating success");
        }

        public async Task<Result<PagedResult<ProductRatingModel>>>? GetAllByIdProductAsync(Guid id, int pageSize = 4, bool ascOrder = false)
        {

            var query = productRatingRepository.GetListByProdut(id);

            if (ascOrder)
            {
                query = query.OrderBy(pr => pr.RatedAt);

            }
            else
            {
                query = query.OrderByDescending(pr => pr.Id);
            }

            var userInfo = await applicationUserRepository.GetByIdAsync(id);

            var data = query.Take(pageSize)
                .Include(pr => pr.User)
                .Select(pr => mapper.Map<ProductRatingModel>(pr))
                .ToList();

            PagedResult<ProductRatingModel> result = new PagedResult<ProductRatingModel>()
            {
                Page = 1,
                PageSize = pageSize,
                TotalItems = query.Count(),
                Items = data,
                TotalPages = (int)Math.Ceiling(query.Count() / (double)pageSize)
            };

            return Result<PagedResult<ProductRatingModel>>.Success("Get rating success", result);

        }
    }
}
