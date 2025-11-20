using Commen.ViewModels;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IProductService
    {
        Task<int> AddProduct(ProductAddVM productAddVM);
        Task EditProduct(ProductAddVM productAddVM);
        Task<Product> GetProductById(int id);
        Task<List<Product>> GetAllProducts();
        Task<List<Product>> GetAllProductsByCategoryId(int categoryId);
        Task DeleteProduct(int id);
    }
}
