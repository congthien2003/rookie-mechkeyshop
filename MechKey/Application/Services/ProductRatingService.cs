﻿using Application.Comoon;
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
            try
            {

                var entity = mapper.Map<ProductRating>(model);

                var product = await productRepository.GetByIdAsync(entity.ProductId);
                product.AddRating(entity);

                await productRepository.UpdateAsync(product);
                //var result = await productRatingRepository.CreateAsync(entity);
                return Result.Success("Add rating success");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw new Exception("Add rating failed");
            }
        }

        public async Task<Result<PagedResult<ProductRatingModel>>>? GetAllByIdProductAsync(Guid id, int pageSize = 4, bool ascOrder = false)
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
                    .Select(pr => mapper.Map<ProductRatingModel>(pr))
                    .ToListAsync();

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
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return Result<PagedResult<ProductRatingModel>>.Failure("Get failed", null);
            }
        }
    }
}
