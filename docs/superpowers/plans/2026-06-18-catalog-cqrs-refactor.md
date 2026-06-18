# Catalog CQRS Refactor — MediatR Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Refactor Catalog API controllers from direct service injection to MediatR command/query pattern without removing existing service code.

**Architecture:** Add MediatR 12.5.0 to MechKey.Application, create `Catalog/Commands/` and `Catalog/Queries/` folders under Application. Each file = 1 use case (request + validator + handler). Handlers use `ApplicationDbContext` directly (skip repository abstraction). Controllers swap service injection for `ISender`.

**Tech Stack:** .NET 10, MediatR 12.5.0, EF Core 10, Redis (caching), MassTransit (EventBus), Supabase (image upload)

**Spec Reference:** `rule-cqrs-refactor.md` at repo root

---

## File Map

### Created files (all under `MechKey\MechKey.Application\Catalog\`)

| File | Purpose |
|------|---------|
| `Commands\CreateCategory.cs` | Request + validator + handler for create category |
| `Commands\UpdateCategory.cs` | Request + validator + handler for update category |
| `Commands\DeleteCategory.cs` | Request + handler for delete category |
| `Commands\CreateProduct.cs` | Request + validator + handler for create product (txn + upload) |
| `Commands\UpdateProduct.cs` | Request + validator + handler for update product (txn + upload) |
| `Commands\DeleteProduct.cs` | Request + handler for delete product |
| `Commands\CreateProductRating.cs` | Request + validator + handler for create rating |
| `Queries\GetCategoryList.cs` | Request + handler for paginated category list |
| `Queries\GetCategoryById.cs` | Request + handler for single category |
| `Queries\GetProductList.cs` | Request + handler for paginated filtered product list |
| `Queries\GetProductById.cs` | Request + handler for single product |
| `Queries\GetBestSellerProducts.cs` | Request + handler for top 4 best sellers |
| `Queries\GetProductRatings.cs` | Request + handler for product rating list |
| `Queries\GetDashboard.cs` | Request + handler for dashboard aggregate data |

### Modified files

| File | Change |
|------|--------|
| `MechKey.Application\MechKey.Application.csproj` | Add MediatR 12.5.0 package |
| `MechKey.Application\DependencyInjection.cs` | Register MediatR |
| `MechKey.WebAPI\Controllers\CategoryController.cs` | Swap service for ISender |
| `MechKey.WebAPI\Controllers\ProductController.cs` | Swap service for ISender |
| `MechKey.WebAPI\Controllers\DashboardController.cs` | Swap service for ISender |

---

## Phase 1: Infrastructure Setup

### Task 1.1: Add MediatR package

**Files:**
- Modify: `MechKey\MechKey.Application\MechKey.Application.csproj`

- [ ] **Step 1: Add MediatR package reference**

Open `MechKey\MechKey.Application\MechKey.Application.csproj`. After the existing `Newtonsoft.Json` PackageReference, add:

```xml
<PackageReference Include="MediatR" Version="12.5.0" />
```

Final `ItemGroup` should look like:

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="10.0.0" />
  <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="10.0.0" />
  <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
  <PackageReference Include="MediatR" Version="12.5.0" />
</ItemGroup>
```

- [ ] **Step 2: Restore packages**

```powershell
dotnet restore "MechKey\MechKey.Application\MechKey.Application.csproj"
```

Expected: "Restore succeeded" or no errors.

### Task 1.2: Register MediatR in DI

**Files:**
- Modify: `MechKey\MechKey.Application\DependencyInjection.cs`

- [ ] **Step 1: Add MediatR registration**

Open `MechKey\MechKey.Application\DependencyInjection.cs`. Add `using MediatR;` and call `AddMediatR`. The method should become:

```csharp
using Application.Interfaces.IServices;
using Application.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shared.Mapping.Implementations;
using Shared.Mapping.Interfaces;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection service)
        {
            // Register mapping services
            service.AddScoped<IApplicationUserMapping, ApplicationUserMapping>();
            service.AddScoped<IProductMapping, ProductMapping>();
            service.AddScoped<IProductRatingMapping, ProductRatingMapping>();
            service.AddScoped<IOrderItemMapping, OrderItemMapping>();
            service.AddScoped<IOrderMapping, OrderMapping>();
            service.AddScoped<ICategoryMapping, CategoryMapping>();

            // Register application services
            service.AddScoped<IApplicaionUserService, ApplicationUserService>();
            service.AddScoped<IAuthenticationService, AuthenticationService>();
            service.AddScoped<IProductService, ProductService>();
            service.AddScoped<IProductSalesTracker, ProductSalesTracker>();
            service.AddScoped<IProductRatingService, ProductRatingService>();
            service.AddScoped<ICategoryService, CategoryService>();
            service.AddScoped<IJwtManager, JwtManager>();
            service.AddScoped<IOrderService, OrderService>();
            service.AddScoped<IDashboardService, DashboardService>();

            // Register MediatR from this assembly
            service.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

            return service;
        }
    }
}
```

- [ ] **Step 2: Create Catalog folder structure**

```powershell
New-Item -ItemType Directory -Path "MechKey\MechKey.Application\Catalog\Commands" -Force
New-Item -ItemType Directory -Path "MechKey\MechKey.Application\Catalog\Queries" -Force
```

- [ ] **Step 3: Build to verify MediatR resolves**

```powershell
dotnet build "MechKey\MechKey.WebAPI\WebAPI.csproj"
```

Expected: Build succeeded. 0 errors. If "type or namespace 'MediatR' could not be found", ensure package restore ran.

- [ ] **Step 4: Commit**

```bash
git add MechKey/MechKey.Application/MechKey.Application.csproj MechKey/MechKey.Application/DependencyInjection.cs
git commit -m "feat: add MediatR 12.5.0 package and DI registration"
```

---

## Phase 2: Category Commands (Create, Update, Delete)

After this phase: `dotnet build` must pass. CategoryController still uses service for GET endpoints but switches to ISender for POST/PUT/DELETE.

### Task 2.1: CreateCategory command

**Files:**
- Create: `MechKey\MechKey.Application\Catalog\Commands\CreateCategory.cs`
- Modify: `MechKey\MechKey.WebAPI\Controllers\CategoryController.cs` (POST endpoint only)

- [ ] **Step 1: Create `CreateCategory.cs`**

```csharp
using Application.Comoon;
using Application.Interfaces.IApiClient.Redis;
using Domain.Entity;
using Domain.Exceptions;
using MediatR;
using MechkeyShop.Data;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Category;

namespace Application.Catalog.Commands;

public class CreateCategoryCommand : IRequest<Result<CategoryModel>>
{
    public string Name { get; init; } = default!;
}

public class CreateCategoryValidator
{
    public static void Validate(CreateCategoryCommand command)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        if (string.IsNullOrWhiteSpace(command.Name))
            throw new CategoryInvalidDataException("Category name is required");
    }
}

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryModel>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IRedisService _redisService;
    private readonly ICategoryMapping _categoryMapping;

    public CreateCategoryHandler(
        ApplicationDbContext dbContext,
        IRedisService redisService,
        ICategoryMapping categoryMapping)
    {
        _dbContext = dbContext;
        _redisService = redisService;
        _categoryMapping = categoryMapping;
    }

    public async Task<Result<CategoryModel>> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        CreateCategoryValidator.Validate(request);

        var entity = new Category { Id = Guid.NewGuid(), Name = request.Name };
        _dbContext.Categories.Add(entity);
        await _dbContext.SaveChangesAsync(ct);
        await _redisService.RemoveByPrefixAsync("category", ct);

        return Result<CategoryModel>.Success("Add category success", _categoryMapping.ToCategoryModel(entity));
    }
}
```

- [ ] **Step 2: Refactor CategoryController.Create endpoint**

Open `MechKey\MechKey.WebAPI\Controllers\CategoryController.cs`.

Remove: `using Application.Interfaces.IServices;`
Add: `using Application.Catalog.Commands;` and `using MediatR;`

Replace field:
```csharp
// OLD
private readonly ICategoryService categoryService;
public CategoryController(ICategoryService categoryService)
{
    this.categoryService = categoryService;
}
```

With:
```csharp
private readonly ISender _sender;
private readonly ICategoryService _categoryService;

public CategoryController(ISender sender, ICategoryService categoryService)
{
    _sender = sender;
    _categoryService = categoryService;
}
```

Replace Create method body:
```csharp
// OLD
[HttpPost]
public async Task<IActionResult> Create(CreateCategoryModel model, CancellationToken cancellationToken = default)
{
    var entity = await categoryService.AddAsync(model, cancellationToken);
    return CreatedAtAction(nameof(GetById), new { id = entity.Data.Id }, entity);
}
```

With:
```csharp
[HttpPost]
public async Task<IActionResult> Create(CreateCategoryModel model, CancellationToken cancellationToken = default)
{
    var result = await _sender.Send(new CreateCategoryCommand { Name = model.Name }, cancellationToken);
    return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
}
```

- [ ] **Step 3: Build**

```powershell
dotnet build "MechKey\MechKey.WebAPI\WebAPI.csproj"
```

Expected: Build succeeded. 0 errors.

- [ ] **Step 4: Commit**

```bash
git add MechKey/MechKey.Application/Catalog/Commands/CreateCategory.cs MechKey/MechKey.WebAPI/Controllers/CategoryController.cs
git commit -m "feat: add CreateCategory command and refactor CategoryController.Create"
```

### Task 2.2: UpdateCategory command

**Files:**
- Create: `MechKey\MechKey.Application\Catalog\Commands\UpdateCategory.cs`
- Modify: `MechKey\MechKey.WebAPI\Controllers\CategoryController.cs` (PUT endpoint only)

- [ ] **Step 1: Create `UpdateCategory.cs`**

```csharp
using Application.Comoon;
using Application.Interfaces.IApiClient.Redis;
using Domain.Entity;
using Domain.Exceptions;
using MediatR;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Category;

namespace Application.Catalog.Commands;

public class UpdateCategoryCommand : IRequest<Result<CategoryModel>>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
}

public class UpdateCategoryValidator
{
    public static void Validate(UpdateCategoryCommand command)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        if (Guid.Equals(command.Id, Guid.Empty))
            throw new CategoryInvalidDataException("Category Id is required");

        if (string.IsNullOrWhiteSpace(command.Name))
            throw new CategoryInvalidDataException("Category name is required");
    }
}

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, Result<CategoryModel>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IRedisService _redisService;
    private readonly ICategoryMapping _categoryMapping;
    private readonly ILogger<UpdateCategoryHandler> _logger;

    public UpdateCategoryHandler(
        ApplicationDbContext dbContext,
        IRedisService redisService,
        ICategoryMapping categoryMapping,
        ILogger<UpdateCategoryHandler> logger)
    {
        _dbContext = dbContext;
        _redisService = redisService;
        _categoryMapping = categoryMapping;
        _logger = logger;
    }

    public async Task<Result<CategoryModel>> Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        UpdateCategoryValidator.Validate(request);

        var entity = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (entity == null)
        {
            _logger.LogWarning("Category not found in {Method}. CategoryId: {CategoryId}", nameof(Handle), request.Id);
            throw new CategoryNotFoundException();
        }

        entity.Name = request.Name;
        entity.LastUpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(ct);
        await _redisService.RemoveByPrefixAsync("category", ct);

        return Result<CategoryModel>.Success("Update category success", _categoryMapping.ToCategoryModel(entity));
    }
}
```

- [ ] **Step 2: Refactor CategoryController.UpdateById endpoint**

Add `using Application.Catalog.Commands;` (if not already from Task 2.1).

Replace UpdateById method body:
```csharp
// OLD
[HttpPut("{id:guid}")]
public async Task<IActionResult> UpdateById(Guid id, CategoryModel model, CancellationToken cancellationToken = default)
{
    if (id != model.Id)
        return BadRequest("ID mismatch");
    var entity = await categoryService.UpdateAsync(model, cancellationToken);
    return Ok(entity);
}
```

With:
```csharp
[HttpPut("{id:guid}")]
public async Task<IActionResult> UpdateById(Guid id, CategoryModel model, CancellationToken cancellationToken = default)
{
    if (id != model.Id)
        return BadRequest("ID mismatch");
    var result = await _sender.Send(new UpdateCategoryCommand { Id = model.Id, Name = model.Name }, cancellationToken);
    return Ok(result);
}
```

- [ ] **Step 3: Build**

```powershell
dotnet build "MechKey\MechKey.WebAPI\WebAPI.csproj"
```

Expected: Build succeeded. 0 errors.

- [ ] **Step 4: Commit**

```bash
git add MechKey/MechKey.Application/Catalog/Commands/UpdateCategory.cs MechKey/MechKey.WebAPI/Controllers/CategoryController.cs
git commit -m "feat: add UpdateCategory command and refactor CategoryController.UpdateById"
```

### Task 2.3: DeleteCategory command

**Files:**
- Create: `MechKey\MechKey.Application\Catalog\Commands\DeleteCategory.cs`
- Modify: `MechKey\MechKey.WebAPI\Controllers\CategoryController.cs` (DELETE endpoint only)

- [ ] **Step 1: Create `DeleteCategory.cs`**

```csharp
using Application.Comoon;
using Application.Interfaces.IApiClient.Redis;
using Domain.Entity;
using Domain.Exceptions;
using MediatR;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Catalog.Commands;

public class DeleteCategoryCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IRedisService _redisService;
    private readonly ILogger<DeleteCategoryHandler> _logger;

    public DeleteCategoryHandler(
        ApplicationDbContext dbContext,
        IRedisService redisService,
        ILogger<DeleteCategoryHandler> logger)
    {
        _dbContext = dbContext;
        _redisService = redisService;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        var entity = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (entity == null)
        {
            _logger.LogWarning("Category not found in {Method}. CategoryId: {CategoryId}", nameof(Handle), request.Id);
            throw new CategoryNotFoundException();
        }

        _dbContext.Categories.Remove(entity);
        await _dbContext.SaveChangesAsync(ct);
        await _redisService.RemoveByPrefixAsync("category", ct);

        return Result.Success("Delete category success");
    }
}
```

- [ ] **Step 2: Refactor CategoryController.Delete endpoint**

Replace Delete method body:
```csharp
// OLD
[HttpDelete("{id:guid}")]
public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
{
    var result = await categoryService.DeleteAsync(id, cancellationToken);
    return Ok(result);
}
```

With:
```csharp
[HttpDelete("{id:guid}")]
public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
{
    var result = await _sender.Send(new DeleteCategoryCommand { Id = id }, cancellationToken);
    return Ok(result);
}
```

- [ ] **Step 3: Build**

```powershell
dotnet build "MechKey\MechKey.WebAPI\WebAPI.csproj"
```

Expected: Build succeeded. 0 errors.

- [ ] **Step 4: Commit**

```bash
git add MechKey/MechKey.Application/Catalog/Commands/DeleteCategory.cs MechKey/MechKey.WebAPI/Controllers/CategoryController.cs
git commit -m "feat: add DeleteCategory command and refactor CategoryController.Delete"
```

---

## Phase 3: Category Queries (GetList, GetById)

After this phase: CategoryController fully migrated to ISender. All category endpoints use MediatR.

### Task 3.1: GetCategoryList query

**Files:**
- Create: `MechKey\MechKey.Application\Catalog\Queries\GetCategoryList.cs`
- Modify: `MechKey\MechKey.WebAPI\Controllers\CategoryController.cs` (GET list endpoint only)

- [ ] **Step 1: Create `GetCategoryList.cs`**

```csharp
using Application.Comoon;
using Application.Interfaces.IApiClient.Redis;
using MediatR;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Category;

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
    private readonly ICategoryMapping _categoryMapping;

    public GetCategoryListHandler(
        ApplicationDbContext dbContext,
        IRedisService redisService,
        ICategoryMapping categoryMapping)
    {
        _dbContext = dbContext;
        _redisService = redisService;
        _categoryMapping = categoryMapping;
    }

    public async Task<Result<PagedResult<CategoryModel>>> Handle(GetCategoryListQuery request, CancellationToken ct)
    {
        string key = $"category-{request.Page}-{request.PageSize}";
        var data = _redisService.Get<PagedResult<CategoryModel>>(key);

        if (data is null)
        {
            var query = _dbContext.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(c => c.Name.Contains(request.SearchTerm));
            }

            int totalCount = await query.CountAsync(ct);

            var items = await query
                .Where(c => c.IsDeleted == false)
                .OrderBy(c => c.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(ct);

            data = new PagedResult<CategoryModel>
            {
                Items = _categoryMapping.ToListCategoryModel(items),
                TotalItems = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };

            await _redisService.Set(key, data, 120);
        }

        return Result<PagedResult<CategoryModel>>.Success("Get category list success", data);
    }
}
```

- [ ] **Step 2: Refactor CategoryController.GetList endpoint**

Add `using Application.Catalog.Queries;`.

Replace GetList method body:
```csharp
// OLD
[HttpGet("list")]
public async Task<Result<PagedResult<CategoryModel>>> GetList(int page = 1, int pageSize = 10, string searchTerm = "", CancellationToken cancellationToken = default)
{
    PaginationReqModel paginationReqModel = new PaginationReqModel()
    {
        Page = page,
        PageSize = pageSize,
        SearchTerm = searchTerm
    };
    Result<PagedResult<CategoryModel>> result = await categoryService.GetAllAsync(paginationReqModel, cancellationToken);
    return result;
}
```

With:
```csharp
[HttpGet("list")]
public async Task<Result<PagedResult<CategoryModel>>> GetList(int page = 1, int pageSize = 10, string searchTerm = "", CancellationToken cancellationToken = default)
{
    return await _sender.Send(new GetCategoryListQuery
    {
        Page = page,
        PageSize = pageSize,
        SearchTerm = searchTerm
    }, cancellationToken);
}
```

- [ ] **Step 3: Build**

```powershell
dotnet build "MechKey\MechKey.WebAPI\WebAPI.csproj"
```

Expected: Build succeeded. 0 errors.

- [ ] **Step 4: Commit**

```bash
git add MechKey/MechKey.Application/Catalog/Queries/GetCategoryList.cs MechKey/MechKey.WebAPI/Controllers/CategoryController.cs
git commit -m "feat: add GetCategoryList query and refactor CategoryController.GetList"
```

### Task 3.2: GetCategoryById query

**Files:**
- Create: `MechKey\MechKey.Application\Catalog\Queries\GetCategoryById.cs`
- Modify: `MechKey\MechKey.WebAPI\Controllers\CategoryController.cs` (GET by id endpoint only)

- [ ] **Step 1: Create `GetCategoryById.cs`**

```csharp
using Application.Comoon;
using Domain.Exceptions;
using MediatR;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Category;

namespace Application.Catalog.Queries;

public class GetCategoryByIdQuery : IRequest<Result<CategoryModel>>
{
    public Guid Id { get; init; }
}

public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryModel>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICategoryMapping _categoryMapping;
    private readonly ILogger<GetCategoryByIdHandler> _logger;

    public GetCategoryByIdHandler(
        ApplicationDbContext dbContext,
        ICategoryMapping categoryMapping,
        ILogger<GetCategoryByIdHandler> logger)
    {
        _dbContext = dbContext;
        _categoryMapping = categoryMapping;
        _logger = logger;
    }

    public async Task<Result<CategoryModel>> Handle(GetCategoryByIdQuery request, CancellationToken ct)
    {
        var entity = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (entity == null)
        {
            _logger.LogWarning("Category not found in {Method}. CategoryId: {CategoryId}", nameof(Handle), request.Id);
            throw new CategoryNotFoundException();
        }

        return Result<CategoryModel>.Success("Get category by id success", _categoryMapping.ToCategoryModel(entity));
    }
}
```

- [ ] **Step 2: Refactor CategoryController.GetById endpoint**

Replace GetById method body:
```csharp
// OLD
[HttpGet("{id:guid}")]
public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
{
    var entity = await categoryService.GetByIdAsync(id, cancellationToken);
    return Ok(entity);
}
```

With:
```csharp
[HttpGet("{id:guid}")]
public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
{
    var result = await _sender.Send(new GetCategoryByIdQuery { Id = id }, cancellationToken);
    return Ok(result);
}
```

- [ ] **Step 3: Now remove the `_categoryService` field from CategoryController**

CategoryController is now fully migrated. Remove the `private readonly ICategoryService _categoryService;` field and the `ICategoryService categoryService` constructor parameter. The constructor should become:

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
    // ... endpoints
}
```

Remove unused using: `using Application.Interfaces.IServices;` if CategoryController was the only consumer.

- [ ] **Step 4: Build**

```powershell
dotnet build "MechKey\MechKey.WebAPI\WebAPI.csproj"
```

Expected: Build succeeded. 0 errors.

- [ ] **Step 5: Commit**

```bash
git add MechKey/MechKey.Application/Catalog/Queries/GetCategoryById.cs MechKey/MechKey.WebAPI/Controllers/CategoryController.cs
git commit -m "feat: add GetCategoryById query, fully migrate CategoryController to ISender"
```

### Phase 3 Verification

CategoryController now has zero service dependencies. All 5 endpoints use `_sender.Send()`. Existing services remain registered and untouched.

---

## Phase 4: Product Queries (GetList, GetById, GetBestSeller)

After this phase: ProductController GET endpoints use ISender. POST/PUT/DELETE still use service (migrated in Phase 5).

### Task 4.1: GetProductList query

**Files:**
- Create: `MechKey\MechKey.Application\Catalog\Queries\GetProductList.cs`
- Modify: `MechKey\MechKey.WebAPI\Controllers\ProductController.cs` (GET list endpoint only)

- [ ] **Step 1: Create `GetProductList.cs`**

```csharp
using Application.Comoon;
using Application.Interfaces.IApiClient.Redis;
using MediatR;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Product;

namespace Application.Catalog.Queries;

public class GetProductListQuery : IRequest<Result<PagedResult<ProductModel>>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string SearchTerm { get; init; } = "";
    public string CategoryId { get; init; } = "";
    public string SortCol { get; init; } = "";
    public bool Asc { get; init; } = true;
}

public class GetProductListHandler : IRequestHandler<GetProductListQuery, Result<PagedResult<ProductModel>>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IRedisService _redisService;
    private readonly IProductMapping _productMapping;

    public GetProductListHandler(
        ApplicationDbContext dbContext,
        IRedisService redisService,
        IProductMapping productMapping)
    {
        _dbContext = dbContext;
        _redisService = redisService;
        _productMapping = productMapping;
    }

    public async Task<Result<PagedResult<ProductModel>>> Handle(GetProductListQuery request, CancellationToken ct)
    {
        string key = $"product-{request.Page}-{request.PageSize}-{request.CategoryId}-{request.SearchTerm}-{request.SortCol}-{request.Asc}";
        var data = _redisService.Get<PagedResult<ProductModel>>(key);
        if (data is not null)
        {
            return Result<PagedResult<ProductModel>>.Success("Get list product from cache", data);
        }

        var query = _dbContext.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(request.CategoryId))
        {
            query = query.Where(q => q.Category.Id.ToString() == request.CategoryId);
        }

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(u => u.Name.Contains(request.SearchTerm));
        }

        if (!string.IsNullOrEmpty(request.SortCol))
        {
            switch (request.SortCol)
            {
                case "price":
                    query = request.Asc ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price);
                    break;
                case "createdAt":
                    query = request.Asc ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt);
                    break;
                default:
                    query = query.OrderBy(p => p.Id);
                    break;
            }
        }

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        data = new PagedResult<ProductModel>
        {
            Items = _productMapping.ToListProductModel(items),
            TotalItems = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
        };

        await _redisService.Set(key, data, 15);

        return Result<PagedResult<ProductModel>>.Success("Get list product success", data);
    }
}
```

- [ ] **Step 2: Refactor ProductController.Get endpoint**

Open `MechKey\MechKey.WebAPI\Controllers\ProductController.cs`.

Add usings:
```csharp
using Application.Catalog.Queries;
using MediatR;
```

Update constructor to accept both ISender and IProductService (for now):
```csharp
private readonly ISender _sender;
private readonly IProductService _productService;

public ProductController(ISender sender, IProductService productService)
{
    _sender = sender;
    _productService = productService;
}
```

Replace Get method:
```csharp
// OLD
[HttpGet("list")]
public async Task<IActionResult> Get(
    int page = 1,
    int pageSize = 10,
    string searchTerm = "",
    string categoryId = "",
    string sortCol = "",
    bool asc = true,
    CancellationToken cancellationToken = default)
{
    PaginationReqModel pagination = new PaginationReqModel()
    {
        Page = page,
        PageSize = pageSize,
        SearchTerm = searchTerm,
    };

    var result = await productService.GetAllAsync(pagination, categoryId, sortCol, asc, cancellationToken);
    return Ok(result);
}
```

With:
```csharp
[HttpGet("list")]
public async Task<IActionResult> Get(
    int page = 1,
    int pageSize = 10,
    string searchTerm = "",
    string categoryId = "",
    string sortCol = "",
    bool asc = true,
    CancellationToken cancellationToken = default)
{
    var result = await _sender.Send(new GetProductListQuery
    {
        Page = page,
        PageSize = pageSize,
        SearchTerm = searchTerm,
        CategoryId = categoryId,
        SortCol = sortCol,
        Asc = asc
    }, cancellationToken);
    return Ok(result);
}
```

Also remove unused `using Shared.Common;` if Get was the only consumer of `PaginationReqModel` (it was constructed inline, now gone).

- [ ] **Step 3: Build**

```powershell
dotnet build "MechKey\MechKey.WebAPI\WebAPI.csproj"
```

Expected: Build succeeded. 0 errors.

- [ ] **Step 4: Commit**

```bash
git add MechKey/MechKey.Application/Catalog/Queries/GetProductList.cs MechKey/MechKey.WebAPI/Controllers/ProductController.cs
git commit -m "feat: add GetProductList query and refactor ProductController.Get"
```

### Task 4.2: GetProductById query

**Files:**
- Create: `MechKey\MechKey.Application\Catalog\Queries\GetProductById.cs`
- Modify: `MechKey\MechKey.WebAPI\Controllers\ProductController.cs` (GET by id endpoint only)

- [ ] **Step 1: Create `GetProductById.cs`**

```csharp
using Application.Comoon;
using Domain.Exceptions;
using MediatR;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Product;

namespace Application.Catalog.Queries;

public class GetProductByIdQuery : IRequest<Result<ProductModel>>
{
    public Guid Id { get; init; }
}

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Result<ProductModel>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IProductMapping _productMapping;
    private readonly ILogger<GetProductByIdHandler> _logger;

    public GetProductByIdHandler(
        ApplicationDbContext dbContext,
        IProductMapping productMapping,
        ILogger<GetProductByIdHandler> logger)
    {
        _dbContext = dbContext;
        _productMapping = productMapping;
        _logger = logger;
    }

    public async Task<Result<ProductModel>> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var entity = await _dbContext.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct);

        if (entity == null)
        {
            _logger.LogWarning("Product not found in {Method}. ProductId: {ProductId}", nameof(Handle), request.Id);
            throw new ProductNotFoundException();
        }

        return Result<ProductModel>.Success("Get product by id success", _productMapping.ToProductModel(entity));
    }
}
```

- [ ] **Step 2: Refactor ProductController.GetById endpoint**

Replace GetById method:
```csharp
// OLD
[HttpGet("{id:guid}")]
public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
{
    var result = await productService.GetByIdAsync(id, cancellationToken);
    return Ok(result);
}
```

With:
```csharp
[HttpGet("{id:guid}")]
public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
{
    var result = await _sender.Send(new GetProductByIdQuery { Id = id }, cancellationToken);
    return Ok(result);
}
```

- [ ] **Step 3: Build**

```powershell
dotnet build "MechKey\MechKey.WebAPI\WebAPI.csproj"
```

Expected: Build succeeded. 0 errors.

- [ ] **Step 4: Commit**

```bash
git add MechKey/MechKey.Application/Catalog/Queries/GetProductById.cs MechKey/MechKey.WebAPI/Controllers/ProductController.cs
git commit -m "feat: add GetProductById query and refactor ProductController.GetById"
```

### Task 4.3: GetBestSellerProducts query

**Files:**
- Create: `MechKey\MechKey.Application\Catalog\Queries\GetBestSellerProducts.cs`
- No controller change needed — `GetBestSellerAsync` is called from `MechkeyShop\Controllers\ProductController.cs` (MVC), not from WebAPI. This query is created for future use and completeness.

> Note: The handler includes `.Include(p => p.Category)` to get CategoryName in mapping. This matches the original `GetBestSellerAsync` behavior which called `.Include(p => p.Category)`.

- [ ] **Step 1: Create `GetBestSellerProducts.cs`**

```csharp
using Application.Comoon;
using Domain.Exceptions;
using MediatR;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Product;

namespace Application.Catalog.Queries;

public class GetBestSellerProductsQuery : IRequest<Result<IEnumerable<ProductModel>>>
{
}

public class GetBestSellerProductsHandler : IRequestHandler<GetBestSellerProductsQuery, Result<IEnumerable<ProductModel>>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IProductMapping _productMapping;
    private readonly ILogger<GetBestSellerProductsHandler> _logger;

    public GetBestSellerProductsHandler(
        ApplicationDbContext dbContext,
        IProductMapping productMapping,
        ILogger<GetBestSellerProductsHandler> logger)
    {
        _dbContext = dbContext;
        _productMapping = productMapping;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<ProductModel>>> Handle(GetBestSellerProductsQuery request, CancellationToken ct)
    {
        try
        {
            var items = await _dbContext.Products
                .OrderByDescending(p => p.SellCount)
                .Take(4)
                .Include(p => p.Category)
                .ToListAsync(ct);

            return Result<IEnumerable<ProductModel>>.Success("Get Best seller", _productMapping.ToListProductModel(items));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in {Method}. Message: {Message}", nameof(Handle), ex.Message);
            throw new ProductHandleFailedException();
        }
    }
}
```

- [ ] **Step 2: Build**

```powershell
dotnet build "MechKey\MechKey.WebAPI\WebAPI.csproj"
```

Expected: Build succeeded. 0 errors.

- [ ] **Step 3: Commit**

```bash
git add MechKey/MechKey.Application/Catalog/Queries/GetBestSellerProducts.cs
git commit -m "feat: add GetBestSellerProducts query (no controller change needed)"
```

### Phase 4 Verification

All ProductController GET endpoints now use ISender. POST/PUT/DELETE still use `_productService`. `dotnet build` passes.

---

## Phase 5: Product Commands (Create, Update, Delete)

After this phase: ProductController fully migrated to ISender.

### Task 5.1: CreateProduct command

**Files:**
- Create: `MechKey\MechKey.Application\Catalog\Commands\CreateProduct.cs`
- Modify: `MechKey\MechKey.WebAPI\Controllers\ProductController.cs` (POST endpoint only)

- [ ] **Step 1: Create `CreateProduct.cs`**

```csharp
using Application.Comoon;
using Application.Events;
using Application.Interfaces.IApiClient.Redis;
using Application.Interfaces.IApiClient.Supabase;
using Application.Interfaces.IUnitOfWork;
using Application.Validators;
using Domain.Entity;
using Domain.Exceptions;
using EventBus;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.ImageUpload;
using Shared.ViewModels.Product;

namespace Application.Catalog.Commands;

public class CreateProductCommand : IRequest<Result<ProductModel>>
{
    public string Name { get; init; } = default!;
    public string Description { get; init; } = string.Empty;
    public double Price { get; init; }
    public Guid CategoryId { get; init; }
    public List<VariantAttribute> Variants { get; init; } = new();
    public string ImageData { get; init; } = default!;
}

public class CreateProductValidator
{
    public static void Validate(CreateProductCommand command)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ProductInvalidDataException("Product's name is required");

        if (Guid.Equals(command.CategoryId, Guid.Empty))
            throw new ProductInvalidDataException("Product's categoryId is required");

        if (string.IsNullOrEmpty(command.ImageData))
            throw new ProductInvalidDataException("Product's image data is required");

        if (command.Price < 0)
            throw new ProductInvalidDataException("Price must be positive");
    }
}

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<ProductModel>>
{
    private readonly IProductUnitOfWork _unitOfWork;
    private readonly ISupabaseService _supabaseService;
    private readonly IEventBus _eventBus;
    private readonly IRedisService _redisService;
    private readonly IProductMapping _productMapping;
    private readonly ILogger<CreateProductHandler> _logger;

    public CreateProductHandler(
        IProductUnitOfWork unitOfWork,
        ISupabaseService supabaseService,
        IEventBus eventBus,
        IRedisService redisService,
        IProductMapping productMapping,
        ILogger<CreateProductHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _supabaseService = supabaseService;
        _eventBus = eventBus;
        _redisService = redisService;
        _productMapping = productMapping;
        _logger = logger;
    }

    public async Task<Result<ProductModel>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        CreateProductValidator.Validate(request);

        var base64Parts = request.ImageData.Split(',');
        if (base64Parts.Length != 2)
            throw new InvalidDataException("Base64 is invalid");

        byte[] imageBytes = Convert.FromBase64String(base64Parts[1]);
        var uploadImageResponse = await _supabaseService.UploadImage(imageBytes);

        if (!uploadImageResponse.IsSuccess)
            throw new ProductImageHandleFailedException();

        try
        {
            await _unitOfWork.BeginTransactionAsync(ct);

            var entity = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                CategoryId = request.CategoryId,
                Price = request.Price,
                Variants = JsonConvert.SerializeObject(request.Variants),
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };

            ProductImage productImage = new ProductImage
            {
                FilePath = uploadImageResponse.Data.FilePath,
                Url = uploadImageResponse.Data.PublicUrl,
                ProductId = entity.Id,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
            };

            entity.ImageUrl = uploadImageResponse.Data.PublicUrl;

            var newEntity = await _unitOfWork.ProductRepository.CreateAsync(entity, ct);

            await _unitOfWork.CommitAsync(ct);
            await _redisService.RemoveByPrefixAsync("product", ct);
            return Result<ProductModel>.Success("Add product success", _productMapping.ToProductModel(newEntity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in {Method}. Model: {Model}, Message: {Message}", nameof(Handle), request, ex.Message);

            await _eventBus.PublishAsync(new DeleteImageEvent
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Url = uploadImageResponse.Data.PublicUrl
            }, ct);

            await _unitOfWork.RollbackAsync(ct);

            throw new ProductHandleFailedException();
        }
    }
}
```

- [ ] **Step 2: Refactor ProductController.Create endpoint**

Replace Create method:
```csharp
// OLD
[HttpPost]
public async Task<IActionResult> Create(CreateProductModel model, CancellationToken cancellationToken = default)
{
    var result = await productService.AddAsync(model, cancellationToken);
    return Ok(result);
}
```

With:
```csharp
[HttpPost]
public async Task<IActionResult> Create(CreateProductModel model, CancellationToken cancellationToken = default)
{
    var result = await _sender.Send(new CreateProductCommand
    {
        Name = model.Name,
        Description = model.Description,
        Price = model.Price,
        CategoryId = model.CategoryId,
        Variants = model.Variants ?? new(),
        ImageData = model.ImageData
    }, cancellationToken);
    return Ok(result);
}
```

- [ ] **Step 3: Build**

```powershell
dotnet build "MechKey\MechKey.WebAPI\WebAPI.csproj"
```

Expected: Build succeeded. 0 errors.

- [ ] **Step 4: Commit**

```bash
git add MechKey/MechKey.Application/Catalog/Commands/CreateProduct.cs MechKey/MechKey.WebAPI/Controllers/ProductController.cs
git commit -m "feat: add CreateProduct command and refactor ProductController.Create"
```

### Task 5.2: UpdateProduct command

**Files:**
- Create: `MechKey\MechKey.Application\Catalog\Commands\UpdateProduct.cs`
- Modify: `MechKey\MechKey.WebAPI\Controllers\ProductController.cs` (PUT endpoint only)

- [ ] **Step 1: Create `UpdateProduct.cs`**

```csharp
using Application.Comoon;
using Application.Events;
using Application.Interfaces.IApiClient.Redis;
using Application.Interfaces.IApiClient.Supabase;
using Application.Validators;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using EventBus;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.ImageUpload;
using Shared.ViewModels.Product;

namespace Application.Catalog.Commands;

public class UpdateProductCommand : IRequest<Result<ProductModel>>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string Description { get; init; } = string.Empty;
    public double Price { get; init; }
    public string? ImageUrl { get; init; }
    public string? ImageData { get; init; }
    public Guid CategoryId { get; init; }
    public List<VariantAttribute> Variants { get; init; } = new();
    public bool IsDeleted { get; init; }
}

public class UpdateProductValidator
{
    public static void Validate(UpdateProductCommand command)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        if (Guid.Equals(command.Id, Guid.Empty))
            throw new ProductInvalidDataException("Product Id is required");

        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ProductInvalidDataException("Product's name is required");

        if (command.Price < 0)
            throw new ProductInvalidDataException("Price must be positive");

        if (Guid.Equals(command.CategoryId, Guid.Empty))
            throw new ProductInvalidDataException("CategoryId is required");
    }
}

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Result<ProductModel>>
{
    private readonly IProductRepository<Product> _repository;
    private readonly IRedisService _redisService;
    private readonly ISupabaseService _supabaseService;
    private readonly IEventBus _eventBus;
    private readonly IProductMapping _productMapping;
    private readonly ILogger<UpdateProductHandler> _logger;

    public UpdateProductHandler(
        IProductRepository<Product> repository,
        IRedisService redisService,
        ISupabaseService supabaseService,
        IEventBus eventBus,
        IProductMapping productMapping,
        ILogger<UpdateProductHandler> logger)
    {
        _repository = repository;
        _redisService = redisService;
        _supabaseService = supabaseService;
        _eventBus = eventBus;
        _productMapping = productMapping;
        _logger = logger;
    }

    public async Task<Result<ProductModel>> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        UpdateProductValidator.Validate(request);

        var entity = await _repository.GetByIdAsync(request.Id, ct);
        if (entity == null)
        {
            _logger.LogWarning("Product not found in {Method}. ProductId: {ProductId}", nameof(Handle), request.Id);
            throw new ProductNotFoundException();
        }

        if (!string.IsNullOrEmpty(request.ImageData))
        {
            var base64Parts = request.ImageData.Split(',');
            if (base64Parts.Length != 2)
                throw new InvalidDataException("Base64 is invalid");

            byte[] imageBytes = Convert.FromBase64String(base64Parts[1]);
            var uploadImageResponse = await _supabaseService.UploadImage(imageBytes);

            if (uploadImageResponse.IsSuccess)
            {
                entity.ImageUrl = uploadImageResponse.Data.PublicUrl;

                await _eventBus.PublishAsync(new DeleteImageEvent
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    Url = entity.ImageUrl,
                }, ct);
            }
        }

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.LastUpdatedAt = DateTime.UtcNow;
        entity.Variants = JsonConvert.SerializeObject(request.Variants);
        entity.IsDeleted = request.IsDeleted;

        var result = await _repository.UpdateAsync(entity, ct);
        await _redisService.RemoveByPrefixAsync("product", ct);

        return Result<ProductModel>.Success("Update product success", _productMapping.ToProductModel(result));
    }
}
```

> Note: UpdateProduct keeps `IProductRepository<Product>` injection (not DbContext) because the original `ProductService.UpdateAsync` pattern calls `_repository.UpdateAsync(entity)` which is a repository-specific method. This is an exception to the "use DbContext directly" rule, documented here.

- [ ] **Step 2: Refactor ProductController.Update endpoint**

Replace Update method:
```csharp
// OLD
[HttpPut]
public async Task<IActionResult> Update(UpdateProductModel model, CancellationToken cancellationToken = default)
{
    var result = await productService.UpdateAsync(model, cancellationToken);
    return Ok(result);
}
```

With:
```csharp
[HttpPut]
public async Task<IActionResult> Update(UpdateProductModel model, CancellationToken cancellationToken = default)
{
    var result = await _sender.Send(new UpdateProductCommand
    {
        Id = model.Id,
        Name = model.Name,
        Description = model.Description,
        Price = model.Price,
        ImageUrl = model.ImageUrl,
        ImageData = model.ImageData,
        CategoryId = model.CategoryId,
        Variants = model.Variants ?? new(),
        IsDeleted = model.IsDeleted
    }, cancellationToken);
    return Ok(result);
}
```

- [ ] **Step 3: Build**

```powershell
dotnet build "MechKey\MechKey.WebAPI\WebAPI.csproj"
```

Expected: Build succeeded. 0 errors.

- [ ] **Step 4: Commit**

```bash
git add MechKey/MechKey.Application/Catalog/Commands/UpdateProduct.cs MechKey/MechKey.WebAPI/Controllers/ProductController.cs
git commit -m "feat: add UpdateProduct command and refactor ProductController.Update"
```

### Task 5.3: DeleteProduct command

**Files:**
- Create: `MechKey\MechKey.Application\Catalog\Commands\DeleteProduct.cs`
- Modify: `MechKey\MechKey.WebAPI\Controllers\ProductController.cs` (DELETE endpoint only)

- [ ] **Step 1: Create `DeleteProduct.cs`**

```csharp
using Application.Comoon;
using Application.Interfaces.IApiClient.Redis;
using Domain.Entity;
using Domain.Exceptions;
using Domain.IRepositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Catalog.Commands;

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
```

> Note: DeleteProduct uses `IProductRepository<Product>` because `_repository.DeleteAsync()` is a repository-specific method. Same exception as UpdateProduct.

- [ ] **Step 2: Refactor ProductController.Delete endpoint**

Replace Delete method:
```csharp
// OLD
[HttpDelete("{id:guid}")]
public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
{
    var result = await productService.DeleteAsync(id, cancellationToken);
    return Ok(result);
}
```

With:
```csharp
[HttpDelete("{id:guid}")]
public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
{
    var result = await _sender.Send(new DeleteProductCommand { Id = id }, cancellationToken);
    return Ok(result);
}
```

- [ ] **Step 3: Remove `_productService` from ProductController**

ProductController is now fully migrated. Remove `IProductService` injection. Final constructor:

```csharp
private readonly ISender _sender;

public ProductController(ISender sender)
{
    _sender = sender;
}
```

Remove unused usings: `using Application.Interfaces.IServices;`, `using Shared.Common;`.

- [ ] **Step 4: Build**

```powershell
dotnet build "MechKey\MechKey.WebAPI\WebAPI.csproj"
```

Expected: Build succeeded. 0 errors.

- [ ] **Step 5: Commit**

```bash
git add MechKey/MechKey.Application/Catalog/Commands/DeleteProduct.cs MechKey/MechKey.WebAPI/Controllers/ProductController.cs
git commit -m "feat: add DeleteProduct command, fully migrate ProductController to ISender"
```

### Phase 5 Verification

ProductController now has zero service dependencies. All 5 endpoints use `_sender.Send()`. Existing services remain registered.

---

## Phase 6: ProductRating + Dashboard

### Task 6.1: CreateProductRating command

**Files:**
- Create: `MechKey\MechKey.Application\Catalog\Commands\CreateProductRating.cs`
- No controller change in WebAPI. The MVC `MechkeyShop\Controllers\ProductController.SubmitRating` is the consumer.

- [ ] **Step 1: Create `CreateProductRating.cs`**

```csharp
using Application.Comoon;
using Application.Validators;
using Domain.Entity;
using Domain.Exceptions;
using MediatR;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Product;

namespace Application.Catalog.Commands;

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
    private readonly ApplicationDbContext _dbContext;
    private readonly IProductRatingMapping _productRatingMapping;
    private readonly ILogger<CreateProductRatingHandler> _logger;

    public CreateProductRatingHandler(
        ApplicationDbContext dbContext,
        IProductRatingMapping productRatingMapping,
        ILogger<CreateProductRatingHandler> logger)
    {
        _dbContext = dbContext;
        _productRatingMapping = productRatingMapping;
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
```

- [ ] **Step 2: Build**

```powershell
dotnet build "MechKey\MechKey.WebAPI\WebAPI.csproj"
```

Expected: Build succeeded. 0 errors.

- [ ] **Step 3: Commit**

```bash
git add MechKey/MechKey.Application/Catalog/Commands/CreateProductRating.cs
git commit -m "feat: add CreateProductRating command"
```

### Task 6.2: GetProductRatings query

**Files:**
- Create: `MechKey\MechKey.Application\Catalog\Queries\GetProductRatings.cs`
- No controller change needed (used by MVC shop, not WebAPI).

- [ ] **Step 1: Create `GetProductRatings.cs`**

```csharp
using Application.Comoon;
using MediatR;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.Mapping.Interfaces;
using Shared.ViewModels.Product;

namespace Application.Catalog.Queries;

public class GetProductRatingsQuery : IRequest<Result<PagedResult<ProductRatingModel>>>
{
    public Guid ProductId { get; init; }
    public int PageSize { get; init; } = 4;
    public bool AscOrder { get; init; } = false;
}

public class GetProductRatingsHandler : IRequestHandler<GetProductRatingsQuery, Result<PagedResult<ProductRatingModel>>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IProductRatingMapping _productRatingMapping;

    public GetProductRatingsHandler(
        ApplicationDbContext dbContext,
        IProductRatingMapping productRatingMapping)
    {
        _dbContext = dbContext;
        _productRatingMapping = productRatingMapping;
    }

    public async Task<Result<PagedResult<ProductRatingModel>>> Handle(GetProductRatingsQuery request, CancellationToken ct)
    {
        var query = _dbContext.ProductRatings
            .Where(pr => pr.ProductId == request.ProductId)
            .AsQueryable();

        if (request.AscOrder)
        {
            query = query.OrderBy(pr => pr.RatedAt);
        }
        else
        {
            query = query.OrderByDescending(pr => pr.Id);
        }

        var data = await query
            .Take(request.PageSize)
            .Include(pr => pr.User)
            .Select(pr => _productRatingMapping.ToProductRatingModel(pr))
            .ToListAsync(ct);

        var totalItems = await _dbContext.ProductRatings
            .Where(pr => pr.ProductId == request.ProductId)
            .CountAsync(ct);

        var result = new PagedResult<ProductRatingModel>
        {
            Page = 1,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            Items = data,
            TotalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize)
        };

        return Result<PagedResult<ProductRatingModel>>.Success("Get rating success", result);
    }
}
```

- [ ] **Step 2: Build**

```powershell
dotnet build "MechKey\MechKey.WebAPI\WebAPI.csproj"
```

Expected: Build succeeded. 0 errors.

- [ ] **Step 3: Commit**

```bash
git add MechKey/MechKey.Application/Catalog/Queries/GetProductRatings.cs
git commit -m "feat: add GetProductRatings query"
```

### Task 6.3: GetDashboard query + refactor DashboardController

**Files:**
- Create: `MechKey\MechKey.Application\Catalog\Queries\GetDashboard.cs`
- Modify: `MechKey\MechKey.WebAPI\Controllers\DashboardController.cs`

- [ ] **Step 1: Create `GetDashboard.cs`**

```csharp
using Application.Comoon;
using Domain.Enum;
using MediatR;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Common;
using Shared.ViewModels.Dashboard;

namespace Application.Catalog.Queries;

public class GetDashboardQuery : IRequest<Result<DashboardData>>
{
}

public class GetDashboardHandler : IRequestHandler<GetDashboardQuery, Result<DashboardData>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<GetDashboardHandler> _logger;

    public GetDashboardHandler(
        ApplicationDbContext dbContext,
        ILogger<GetDashboardHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<DashboardData>> Handle(GetDashboardQuery request, CancellationToken ct)
    {
        try
        {
            var orders = _dbContext.Orders.AsQueryable();
            var products = _dbContext.Products.AsQueryable();
            var users = _dbContext.ApplicationUsers.AsQueryable();

            var dashboardData = new DashboardData
            {
                TotalRevenue = (decimal)await orders.SumAsync(o => o.TotalAmount, ct),
                TotalSellCount = (int)await products.SumAsync(p => p.SellCount, ct),
                TotalOrderCount = await orders.CountAsync(ct),
                TotalOrderCompleted = await orders.CountAsync(o => o.Status == OrderStatus.Completed, ct),
                TotalOrderPending = await orders.CountAsync(o => o.Status == OrderStatus.Pending, ct),
                TotalProductAvailable = await products.CountAsync(p => !p.IsDeleted, ct),
                TotalUserAvailable = await users.CountAsync(u => !u.IsDeleted, ct)
            };

            return Result<DashboardData>.Success("Get dashboard data success", dashboardData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in {Method}. Message: {Message}", nameof(Handle), ex.Message);
            return Result<DashboardData>.Failure("Failed to get dashboard data", new DashboardData());
        }
    }
}
```

- [ ] **Step 2: Refactor DashboardController**

Open `MechKey\MechKey.WebAPI\Controllers\DashboardController.cs`.

Replace entire file content:
```csharp
using Application.Catalog.Queries;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.ViewModels.Dashboard;

namespace WebAPI.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{v:apiVersion}/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ISender _sender;

        public DashboardController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<Result<DashboardData>> GetDashboardData(CancellationToken cancellationToken = default)
        {
            return await _sender.Send(new GetDashboardQuery(), cancellationToken);
        }
    }
}
```

- [ ] **Step 3: Build**

```powershell
dotnet build "MechKey\MechKey.WebAPI\WebAPI.csproj"
```

Expected: Build succeeded. 0 errors.

- [ ] **Step 4: Commit**

```bash
git add MechKey/MechKey.Application/Catalog/Queries/GetDashboard.cs MechKey/MechKey.WebAPI/Controllers/DashboardController.cs
git commit -m "feat: add GetDashboard query, migrate DashboardController to ISender"
```

### Phase 6 Verification

All 14 commands/queries created. All 3 WebAPI controllers (Category, Product, Dashboard) fully migrated to ISender.

---

## Phase 7: Final Verification & Cleanup

### Task 7.1: Full solution build

- [ ] **Step 1: Build entire solution**

```powershell
dotnet build
```

Expected: Build succeeded with 0 errors across all projects.

### Task 7.2: Verify old services still registered (not accidentally removed)

- [ ] **Step 1: Confirm `Application\DependencyInjection.cs` still has all old service registrations**

File should still contain:
```csharp
service.AddScoped<ICategoryService, CategoryService>();
service.AddScoped<IProductService, ProductService>();
service.AddScoped<IProductRatingService, ProductRatingService>();
service.AddScoped<IDashboardService, DashboardService>();
```

These remain registered so MVC shop or other consumers continue working.

### Task 7.3: Quick sanity checklist

- [ ] CategoryController: 5 endpoints, all use `_sender.Send()`
- [ ] ProductController: 5 endpoints, all use `_sender.Send()`
- [ ] DashboardController: 1 endpoint, uses `_sender.Send()`
- [ ] All command files contain: Request class + Validator class + Handler class in 1 file
- [ ] All query files contain: Request class + Handler class in 1 file
- [ ] Handlers use `ApplicationDbContext` directly (except UpdateProduct + DeleteProduct which use `IProductRepository<Product>` because of repository-specific methods `UpdateAsync`/`DeleteAsync`)
- [ ] Old service files untouched (`CategoryService.cs`, `ProductService.cs`, `ProductRatingService.cs`, `DashboardService.cs`)
- [ ] Old validator files untouched (`CategoryValidator.cs`, `ProductValidator.cs`)

---

## Exception Notes

### UpdateProduct & DeleteProduct use IProductRepository

These two handlers inject `IProductRepository<Product>` instead of `ApplicationDbContext` because the original service code calls:
- `_repository.UpdateAsync(entity, ct)` — a repository method that may include soft-delete logic
- `_repository.DeleteAsync(entity, ct)` — a repository method that may include soft-delete logic

This is documented as an intentional exception. All other handlers use `ApplicationDbContext` directly.

### GetBestSellerProducts not wired to any WebAPI controller

This query exists for completeness (mirrors `IProductService.GetBestSellerAsync`). It is consumed by `MechkeyShop\Controllers\ProductController.cs` (MVC shop), not by WebAPI. Wire it there when the MVC controllers are refactored in a separate plan.

### CreateProductRating not wired to any WebAPI controller

Same as above — consumed by the MVC shop's `SubmitRating` action, not by WebAPI. Wire in a separate MVC refactor plan.

---

## Rollback Plan

If something breaks after migration:
1. Revert controller changes (controllers injected both ISender and IService during transition — pick the old service call)
2. Old services remain fully registered and functional
3. Old service interfaces unchanged
4. No database schema changes

---

## Files Changed Summary

**Created (14 files):**
```
MechKey.Application/Catalog/Commands/CreateCategory.cs
MechKey.Application/Catalog/Commands/UpdateCategory.cs
MechKey.Application/Catalog/Commands/DeleteCategory.cs
MechKey.Application/Catalog/Commands/CreateProduct.cs
MechKey.Application/Catalog/Commands/UpdateProduct.cs
MechKey.Application/Catalog/Commands/DeleteProduct.cs
MechKey.Application/Catalog/Commands/CreateProductRating.cs
MechKey.Application/Catalog/Queries/GetCategoryList.cs
MechKey.Application/Catalog/Queries/GetCategoryById.cs
MechKey.Application/Catalog/Queries/GetProductList.cs
MechKey.Application/Catalog/Queries/GetProductById.cs
MechKey.Application/Catalog/Queries/GetBestSellerProducts.cs
MechKey.Application/Catalog/Queries/GetProductRatings.cs
MechKey.Application/Catalog/Queries/GetDashboard.cs
```

**Modified (5 files):**
```
MechKey.Application/MechKey.Application.csproj          (+MediatR package)
MechKey.Application/DependencyInjection.cs              (+AddMediatR)
MechKey.WebAPI/Controllers/CategoryController.cs         (ISender)
MechKey.WebAPI/Controllers/ProductController.cs          (ISender)
MechKey.WebAPI/Controllers/DashboardController.cs        (ISender)
```

**Untouched (service layer preserved):**
```
MechKey.Application/Services/CategoryService.cs
MechKey.Application/Services/ProductService.cs
MechKey.Application/Services/ProductRatingService.cs
MechKey.Application/Services/DashboardService.cs
MechKey.Application/Interfaces/IServices/*.cs
MechKey.Application/Validators/CategoryValidator.cs
MechKey.Application/Validators/ProductValidator.cs
```
