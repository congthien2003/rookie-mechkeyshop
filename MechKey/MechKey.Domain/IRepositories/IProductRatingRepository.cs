namespace Domain.IRepositories
{
    public interface IProductRatingRepository<ProductRating>
    {
        public Task<ProductRating> GetByIdAsync(int id, CancellationToken token);
        public Task<ProductRating> CreateAsync(ProductRating productRating, CancellationToken token);
        public Task<ProductRating> UpdateAsync(ProductRating productRating, CancellationToken token);
        public Task DeleteAsync(ProductRating productRating, CancellationToken token);
        public IQueryable<ProductRating> GetListByProduct(Guid id);
    }
}
