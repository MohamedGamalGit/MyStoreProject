using AutoMapper;
using Commen.ViewModels;
using Commen.ViewModels.Employees;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using Repositories.IGenericRepository;

namespace MyStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IGenericRepository<Employee> _employeeRepository;
        private readonly StoreDbContext _dbContext;
        private readonly IMapper _mapper;

        public EmployeesController(IGenericRepository<Employee> employeeRepository, IMapper mapper, StoreDbContext storeDbContext)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _dbContext = storeDbContext;
        }


        [HttpPost("addEmployee")]
        public async Task<IActionResult> AddEmployee(EmployeeAddVM employeeAddVM)
        {
            try
            {
                var model = _mapper.Map<Employee>(employeeAddVM);
                await _employeeRepository.AddAsync(model);
                await _employeeRepository.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpGet("getAllEmployees")]
        public async Task<ActionResult<ResponseVM<EmployeeAddVM>>> GetAllEmployees()
        {
            try
            {
                var employees = await _employeeRepository.GetAllAsync();
                var data = _mapper.Map<List<EmployeeAddVM>>(employees?.ToList());
                return Ok(new ResponseVM<EmployeeAddVM>()
                {
                    Code = 200,
                    Message = "Employee Get successfully",
                    Data = employees.Count() > 0 ? data : null
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet("getById/{id}")]
        public async Task<ActionResult<ResponseVM<EmployeeAddVM>>> GetById(int id)
        {
            var result = await _employeeRepository.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new ResponseVM<EmployeeAddVM>()
                {
                    Code = 200,
                    Message = "Employee Not Fount",
                    Data = null
                });
            }

            return Ok(new ResponseVM<EmployeeAddVM>()
            {
                Code = 200,
                Message = "Employee updated successfully",
                Data = new List<EmployeeAddVM>() { _mapper.Map<EmployeeAddVM>(result) }
            });
        }
        [HttpPost("update")]
        public async Task<ActionResult<ResponseVM<EmployeeAddVM>>> UpdateEmployee(EmployeeAddVM employee)
        {
            try
            {
                var existing = await _employeeRepository.GetByIdAsync(employee.Id);
                if (existing == null)
                {
                    return NotFound(new ResponseVM<EmployeeAddVM>()
                    {
                        Code = 404,
                        Message = "Employee not found",
                        Data = null
                    });
                }

                _mapper.Map(employee, existing);
                await _employeeRepository.SaveChangesAsync();

                return Ok(new ResponseVM<EmployeeAddVM>()
                {
                    Code = 200,
                    Message = "Employee updated successfully",
                    Data = new List<EmployeeAddVM>() { employee }
                });
            }
            catch (Exception ex)
            {
                // إذا عندك EF context
                //_context.ChangeTracker.Clear();

                return BadRequest(new ResponseVM<EmployeeAddVM>()
                {
                    Code = 400,
                    Message = ex.InnerException?.Message ?? "Employee update error",
                    Data = null
                });
            }
        }

        //[HttpPost("update")]
        //public async Task<ResponseVM<EmployeeAddVM>> UpdateEmployee(EmployeeAddVM employee)
        //{
        //    try
        //    {

        //        _employeeRepository.Update(_mapper.Map<Employee>(employee));
        //        await _employeeRepository.SaveChangesAsync();


        //        return new ResponseVM<EmployeeAddVM>()
        //        {
        //            Code = 200,
        //            Message = "Employee updated successfully",
        //            Data = new List<EmployeeAddVM>() { employee }
        //        };
        //    }
        //    catch (Exception ex)
        //    {

        //        return new ResponseVM<EmployeeAddVM>()
        //        {
        //            Code = 400,
        //            Message = "Employee updated Error",
        //            Data = null
        //        };
        //    }

        //}
    }
    public class MyCustomResult : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context)
        {
            // هنا تحدد Status Code والـ Body
            context.HttpContext.Response.StatusCode = 200;
            return context.HttpContext.Response.WriteAsync("Hello");
        }
    }
}
