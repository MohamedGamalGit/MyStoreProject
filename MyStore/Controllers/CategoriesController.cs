using AutoMapper;
using Commen.ViewModels;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using MyStore.Attributes;
using Repositories.IGenericRepository;
using Services.Interfaces;
using Services.Services;

namespace MyStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly IGenericRepository<Category> _CategoryRepository;
        private readonly IMapper _mapper;
        private readonly StoreDbContext _context;
        public CategoriesController(IGenericRepository<Category> CategoryRepository, IMapper mapper, StoreDbContext context)
        {
            _CategoryRepository = CategoryRepository;
            _mapper = mapper;
            _context = context;
        }

        [HttpPost, Route("addCategory")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryAddVM categoryVM)
        {
            try
            {
                var model = _mapper.Map<Category>(categoryVM);
                await _CategoryRepository.AddAsync(model);
                await _CategoryRepository.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
        [HttpPost, Route("updateCategory")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryAddVM categoryVM)
        {
            try
            {
                var model = _mapper.Map<Category>(categoryVM);
                _CategoryRepository.Update(model);
                await _CategoryRepository.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        [HttpGet, Route("getCategories")]
        [HasPermission(Permissions.Categories.View)]

        public async Task<IActionResult> GetCategories()
        {
            var Categories = await _CategoryRepository.GetAllAsync();
            return Ok(Categories);
        }
        [HttpPost("getCategoriesWithagination")]
        [HasPermission(Permissions.Categories.View)]
        public async Task<ActionResult<Pagination<Category>>> GetCategoriesWithagination([FromBody] PaginationVM model)
        {
            var categories= await _CategoryRepository.GetAllAsync();

            var pagination = new Pagination<Category>
            {
                TotalRecords = categories.Count(),
                TotalPages = (int)Math.Ceiling((double)categories.Count() / model.Size),
                Data = categories
                        .Skip((model.Page - 1) * model.Size)
                        .Take(model.Size)
                        .ToList()
            };

            return Ok(pagination);
        }

        [HttpGet, Route("getCategoryById/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var Category = await _CategoryRepository.GetByIdAsync(id);
            return Ok(Category);
        }
        [HttpGet, Route("deleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var model = await _CategoryRepository.GetByIdAsync(id);
                _CategoryRepository.Delete(model);
                await _CategoryRepository.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
    }

}
