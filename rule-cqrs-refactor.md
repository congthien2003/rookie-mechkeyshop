# Rules: Refactor Catalog sang MediatR Command/Query

## 1. Cấu trúc thư mục (Slice Feature)

Tạo trong `MechKey.Application/`:

```
MechKey.Application/
├── Catalog/
│   ├── Commands/
│   │   ├── CreateCategory.cs          // request + validator + handler
│   │   ├── UpdateCategory.cs
│   │   ├── DeleteCategory.cs
│   │   ├── CreateProduct.cs
│   │   ├── UpdateProduct.cs
│   │   ├── DeleteProduct.cs
│   │   └── CreateProductRating.cs
│   ├── Queries/
│   │   ├── GetCategoryList.cs         // request + handler
│   │   ├── GetCategoryById.cs
│   │   ├── GetProductList.cs
│   │   ├── GetProductById.cs
│   │   ├── GetBestSellerProducts.cs
│   │   ├── GetProductRatings.cs
│   │   └── GetDashboard.cs
│   └── Models/
│       └── (response models nếu cần tách riêng)
```

## 2. Quy tắc đặt tên

| Loại | Pattern | Ví dụ |
|------|---------|-------|
| Command request | `{Action}{Entity}Command` | `CreateCategoryCommand`, `DeleteProductCommand` |
| Command handler | `{Action}{Entity}Handler` | `CreateCategoryHandler` |
| Command validator | `{Action}{Entity}Validator` | `CreateCategoryValidator` |
| Query request | `Get{Entity}{Filter}Query` | `GetCategoryListQuery`, `GetProductByIdQuery` |
| Query handler | `Get{Entity}{Filter}Handler` | `GetCategoryListHandler` |
| File name | Trùng với tên command/query chính | `CreateCategory.cs` chứa cả 3 class |

## 3. Pattern cho Command (1 file = request + validator + handler)

```csharp
// File: Catalog/Commands/CreateCategory.cs
namespace Application.Catalog.Commands;

// === Request ===
public class CreateCategoryCommand : IRequest<Result<CategoryModel>>
{
    public string Name { get; init; } = default!;
}

// === Validator ===
// Dùng static validator class, KHÔNG dùng FluentValidation
public class CreateCategoryValidator
{
    public static void Validate(CreateCategoryCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new CategoryInvalidDataException("Category name is required");
    }
}

// === Handler ===
public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryModel>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IRedisService _redisService;
    // KHÔNG inject repository, dùng DbContext trực tiếp

    public CreateCategoryHandler(ApplicationDbContext dbContext, IRedisService redisService)
    {
        _dbContext = dbContext;
        _redisService = redisService;
    }

    public async Task<Result<CategoryModel>> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        CreateCategoryValidator.Validate(request);

        var entity = new Category { Id = Guid.NewGuid(), Name = request.Name };
        _dbContext.Categories.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        await _redisService.RemoveByPrefixAsync("category", ct);

        return Result<CategoryModel>.Success("Add category success", CategoryMapping.ToCategoryModel(entity));
    }
}
```

## 4. Pattern cho Query (1 file = request + handler, không validator)

```csharp
// File: Catalog/Queries/GetCategoryList.cs
namespace Application.Catalog.Queries;

public class GetCategoryListQuery : IRequest<Result<PagedResult<CategoryModel>>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string SearchTerm { get; init; } = "";
}

public class GetCategoryListHandler : IRequestHandler<GetCategoryListQuery, Result<PagedResult<CategoryModel>>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IRedisService _redisService;

    public GetCategoryListHandler(ApplicationDbContext dbContext, IRedisService redisService)
    {
        _dbContext = dbContext;
        _redisService = redisService;
    }

    public async Task<Result<PagedResult<CategoryModel>>> Handle(GetCategoryListQuery request, CancellationToken ct)
    {
        // Copy logic từ CategoryService.GetAllAsync
        // Thay _repository.GetAllAsync() -> _dbContext.Categories.AsQueryable()
        // ...
    }
}
```

## 5. Xử lý đặc biệt cho Product (transaction + EventBus + upload ảnh)

- **CreateProduct**: Handler inject `IProductUnitOfWork` (transaction), `ISupabaseService` (upload ảnh), `IEventBus` (publish rollback event), `IRedisService` (clear cache)
- **UpdateProduct**: Tương tự, upload ảnh conditionally + publish `DeleteImageEvent`
- Copy logic y hệt từ `ProductService`, bao gồm try-catch rollback + publish event

```csharp
public class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<ProductModel>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IProductUnitOfWork _unitOfWork;
    private readonly ISupabaseService _supabaseService;
    private readonly IEventBus _eventBus;
    private readonly IRedisService _redisService;

    public CreateProductHandler(
        ApplicationDbContext dbContext,
        IProductUnitOfWork unitOfWork,
        ISupabaseService supabaseService,
        IEventBus eventBus,
        IRedisService redisService)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _supabaseService = supabaseService;
        _eventBus = eventBus;
        _redisService = redisService;
    }

    public async Task<Result<ProductModel>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        // Copy toàn bộ logic từ ProductService.AddAsync
    }
}
```

## 6. Danh sách Command/Query cần tạo

| # | File | Loại | Ánh xạ từ Service Method | Handler Inject |
|---|------|------|--------------------------|----------------|
| 1 | `CreateCategory.cs` | Command | `CategoryService.AddAsync` | DbContext, IRedisService |
| 2 | `UpdateCategory.cs` | Command | `CategoryService.UpdateAsync` | DbContext, IRedisService |
| 3 | `DeleteCategory.cs` | Command | `CategoryService.DeleteAsync` | DbContext, IRedisService |
| 4 | `GetCategoryList.cs` | Query | `CategoryService.GetAllAsync` | DbContext, IRedisService |
| 5 | `GetCategoryById.cs` | Query | `CategoryService.GetByIdAsync` | DbContext |
| 6 | `CreateProduct.cs` | Command | `ProductService.AddAsync` | DbContext, IProductUnitOfWork, ISupabaseService, IEventBus, IRedisService |
| 7 | `UpdateProduct.cs` | Command | `ProductService.UpdateAsync` | DbContext, IRedisService, ISupabaseService, IEventBus |
| 8 | `DeleteProduct.cs` | Command | `ProductService.DeleteAsync` | DbContext, IRedisService |
| 9 | `GetProductList.cs` | Query | `ProductService.GetAllAsync` | DbContext, IRedisService |
| 10 | `GetProductById.cs` | Query | `ProductService.GetByIdAsync` | DbContext |
| 11 | `GetBestSellerProducts.cs` | Query | `ProductService.GetBestSellerAsync` | DbContext |
| 12 | `CreateProductRating.cs` | Command | `ProductRatingService.AddAsync` | DbContext |
| 13 | `GetProductRatings.cs` | Query | `ProductRatingService.GetAllByIdProductAsync` | DbContext |
| 14 | `GetDashboard.cs` | Query | `DashboardService.GetDashboardDataAsync` | DbContext |

## 7. Không đụng vào code cũ

- Giữ nguyên tất cả service interfaces + implementations
- Giữ nguyên các validator static cũ (`CategoryValidator`, `ProductValidator`)
- Giữ nguyên mapping classes (`CategoryMapping`, `ProductMapping`, `ProductRatingMapping`)
- Chỉ tạo mới command/query files, copy logic từ service cũ vào handler
- Sau khi test ổn định, optionally xóa service cũ (không bắt buộc trong phase này)

## 8. Refactor Controller

Controller chỉ inject `ISender` (từ MediatR), không inject service interface:

```csharp
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/category")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ISender _sender;

    public CategoryController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("list")]
    public async Task<Result<PagedResult<CategoryModel>>> GetList(
        int page = 1, int pageSize = 10, string searchTerm = "", CancellationToken ct = default)
    {
        return await _sender.Send(new GetCategoryListQuery
        {
            Page = page, PageSize = pageSize, SearchTerm = searchTerm
        }, ct);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryModel model, CancellationToken ct = default)
    {
        var result = await _sender.Send(new CreateCategoryCommand { Name = model.Name }, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetCategoryByIdQuery { Id = id }, ct);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateById(Guid id, CategoryModel model, CancellationToken ct = default)
    {
        if (id != model.Id) return BadRequest("ID mismatch");
        var result = await _sender.Send(new UpdateCategoryCommand
        {
            Id = model.Id, Name = model.Name
        }, ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var result = await _sender.Send(new DeleteCategoryCommand { Id = id }, ct);
        return Ok(result);
    }
}
```

**Quy tắc controller:**
- Thay service interface → `ISender`
- Mỗi endpoint 1 dòng gọi: `await _sender.Send(new XxxCommand/Query { ... }, ct)`
- Giữ nguyên: route, HTTP method, response wrapper, api version
- Map model đầu vào của controller → property của command/query

## 9. Cấu hình MediatR

### 9.1 Thêm package

`MechKey.Application.csproj`:
```xml
<PackageReference Include="MediatR" Version="12.5.0" />
```

### 9.2 Đăng ký MediatR

`MechKey.Application\DependencyInjection.cs`:
```csharp
using MediatR;

public static IServiceCollection AddApplication(this IServiceCollection service)
{
    // Giữ nguyên mapping + service registration cũ

    service.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

    return service;
}
```

Không cần thêm package vào `WebAPI.csproj` vì WebAPI đã reference Application.

## 10. Thứ tự triển khai

| Bước | Mô tả | Verify |
|------|-------|--------|
| 1 | Thêm MediatR package vào `MechKey.Application.csproj` | `dotnet restore` |
| 2 | Đăng ký MediatR trong `DependencyInjection.cs` | Build OK |
| 3 | Tạo folder structure: `Catalog/Commands/`, `Catalog/Queries/` | - |
| 4 | Tạo từng command/query file, copy logic từ service → handler | Build OK |
| 5 | Refactor controller: thay service injection → `ISender` | Build OK |
| 6 | Build toàn bộ solution, fix compile errors | `dotnet build` |
| 7 | Test từng endpoint, verify cache, transaction, event bus | Manual/Integration test |
| 8 | (Optional) Xóa service registration + service files cũ | - |

## 11. Tổng kết các nguyên tắc

1. **Slice feature**: Mỗi file = 1 use case hoàn chỉnh (request + validator + handler)
2. **Không xóa code cũ**: Service, validator, mapping giữ nguyên
3. **DbContext trực tiếp**: Handler dùng `ApplicationDbContext` thay vì repository
4. **Static validator**: Giữ pattern validator static class, không dùng FluentValidation
5. **Result pattern**: Giữ nguyên `Result<T>` làm response type cho command/query
6. **Copy logic chính xác**: Handler copy logic từ service, bao gồm caching, transaction, event publishing
7. **ISender trong controller**: Controller chỉ inject `ISender`, mỗi endpoint 1 dòng `_sender.Send()`
