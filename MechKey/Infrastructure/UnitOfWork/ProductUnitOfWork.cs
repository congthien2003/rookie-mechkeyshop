using Application.Interfaces.IUnitOfWork;
using Domain.Entity;
using Domain.IRepositories;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.UnitOfWork
{
    public class ProductUnitOfWork : IProductUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;

        public IProductImageRepository ProductImageRepository { get; }

        public IProductRepository<Product> ProductRepository { get; }

        public ProductUnitOfWork(ApplicationDbContext context,
    IProductImageRepository productImageRepository,
    IProductRepository<Product> productRepository
    )
        {
            _context = context;
            ProductImageRepository = productImageRepository;
            ProductRepository = productRepository;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _transaction.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
        }
    }
}
