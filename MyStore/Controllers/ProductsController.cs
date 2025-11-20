using Commen.ViewModels;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using Services.Interfaces;

namespace MyStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly StoreDbContext _context;
        public ProductsController(IProductService productService, StoreDbContext contex)
        {
            _productService = productService;
            _context = contex;
        }

        [HttpPost, Route("addProduct")]
        public async Task<IActionResult> AddProduct([FromBody]ProductAddVM productAddVM)
        {
            await _productService.AddProduct(productAddVM);
            return Ok();
        }

        [HttpGet, Route("getProducts")]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _productService.GetAllProducts();

                var result = _context.Product.Include(x => x.Category)
                             .GroupBy(c => c.Category)
                             .ToList();
                return Ok(products);
            }
            catch (Exception ex)
            {

                throw;
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
    }
}
