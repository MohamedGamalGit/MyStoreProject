using AutoMapper;
using Commen.ViewModels;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Models.Models;
using MyStore.Attributes;
using Repositories.IGenericRepository;
using Services.Interfaces;
using System.Text.Json;

namespace MyStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IGenericRepository<Category> _CategoryRepository;
        private readonly StoreDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly IMapper _mapper;

        public ProductsController(IProductService productService, StoreDbContext contex, IGenericRepository<Category> CategoryRepository, IDistributedCache cache, IMapper mapper)
        {
            _productService = productService;
            _context = contex;
            _CategoryRepository = CategoryRepository;
            _cache = cache;
            _mapper = mapper;
        }

        [HttpPost, Route("addProduct")]
        [HasPermission(Permissions.Products.Create)]
        public async Task<IActionResult> AddProduct([FromBody]ProductAddVM productAddVM)
        {
            await _productService.AddProduct(productAddVM);
            return Ok();
        }

        [HttpGet, Route("getProducts")]
        [HasPermission(Permissions.Products.View)]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _productService.GetAllProducts();

                var result = _context.Product.Include(x => x.Category)
                             .GroupBy(c => c.Category)
                             .ToList();
                return Ok(_mapper.Map<List<ProductAddVM>>(products) );
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
        [HttpGet, Route("getProductsWithCash")]
        //[HasPermission(Permissions.Products.View)]
        public async Task<IActionResult> getProductsWithCash()
        {
            string cacheKey = "products_all";
            List<ProductAddVM> productDtos;

            try
            {
                // محاولة قراءة البيانات من الكاش
                var cachedData = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    productDtos = JsonSerializer.Deserialize<List<ProductAddVM>>(cachedData);
                    return Ok(new { Source = "Cache", Data = productDtos });
                }
            }
            catch (StackExchange.Redis.RedisConnectionException redisEx)
            {
                // Redis غير متاح، ممكن تسجيل الخطأ بدون ايقاف التطبيق
                Console.WriteLine($"Redis unavailable: {redisEx.Message}");
            }
            catch (Exception ex)
            {
                // أي خطأ آخر في الكاش
                Console.WriteLine($"Cache error: {ex.Message}");
            }

            try
            {
                // جلب البيانات من قاعدة البيانات
                productDtos = await _context.Product
                                            .Include(p => p.Category)
                                            .Select(p => new ProductAddVM
                                            {
                                                Id = p.Id,
                                                Name = p.Name,
                                                Price = p.Price,
                                            })
                                            .ToListAsync();

                // محاولة تخزين البيانات في الكاش لو Redis شغال
                try
                {
                    var serializedData = JsonSerializer.Serialize(productDtos);
                    await _cache.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                    });
                }
                catch (Exception cacheEx)
                {
                    Console.WriteLine($"Failed to set cache: {cacheEx.Message}");
                }

                return Ok(new { Source = "Database", Data = productDtos });
            }
            catch (Exception dbEx)
            {
                // لو فيه خطأ في قاعدة البيانات
                return StatusCode(500, $"حدث خطأ في جلب البيانات: {dbEx.Message}");
            }
        }


        [HttpGet, Route("getProductById/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var products = await _productService.GetProductById(id);
            return Ok(products);
        }

        [HttpPost, Route("editProduct")]
        public async Task<IActionResult> EditProduct([FromBody] ProductAddVM productAddVM)
        {
            await _productService.EditProduct(productAddVM);
            return Ok();
        }
        [HttpGet("getProductsWithPagination")]
        public async Task<ActionResult<Pagination<Product>>> GetProducts([FromQuery] PaginationVM model)
        {
            var products = await _productService.GetAllProducts();
            var result = products
                         .Skip((model.Page - 1) * model.Size)
                         .Take(model.Size).GroupBy(c => c.Category)
                         .ToList();
            var pagination = new Pagination<Product>
            {
                TotalRecords = products.Count,
                TotalPages = (int)Math.Ceiling((double)products.Count / model.Size),
                Data = products
                        .Skip((model.Page - 1) * model.Size)
                        .Take(model.Size)
                        .ToList()
            };

            return Ok(pagination);
        }

        [HttpGet("getNumberOfPeoductsAndCategories")]
        public async Task<IActionResult> GetNumberOfPeoductsAndCategories()
        {
            var products = await _productService.GetAllProducts();
            var categories = await _CategoryRepository.GetAllAsync();
            return Ok(new { NumberOfProducts = products.Count, NumberOfCategories = categories.Count() });
        }
    }
}
