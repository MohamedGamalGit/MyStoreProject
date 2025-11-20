using AutoMapper;
using Commen.ViewModels;
using Models.Models;
using Repositories.IGenericRepository;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IMapper _mapper;
        public ProductService(IGenericRepository<Product> productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }
        public async Task<int> AddProduct(ProductAddVM productAddVM)
        {
            try
            {
                var product = _mapper.Map<Product>(productAddVM);
                await _productRepository.AddAsync(product);
                await _productRepository.SaveChangesAsync();
                return product.Id;
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        public async Task DeleteProduct(int id)
        {

            var product =await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new Exception("Product not found");
            _productRepository.Delete(product);
            await _productRepository.SaveChangesAsync();

        }

        public async Task EditProduct(ProductAddVM productAddVM)
        {
            var product = _mapper.Map<Product>(productAddVM);
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task<List<Product>> GetAllProducts()
        {
           return (await _productRepository.GetAllAsync()).ToList();
        }

        public async Task<List<Product>> GetAllProductsByCategoryId(int categoryId)
        {
            var product= await _productRepository.GetAllAsync();
            return product == null ? new List<Product>() : product.Where(x=>x.CategoryId== categoryId).ToList();
        }

        public async Task<Product>  GetProductById(int id)
        {
           return await _productRepository.GetByIdAsync(id);
        }
    }
}
