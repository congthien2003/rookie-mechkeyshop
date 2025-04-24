using Application.Interfaces.IUnitOfWork;
using Domain.Entity;
using Domain.IRepositories;
using MechkeyShop.Data;

namespace Infrastructure.UnitOfWork
{
    public class ProductUnitOfWork : IProductUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IProductImageRepository ProductImageRepository { get; }

        public IProductRepository<Product> ProductRepository { get; }

        public ProductUnitOfWork(ApplicationDbContext context,
    IProductImageRepository productImageRepository,
    IProductRepository<Product> productRepository)
        {
            _context = context;
            ProductImageRepository = productImageRepository;
            ProductRepository = productRepository;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
