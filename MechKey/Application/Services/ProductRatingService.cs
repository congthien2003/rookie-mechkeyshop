using Application.Comoon;
using Application.Interfaces.IServices;
using AutoMapper;
using Domain.Entity;
using Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Common;
using Shared.ViewModels;

namespace Application.Services
{
    public class ProductRatingService : IProductRatingService
    {
        private readonly IProductRatingRepository<ProductRating> productRatingRepository;
        private readonly IApplicationUserRepository<ApplicationUser> applicationUserRepository;
        private readonly ILogger<ProductRatingService> logger;
        private readonly IMapper mapper;
        public ProductRatingService(IProductRatingRepository<ProductRating> productRatingRepository,
            ILogger<ProductRatingService> logger,
            IMapper mapper,
            IApplicationUserRepository<ApplicationUser> applicationUserRepository)
        {
            this.productRatingRepository = productRatingRepository;
            this.logger = logger;
            this.mapper = mapper;
            this.applicationUserRepository = applicationUserRepository;
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

        public async Task<Result<PagedResult<ProductRatingViewModel>>>? GetAllByIdProductAsync(Guid id, int pageSize = 4, bool ascOrder = false)
        {
            try
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

                var data = await query.Take(pageSize)
                    .Include(pr => pr.User)
                    .Select(pr => mapper.Map<ProductRatingViewModel>(pr))
                    .ToListAsync();

                PagedResult<ProductRatingViewModel> result = new PagedResult<ProductRatingViewModel>()
                {
                    Page = 1,
                    PageSize = pageSize,
                    TotalItems = query.Count(),
                    Items = data,
                    TotalPages = (int)Math.Ceiling(query.Count() / (double)pageSize)
                };

                return Result<PagedResult<ProductRatingViewModel>>.Success("Get rating success", result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return Result<PagedResult<ProductRatingViewModel>>.Failure("Get failed", null);
            }
        }
    }
}
